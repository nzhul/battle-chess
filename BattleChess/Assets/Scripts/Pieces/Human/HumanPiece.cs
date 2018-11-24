public abstract class HumanPiece : Piece
{
    protected override void Awake()
    {
        base.Awake();
        this.OnAttackComplete += OnAttackCompleteSelf;
    }

    private void OnAttackCompleteSelf(Piece piece)
    {
        BoardHighlights.Instance.HideHighlights();
        PlayerManager.Instance.IsTurnComplete = true;
        piece.InvokeOnTurnComplete();
    }
}