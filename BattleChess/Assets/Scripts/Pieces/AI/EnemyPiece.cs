using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sensor))]
public abstract class EnemyPiece : Piece
{
    [Tooltip("Determines the order of taking an action compared to other AI pieces. " +
        "Higher is better. Ex: Piece with Initiative 2 will act before piece with initiative 1")]
    public int Initiative = 0;

    protected override void Awake()
    {
        base.Awake();
        this.IsTurnComplete = true;
    }

    public void PlayTurn()
    {
        if (this.IsDead)
        {
            this.FinishTurn();
            return;
        }

        StartCoroutine(PlayTurnRoutine());
    }

    IEnumerator PlayTurnRoutine()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            yield return new WaitForSeconds(0f);
            this.ExecuteTurn();
        }
    }

    protected virtual void TryMove()
    {
        Coord destination = this.TryFindDestination();

        if (destination != null)
        {
            BoardManager.Instance.Pieces[this.CurrentX, this.CurrentY] = null;
            base.motor.Move(BoardManager.Instance.GetTileCenter(destination.X, destination.Y));
            this.SetPosition(destination.X, destination.Y);
            BoardManager.Instance.Pieces[destination.X, destination.Y] = this;
        }
        else
        {
            Debug.Log(string.Format("CommandUnit at {0}:{1} cannot find destination. Skipping movement!", this.CurrentX, this.CurrentY));
            this.motor.InvokeOnMovementComplete();
        }
    }

    protected virtual List<Coord> FindPossibleDestinations()
    {
        List<Coord> destinations = new List<Coord>();

        bool[,] allowedMoves = this.PossibleMoves();

        for (int x = 0; x < allowedMoves.GetLength(0); x++)
        {
            for (int y = 0; y < allowedMoves.GetLength(1); y++)
            {
                if (allowedMoves[x, y])
                {
                    destinations.Add(new Coord(x, y));
                }
            }
        }

        return destinations;
    }

    protected abstract Coord TryFindDestination();

    protected abstract void ExecuteTurn();
}