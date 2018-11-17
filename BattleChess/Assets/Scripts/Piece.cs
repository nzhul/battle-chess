using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public int Speed;

    public int Hitpoints = 1;

    public int AttackPower = 1;

    public int CurrentX { get; set; }

    public int CurrentY { get; set; }


    public bool IsHuman;

    public void SetPosition(int x, int y)
    {
        this.CurrentX = x;
        this.CurrentY = y;
    }

    public virtual bool[,] PossibleMoves()
    {
        return new bool[8, 8]; //TODO: use grid size; from boardManager;
    }
}
