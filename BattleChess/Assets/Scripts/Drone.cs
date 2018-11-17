public class Drone : Piece
{
    // TODO: Change this method entirely! It temporary moves like Grunt
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8]; // TODO: calculate this dynamicaly
        Piece piece;
        int i;

        // Right
        i = this.CurrentX;

        while (true)
        {
            i++;
            if (i >= 8)
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
                if (piece.IsHuman != this.IsHuman) // TODO: delete this if statement and leave the break only.
                {
                    // in regular chess we will be allowed to go there.
                    // but in our game we are not allowed
                    // r[i, this.CurrentY] = true;
                }

                break;
            }
        }


        // Left
        i = this.CurrentX;

        while (true)
        {
            i--;
            if (i < 0)
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
                if (piece.IsHuman != this.IsHuman) // TODO: delete this if statement and leave the break only.
                {
                    // in regular chess we will be allowed to go there.
                    // but in our game we are not allowed
                    // r[i, this.CurrentY] = true;
                }

                break;
            }
        }


        // Up
        i = this.CurrentY;

        while (true)
        {
            i++;
            if (i >= 8)
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
                if (piece.IsHuman != this.IsHuman) // TODO: delete this if statement and leave the break only.
                {
                    // in regular chess we will be allowed to go there.
                    // but in our game we are not allowed
                    // r[i, this.CurrentY] = true;
                }

                break;
            }
        }


        // Down
        i = this.CurrentY;

        while (true)
        {
            i--;
            if (i < 0)
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
                if (piece.IsHuman != this.IsHuman) // TODO: delete this if statement and leave the break only.
                {
                    // in regular chess we will be allowed to go there.
                    // but in our game we are not allowed
                    // r[i, this.CurrentY] = true;
                }

                break;
            }
        }


        return r;
    }
}
