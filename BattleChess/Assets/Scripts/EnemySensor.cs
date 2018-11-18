using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class EnemySensor : MonoBehaviour
{
    public Piece ClosestHumanPiece { get; set; }

    public List<Piece> AttackTargets { get; set; }

    private Piece _piece;
    private Type _pieceType;

    private void Awake()
    {
        this._piece = GetComponent<Piece>();
        this._pieceType = this._piece.GetType();
    }

    // We will move towards human piece only of the path is not blocked. (Else we will need A* pathfinding ...)
    public void DetectClosestTarget()
    {
        this.ClosestHumanPiece = null;

        float lowestDistance = float.MaxValue;

        foreach (var target in PlayerManager.Instance.Pieces)
        {
            float sqrDistance = (target.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < lowestDistance)
            {
                lowestDistance = sqrDistance;
                this.ClosestHumanPiece = target;
            }
        }

        Debug.Log(_pieceType.Name + " closest target is " + this.ClosestHumanPiece.name);
    }

    // We already moved towards human piece and no will attack, but first we need to detect all possible targets.
    public void DetectPossibleAttackTargets()
    {
        //throw new NotImplementedException();

        // switch _pieceType based on pieceType - do different kind of detection.
    }
}