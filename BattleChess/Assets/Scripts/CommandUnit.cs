using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandUnit : EnemyPiece
{
    protected override void ExecuteTurn()
    {
        this.TryMoveSideway();
    }

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
    }

    private Coord TryFindDestination()
    {
        Coord destination = null;

        List<Coord> possibleDestinations = this.FindPossibleDestinations();

        // TODO:
        // use Pathfinding for all possible destinations
        // choose the path where distance from closest enemy is biggest!

        if (possibleDestinations != null && possibleDestinations.Count > 0)
        {
            // TODO: Change this with Random.
            // Dont forget that the CommandUnit must try to go away from closest human piece!
            // I will have to implement pathfinding!

            destination = possibleDestinations[Random.Range(0, 2)];
        }

        return destination;
    }

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
