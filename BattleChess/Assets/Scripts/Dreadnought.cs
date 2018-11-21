using System.Collections.Generic;
using UnityEngine;

public class Dreadnought : EnemyPiece
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
        this.TryMove();
        this.ShootAtRandomTargetIfPossible();
    }

    private void ShootAtRandomTargetIfPossible()
    {
        //throw new NotImplementedException();
    }

    private void TryMove()
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
            destination = this.PickClosestToEnemyPosition(possibleDestinations);
        }

        return destination;
    }

    private Coord PickClosestToEnemyPosition(List<Coord> possibleDestinations)
    {
        Coord position = null;

        float lowestDistance = float.MaxValue;

        for (int i = 0; i < possibleDestinations.Count; i++)
        {
            Coord newPosition = possibleDestinations[i];
            Vector3 targetPosition = BoardManager.Instance.GetTileCenter(newPosition.X, newPosition.Y);

            float distance = this.GetDistanceBetweenPositions(this.sensor.ClosestHumanPiece.transform.position, targetPosition);

            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                position = newPosition;
            }
        }

        return position;
    }

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

    private List<Coord> FindPossibleDestinations()
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

    // TODO: Extract this in comman library
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8]; // TODO: calculate this dynamicaly
        Piece piece;
        int i, j;
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


        // Up
        i = this.CurrentY;
        maxI = this.CurrentY + this.Speed;

        while (true)
        {
            i++;
            if (i > maxI || i >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[this.CurrentX, i];
            if (piece == null)
            {
                r[this.CurrentX, i] = true;
            }
            else
            {
                break;
            }
        }


        // Down
        i = this.CurrentY;
        maxI = this.CurrentY - this.Speed;

        while (true)
        {
            i--;
            if (i < maxI || i < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[this.CurrentX, i];
            if (piece == null)
            {
                r[this.CurrentX, i] = true;
            }
            else
            {
                break;
            }
        }


        //BISHOP
        // Top Left
        i = this.CurrentX;
        j = this.CurrentY;

        while (true)
        {
            i--;
            j++;
            maxI = this.CurrentX - this.Speed;
            if (i < maxI || i < 0 || j >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, j];
            if (piece == null)
            {
                r[i, j] = true;
            }
            else
            {
                break;
            }
        }

        // Top Right
        i = this.CurrentX;
        j = this.CurrentY;
        maxI = this.CurrentX + this.Speed;
        while (true)
        {
            i++;
            j++;
            if (i > maxI || i >= 8 || j >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, j];
            if (piece == null)
            {
                r[i, j] = true;
            }
            else
            {
                break;
            }
        }

        // Down Left
        i = this.CurrentX;
        j = this.CurrentY;
        maxI = this.CurrentX - this.Speed;

        while (true)
        {
            i--;
            j--;
            if (i < maxI || i < 0 || j < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, j];
            if (piece == null)
            {
                r[i, j] = true;
            }
            else
            {
                break;
            }
        }

        // Down Right
        i = this.CurrentX;
        j = this.CurrentY;
        maxI = this.CurrentX + this.Speed;

        while (true)
        {
            i++;
            j--;
            if (i > maxI || i >= 8 || j < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, j];
            if (piece == null)
            {
                r[i, j] = true;
            }
            else
            {
                break;
            }
        }


        return r;
    }
}
