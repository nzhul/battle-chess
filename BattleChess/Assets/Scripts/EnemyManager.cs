using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Singleton that stores a collection of all active enemies on the board.
// and manages enemy turns
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

    public void CompleteEnemiesTurn()
    {
        foreach (var enemy in this.Pieces)
        {
            enemy.IsTurnComplete = true;
        }
    }

    public bool AreEnemiesAllDead()
    {
        foreach (var enemy in this.Pieces)
        {
            if (!enemy.IsDead)
            {
                return false;
            }
        }

        return true;
    }

    public void StartAITurn()
    {
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
            Debug.Log("Cannot find enemy to play! Skipping AI Round!");
            this.CompleteEnemiesTurn();
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
