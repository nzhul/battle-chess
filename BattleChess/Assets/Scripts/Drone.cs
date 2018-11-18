public class Drone : EnemyPiece
{
    protected override void ExecuteTurn()
    {
        this.TryMoveForward();
        base.sensor.DetectPossibleAttackTargets();
        this.ShootAtRandomTargetIfPossible();
    }

    private void ShootAtRandomTargetIfPossible()
    {
        //throw new NotImplementedException();
    }

    private void TryMoveForward()
    {
        Coord destination = this.TryFindDestination();

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

        bool[,] allowedMoves = this.PossibleMoves();

        int destX = 0;
        int destY = int.MaxValue;

        for (int x = 0; x < allowedMoves.GetLength(0); x++)
        {
            for (int y = 0; y < allowedMoves.GetLength(1); y++)
            {
                if (allowedMoves[x, y])
                {
                    if (y < destY)
                    {
                        destX = x;
                        destY = y;
                    }
                }
            }
        }

        if (destY != int.MaxValue)
        {
            destination = new Coord(destX, destY);
        }

        return destination;
    }

    // TODO: Extract different directions into functions and then in PossibleMoves use only the functions:
    // Ex: Down, Up, Left, TopLeft ...
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8]; // TODO: calculate this dynamicaly
        Piece piece;
        int i;
        int maxI;

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

        return r;
    }
}
