﻿using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PieceMotor))]
[RequireComponent(typeof(Sensor))]
public abstract class Piece : MonoBehaviour, IDamageable
{
    [Tooltip("How many tiles a pice can walk.")]
    public int Speed = 1;

    [Tooltip("If it reach 0, the piece is destroyed.")]
    public int Hitpoints = 1;

    public int AttackPower = 1;

    public int ShootRange = 1;

    public DetectionMethod DetectionMethod;

    public AttackMethod AttackMethod;

    public int CurrentX { get; set; }

    public int CurrentY { get; set; }

    public bool IsHuman;

    public int CurrentHitpoints { get; set; }

    public bool WalkConsumed { get; set; }

    public bool ActionConsumed { get; set; }

    public bool IsTurnComplete { get; set; }

    public bool IsDead { get; set; }

    public Type PieceType { get; set; }

    public event Action<Piece> OnTurnCompleted;

    public event Action<Piece> OnDeath;

    public event Action<Piece> OnAttackComplete;

    [HideInInspector]
    public PieceMotor motor;

    [HideInInspector]
    public Sensor sensor;

    private void Piece_OnTurnCompleted(Piece obj)
    {
        this.FinishTurn();
    }

    protected virtual void Awake()
    {
        this.sensor = GetComponent<Sensor>();
        this.motor = GetComponent<PieceMotor>();
        this.OnTurnCompleted += Piece_OnTurnCompleted;
        this.CurrentHitpoints = this.Hitpoints;
        this.PieceType = this.GetType();
    }

    protected float GetDistanceToPosition(Vector3 position)
    {
        return (position - transform.position).sqrMagnitude;
    }

    protected float GetDistanceBetweenPositions(Vector3 p1, Vector3 p2)
    {
        return (p1 - p2).sqrMagnitude;
    }

    public void SetPosition(int x, int y)
    {
        this.CurrentX = x;
        this.CurrentY = y;
    }

    public void InvokeOnTurnComplete()
    {
        if (this.OnTurnCompleted != null)
        {
            this.OnTurnCompleted(this);
        }
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
    }

    public virtual void Shoot()
    {
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        // wait the piece to stop moving
        while (motor.isMoving)
        {
            yield return null;
        }


        // switch -> AttackMethod
        // sensor.AttackTargets[0].TakeHit()
        // this.TriggerAttackAnimation().

        // OnAttackComplete();

        if (this.sensor.AttackTargets != null && this.sensor.AttackTargets.Count > 0)
        {
            foreach (var target in this.sensor.AttackTargets)
            {
                target.TakeHit(this.AttackPower, Vector3.one);
            }
        }


        if (this.OnAttackComplete != null)
        {
            this.OnAttackComplete(this);
        }

    }

    public void TakeHit(int damage, Vector3 hitDirection)
    {
        // Do something with the hitDirection
        this.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        this.CurrentHitpoints -= damage;
        Debug.Log(string.Format("{0} took {1} damage. HP: {2}/{3}", this.PieceType.Name, damage, this.CurrentHitpoints, this.Hitpoints));
        if (this.CurrentHitpoints <= 0 && !IsDead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    private void Die()
    {
        if (!this.ActionConsumed)
        {
            RoundManager.Instance.TotalActionsLeft--;
            if (IsHuman)
            {
                RoundManager.Instance.PlayerActionsLeft--;
            }
            else
            {
                RoundManager.Instance.AIActionsLeft--;
            }
        }
        Debug.Log(string.Format("{0} at {1}:{2} died!", this.PieceType.Name, this.CurrentX, this.CurrentY));
        this.WalkConsumed = true;
        this.ActionConsumed = true;
        this.IsDead = true;
        //this.FinishTurn();
        PlayerManager.Instance.Pieces.Remove(this);
        BoardManager.Instance.Pieces[this.CurrentX, this.CurrentY] = null;

        //RoundManager.Instance.PieceActionsLeft--;
        //GameManager.Instance.UpdateTurn();

        //this.InvokeOnTurnComplete();

        //if (this.OnDeath != null)
        //{
        //    this.OnDeath(this);
        //}



        ////if (!this.ActionConsumed)
        ////{
        ////    RoundManager.Instance.PieceActionsLeft--;
        ////}

        ////TODO: wait for death animation to complete.
        gameObject.SetActive(false);
    }
}
