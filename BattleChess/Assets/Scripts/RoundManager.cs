using System;
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

    public int PieceActionsLeft { get; set; }

    private void BoardManager_OnBoardInit()
    {
        this.PieceActionsLeft = BoardManager.Instance.InitialPiecesCount;

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
        this.PieceActionsLeft--;

        if (this.PieceActionsLeft == 0)
        {
            this.NewRound();
        }
    }

    private void NewRound()
    {
        this.PieceActionsLeft = BoardManager.Instance.InitialPiecesCount; //TODO: handle dead here.

        // restore player movement and actions
        PlayerManager.Instance.RestoreWalkAndActions();

        // restore enemy movement and actions
        EnemyManager.Instance.RestoreWalkAndActions();

        if (this.OnNewRound != null)
        {
            this.OnNewRound();
        }
    }
}