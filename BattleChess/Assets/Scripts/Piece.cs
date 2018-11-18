using System;
using UnityEngine;

[RequireComponent(typeof(PieceMotor))]
public abstract class Piece : MonoBehaviour
{
    [Tooltip("How many tiles a pice can walk.")]
    public int Speed = 1;

    [Tooltip("If it reach 0, the piece is destroyed.")]
    public int Hitpoints = 1;

    public int AttackPower = 1;

    public int CurrentX { get; set; }

    public int CurrentY { get; set; }

    public bool IsHuman;

    public bool WalkConsumed { get; set; }

    public bool ActionConsumed { get; set; }

    public bool IsTurnComplete { get; set; }

    public event Action<Piece> OnTurnCompleted;

    public bool IsDead { get; set; }

    [HideInInspector]
    public PieceMotor motor;


    protected virtual void Awake()
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
        this.IsTurnComplete = true;
        this.WalkConsumed = true;
        this.ActionConsumed = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateTurn();
        }
    }
}
