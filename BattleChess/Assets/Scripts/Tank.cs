public class Tank : Piece
{
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
