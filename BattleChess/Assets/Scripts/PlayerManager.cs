using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : PieceManager
{
    #region Singleton
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        this.Pieces = new List<Piece>();
    }
    #endregion

    public Button AttackButton;
    public Button DefendButton;

    private void Start()
    {
        BoardManager.Instance.OnBoardInit += BoardManager_OnBoardInit;
    }

    private void BoardManager_OnBoardInit()
    {
        for (int x = 0; x < BoardManager.Instance.Pieces.GetLength(0); x++)
        {
            for (int y = 0; y < BoardManager.Instance.Pieces.GetLength(1); y++)
            {
                Piece p = BoardManager.Instance.Pieces[x, y];
                if (p != null && p.IsHuman)
                {
                    BoardManager.Instance.Pieces[x, y].motor.OnMovementComplete += Motor_OnMovementComplete;
                }

                if (p != null && !p.IsHuman)
                {
                    BoardManager.Instance.Pieces[x, y].OnTurnCompleted += PlayerManager_OnTurnCompleted;
                }
            }
        }
    }

    private void PlayerManager_OnTurnCompleted(Piece piece)
    {
        if (!piece.IsHuman)
        {
            this.SelectRandomPiece();
        }
    }

    private void Motor_OnMovementComplete(Piece piece)
    {
        if (piece.IsHuman)
        {
            BoardManager.Instance.SelectPiece(piece.CurrentX, piece.CurrentY);
        }

    }

    bool _inputEnabled = false;
    public bool InputEnabled
    {
        get
        {
            return _inputEnabled;
        }
        set
        {
            _inputEnabled = value;

            this.EnableUIButtons(value);
        }
    }

    private void EnableUIButtons(bool value)
    {
        this.AttackButton.interactable = value;
        this.DefendButton.interactable = value;
    }

    public bool IsTurnComplete { get; set; }

    public Piece SelectedPiece { get; set; }

    public void Attack()
    {
        if (this.SelectedPiece != null)
        {
            if (this.SelectedPiece.ActionConsumed)
            {
                Debug.Log("This unit has already consumed his action");
                return;
            }

            this.InputEnabled = false;
            this.SelectedPiece.sensor.DetectPossibleAttackTargets();
            this.SelectedPiece.Attack();
        }
        else
        {
            Debug.Log("Please select a piece");
        }
    }

    public void Defend()
    {
        if (this.SelectedPiece != null)
        {
            if (this.SelectedPiece.ActionConsumed)
            {
                Debug.Log("This unit has already consumed his action");
                return;
            }

            BoardHighlights.Instance.HideHighlights();
            this.IsTurnComplete = true;
            this.SelectedPiece.InvokeOnTurnComplete();
        }
        else
        {
            Debug.Log("Please select a piece");
        }
    }

    public void StartHumanTurn()
    {
        if (EnemyManager.Instance.AreAllPiecesDead())
        {
            GameManager.Instance.EndGame(Faction.Human, "All AI pieces are destroyed!");
            return;
        }

        this.InputEnabled = true;
        this.IsTurnComplete = false;

        if (!this.HaveRemainingPiecesToAct() && (RoundManager.Instance.PlayerActionsLeft == 0))
        {
            // Skipping human turn because he have no more actions for this round!
            // must wait for AI to play all his pieces!
            this.InputEnabled = false;
            this.IsTurnComplete = true;
            GameManager.Instance.UpdateTurn();
        }
        else
        {
            if (this.SelectedPiece == null)
            {
                this.SelectRandomPiece();
            }
        }
    }

    public void SelectRandomPiece()
    {
        if (this.Pieces != null && this.Pieces.Count > 0)
        {
            PlayerManager.Instance.SelectedPiece = null;
            List<Piece> remainingPiecesToAct = this.Pieces.Where(p => !p.ActionConsumed).ToList();
            if (remainingPiecesToAct != null && remainingPiecesToAct.Count > 0)
            {
                Piece randomPiece = remainingPiecesToAct[UnityEngine.Random.Range(0, remainingPiecesToAct.Count)];
                BoardManager.Instance.SelectPiece(randomPiece.CurrentX, randomPiece.CurrentY);
            }
        }
    }
}