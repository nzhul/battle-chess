using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : EnemyPiece
{
    protected override void Awake()
    {
        base.Awake();

        this.motor.OnMovementComplete += Motor_OnMovementComplete;
    }

    private void Motor_OnMovementComplete(Piece obj)
    {
        obj.InvokeOnTurnComplete();
    }

    protected override void ExecuteTurn()
    {
        this.sensor.DetectClosestTarget();
        this.TryMove();
    }

    protected override Coord TryFindDestination()
    {
        Coord destination = null;

        List<Coord> possibleDestinations = this.FindPossibleDestinations();

        if (possibleDestinations != null && possibleDestinations.Count > 0)
        {
            destination = this.PickSafestPosition(possibleDestinations);
        }

        return destination;
    }

    private Coord PickSafestPosition(List<Coord> possibleDestinations)
    {
        Coord position = null;

        float bestDistance = float.MinValue;

        for (int i = 0; i < possibleDestinations.Count; i++)
        {
            Coord newPosition = possibleDestinations[i];
            Vector3 targetPosition = BoardManager.Instance.GetTileCenter(newPosition.X, newPosition.Y);

            float distance = this.FindClosestEnemyDistance(targetPosition);

            if (distance > bestDistance)
            {
                bestDistance = distance;
                position = newPosition;
            }
        }

        if (position == null)
        {
            Debug.Log(string.Format("{0} cannot find safe position.", this.PieceType.Name));
        }

        return position;
    }

    //TODO: this method is almost identical to Piece.cs -> GetDistanceToPosition() - Refactor
    private float FindClosestEnemyDistance(Vector3 targetPosition)
    {
        float lowestDistance = float.MaxValue;

        foreach (var target in PlayerManager.Instance.Pieces)
        {
            float sqrDistance = (target.transform.position - targetPosition).sqrMagnitude;
            if (sqrDistance < lowestDistance)
            {
                lowestDistance = sqrDistance;
            }
        }

        return lowestDistance;
    }

    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8]; // TODO: calculate this dynamicaly
        Piece piece;
        int i;
        int maxI;

        // Right
        i = this.CurrentX;
        maxI = this.CurrentX + this.Speed;

        while (true)
        {
            i++;
            if (i > maxI || i >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, this.CurrentY];
            if (piece == null)
            {
                r[i, this.CurrentY] = true;
            }
            else
            {
                break;
            }
        }

        // Left
        i = this.CurrentX;
        maxI = this.CurrentX - this.Speed;

        while (true)
        {
            i--;
            if (i < maxI || i < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, this.CurrentY];
            if (piece == null)
            {
                r[i, this.CurrentY] = true;
            }
            else
            {
                break;
            }
        }

        return r;
    }
}