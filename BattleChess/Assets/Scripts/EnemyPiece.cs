using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemySensor))]
public abstract class EnemyPiece : Piece
{
    protected EnemySensor sensor;

    [Tooltip("Determines the order of taking an action compared to other AI pieces. " +
        "Higher is better. Ex: Piece with Initiative 2 will act before piece with initiative 1")]
    public int Initiative = 0;

    protected override void Awake()
    {
        base.Awake();

        sensor = GetComponent<EnemySensor>();
        this.IsTurnComplete = true;
        this.motor.OnMovementComplete += Motor_OnMovementComplete; //TODO: Move this logic in every ENEMY Piece: Drone, Drag, CommandUnit
    }

    private void Motor_OnMovementComplete(Piece obj)
    {
        obj.InvokeOnTurnComplete();
    }

    public void PlayTurn()
    {
        if (this.IsDead)
        {
            this.FinishTurn();
            return;
        }

        StartCoroutine(PlayTurnRoutine());
    }

    // main enemy routine: detect closest human piece. Move towards it for possible. Attack the piece if possible.
    IEnumerator PlayTurnRoutine()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            yield return new WaitForSeconds(0f);
            this.ExecuteTurn();
        }
    }

    protected abstract void ExecuteTurn();
}