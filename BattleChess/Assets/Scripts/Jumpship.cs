public class Jumpship : Piece
{
    protected override void Awake()
    {
        base.Awake();
        this.OnAttackComplete += Jumpship_OnAttackComplete;
    }

    private void Jumpship_OnAttackComplete(Piece obj)
    {
        BoardHighlights.Instance.HideHighlights();
        PlayerManager.Instance.IsTurnComplete = true;
        obj.InvokeOnTurnComplete();
    }

    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];

        // UpLeft
        this.CheckPosition(this.CurrentX - 1, this.CurrentY + 2, ref r);

        // UpRight
        this.CheckPosition(this.CurrentX + 1, this.CurrentY + 2, ref r);

        // RightUp
        this.CheckPosition(this.CurrentX + 2, this.CurrentY + 1, ref r);

        // RightDown
        this.CheckPosition(this.CurrentX + 2, this.CurrentY - 1, ref r);

        // DownLeft
        this.CheckPosition(this.CurrentX - 1, this.CurrentY - 2, ref r);

        // DownRight
        this.CheckPosition(this.CurrentX + 1, this.CurrentY - 2, ref r);

        // LeftUp
        this.CheckPosition(this.CurrentX - 2, this.CurrentY + 1, ref r);

        // LeftDown
        this.CheckPosition(this.CurrentX - 2, this.CurrentY - 1, ref r);

        return r;
    }

    public void CheckPosition(int x, int y, ref bool[,] r)
    {
        Piece c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Pieces[x, y];
            if (c == null)
            {
                r[x, y] = true;
            }
        }
    }

}
