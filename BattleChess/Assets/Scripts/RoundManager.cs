using System;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    #region Singleton
    private static RoundManager _instance;

    public static RoundManager Instance
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
    }
    #endregion

    public event Action OnNewRound;

    private void Start()
    {
        BoardManager.Instance.OnBoardInit += BoardManager_OnBoardInit;
    }

    public int TotalActionsLeft { get; set; }

    public int PlayerActionsLeft { get; set; }

    public int AIActionsLeft { get; set; }

    private void BoardManager_OnBoardInit()
    {
        this.TotalActionsLeft = BoardManager.Instance.InitialPiecesCount;
        this.PlayerActionsLeft = PlayerManager.Instance.Pieces.Count;
        this.AIActionsLeft = EnemyManager.Instance.Pieces.Count;

        for (int x = 0; x < BoardManager.Instance.Pieces.GetLength(0); x++)
        {
            for (int y = 0; y < BoardManager.Instance.Pieces.GetLength(1); y++)
            {
                Piece p = BoardManager.Instance.Pieces[x, y];
                if (p != null)
                {
                    BoardManager.Instance.Pieces[x, y].OnTurnCompleted += RoundManager_OnTurnCompleted;
                }
            }
        }
    }

    private void RoundManager_OnTurnCompleted(Piece obj)
    {
        this.TotalActionsLeft--;

        if (obj.IsHuman)
        {
            this.PlayerActionsLeft--;
        }
        else if(!obj.IsHuman)
        {
            this.AIActionsLeft--;
        }

        if (this.TotalActionsLeft == 0)
        {
            this.NewRound();
        }

        GameManager.Instance.UpdateTurn();
    }

    private void NewRound()
    {
        this.TotalActionsLeft = PlayerManager.Instance.Pieces.Count(p => !p.IsDead) + EnemyManager.Instance.Pieces.Count(p => !p.IsDead);
        this.PlayerActionsLeft = PlayerManager.Instance.Pieces.Count(p => !p.IsDead);
        this.AIActionsLeft = EnemyManager.Instance.Pieces.Count(p => !p.IsDead);

        PlayerManager.Instance.RestoreWalkAndActions();

        EnemyManager.Instance.RestoreWalkAndActions();

        if (this.OnNewRound != null)
        {
            this.OnNewRound();
        }
    }
}