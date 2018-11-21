using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : EnemyPiece
{
    protected override void Awake()
    {
        base.Awake();

        this.motor.OnMovementComplete += Motor_OnMovementComplete; //TODO: Move this logic in every ENEMY Piece: Drone, Drag, CommandUnit
    }

    private void Motor_OnMovementComplete(Piece obj)
    {
        obj.InvokeOnTurnComplete();
    }

    protected override void ExecuteTurn()
    {
        this.sensor.DetectClosestTarget();
        this.TryMoveSideway();
    }

    // The command unit will try to move sideways without changing his X value
    // and also will try to avoid enemy if possible
    // if only possible destination will get the unit closer to the enemy.
    // command unit should skip the turn instead of moving.
    private void TryMoveSideway()
    {
        Coord destination = this.TryFindDestination();

        //TODO: Extract this logic in base method.
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

    private Coord TryFindDestination()
    {
        Coord destination = null;

        List<Coord> possibleDestinations = this.FindPossibleDestinations();

        if (possibleDestinations != null && possibleDestinations.Count > 0)
        {
            destination = this.PickSafestPosition(possibleDestinations);
        }

        return destination;
    }

    private bool NewDestinationIsDangerous(Coord destination)
    {
        foreach (var enemy in PlayerManager.Instance.Pieces)
        {
            float distanceToEnemy = this.GetDistanceToPosition(enemy.transform.position);
            if (distanceToEnemy <= this.sensor.ClosestEnemyDistance)
            {
                return true;
            }
        }

        return false;
    }

    private Coord PickSafestPosition(List<Coord> possibleDestinations)
    {
        Coord safePosition = null;

        for (int i = 0; i < possibleDestinations.Count; i++)
        {
            Coord newPosition = possibleDestinations[i];
            Vector3 targetPosition = BoardManager.Instance.GetTileCenter(newPosition.X, newPosition.Y);

            float newPositionClosestEnemyDistance = this.FindClosestEnemyDistance(targetPosition);

            if (newPositionClosestEnemyDistance > this.sensor.ClosestEnemyDistance)
            {
                safePosition = newPosition;
            }

        }

        return safePosition;
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

    // From all possible destinations.
    // here we will pick the furthest from us!
    private List<Coord> FindPossibleDestinations()
    {
        List<Coord> destinations = new List<Coord>();

        bool[,] allowedMoves = this.PossibleMoves();

        int minX = int.MaxValue;
        int minXY = 0;
        int maxX = int.MinValue;
        int maxXY = 0;


        for (int x = 0; x < allowedMoves.GetLength(0); x++)
        {
            for (int y = 0; y < allowedMoves.GetLength(1); y++)
            {
                if (allowedMoves[x, y])
                {
                    if (x < minX)
                    {
                        minX = x;
                        minXY = y;
                    }

                    if (x > maxX)
                    {
                        maxX = x;
                        maxXY = y;
                    }
                }
            }
        }

        if (minX != int.MaxValue)
        {
            destinations.Add(new Coord(minX, minXY));
        }

        if (maxX != int.MinValue)
        {
            destinations.Add(new Coord(maxX, maxXY));
        }

        // If we have only one possible destination we should remove 
        // one of the destinations because they are the same
        if (destinations.Count > 0 && minX == maxX)
        {
            destinations.RemoveAt(destinations.Count - 1);
        }

        return destinations;
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
