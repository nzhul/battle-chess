using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : PieceManager
{
    #region Singleton
    private static EnemyManager _instance;

    public static EnemyManager Instance
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

    public bool IsEnemyTurnComplete()
    {
        foreach (var enemy in this.Pieces)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            if (!enemy.IsTurnComplete)
            {
                return false;
            }
        }

        return true;
    }

    public void StartAITurn()
    {
        if (PlayerManager.Instance.AreAllPiecesDead())
        {
            GameManager.Instance.EndGame(Faction.AI, "All human pieces are destroyed!");
            return;
        }

        if (!this.IsEnemyTurnComplete())
        {
            return;
        }

        EnemyPiece enemy = this.FindNextEnemyToPlay();

        if (enemy != null && !enemy.IsDead)
        {
            enemy.IsTurnComplete = false;
            enemy.PlayTurn();
        }
        else
        {
            Debug.Log("All enemy actions are consumed! Skipping AI Turn!");
            this.CompletePiecesTurns();
            PlayerManager.Instance.SelectRandomPiece();
            GameManager.Instance.UpdateTurn();
        }
    }

    private EnemyPiece FindNextEnemyToPlay()
    {
        return this.Pieces.Where(p => p.ActionConsumed == false)
            .Cast<EnemyPiece>()
            .OrderBy(p => p.Initiative)
            .FirstOrDefault();
    }
}