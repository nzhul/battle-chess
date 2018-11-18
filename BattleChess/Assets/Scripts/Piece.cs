using System;
using UnityEngine;

[RequireComponent(typeof(PieceMotor))]
public abstract class Piece : MonoBehaviour
{
    public int Speed = 1;

    public int Hitpoints = 1;

    public int AttackPower = 1;

    public int CurrentX { get; set; }

    public int CurrentY { get; set; }

    public bool IsHuman;

    public bool WalkConsumed { get; set; }

    public bool ActionConsumed { get; set; }

    public event Action<Piece> OnActionConsumed;

    public bool IsDead { get; set; }

    [HideInInspector]
    public PieceMotor motor;

    protected bool _isTurnComplete = false;
    public bool IsTurnComplete { get { return _isTurnComplete; } set { _isTurnComplete = value; } }

    private void Awake()
    {
        this.motor = GetComponent<PieceMotor>();
    }

    public void SetPosition(int x, int y)
    {
        this.CurrentX = x;
        this.CurrentY = y;
    }

    public virtual bool[,] PossibleMoves()
    {
        return new bool[8, 8]; //TODO: use grid size; from boardManager;
    }

    public virtual void FinishTurn()
    {
        _isTurnComplete = true;
        this.WalkConsumed = true;
        this.ActionConsumed = true;

        if (this.OnActionConsumed != null)
        {
            this.OnActionConsumed(this);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateTurn();
        }
    }
}
