using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class Sensor : MonoBehaviour
{
    public Piece ClosestEnemyPiece { get; set; }

    public List<Piece> AttackTargets { get; set; }

    private Piece _piece;
    private Type _pieceType;

    private void Awake()
    {
        this._piece = GetComponent<Piece>();
        this._pieceType = this._piece.GetType();
    }

    public void DetectClosestTarget()
    {
        this.ClosestEnemyPiece = null;

        float lowestDistance = float.MaxValue;

        List<Piece> targets = _piece.IsHuman ? EnemyManager.Instance.Pieces : PlayerManager.Instance.Pieces;

        foreach (var target in targets)
        {
            float sqrDistance = (target.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < lowestDistance)
            {
                lowestDistance = sqrDistance;
                this.ClosestEnemyPiece = target;
            }
        }

        // Debug.Log(_pieceType.Name + " closest target is " + this.ClosestHumanPiece.name);
    }

    // We already moved towards human piece and no will attack, but first we need to detect all possible targets.
    public void DetectPossibleAttackTargets()
    {
        //throw new NotImplementedException();

        // switch _pieceType based on pieceType - do different kind of detection.

        this.AttackTargets = new List<Piece>();

        switch (_piece.DetectionMethod)
        {
            case DetectionMethod.Diagonal:
                this.AttackTargets = this.FindDiagonalTargets();
                break;
            case DetectionMethod.Orthogonal:
                this.AttackTargets = this.FindOrthagonalTargets();
                break;
            case DetectionMethod.Adjacent:
                this.AttackTargets = this.FindAdjacentTargets();
                break;
            default:
                break;
        }
    }

    private List<Piece> FindAdjacentTargets()
    {
        throw new NotImplementedException();
    }

    private List<Piece> FindOrthagonalTargets()
    {
        throw new NotImplementedException();
    }

    private List<Piece> FindDiagonalTargets()
    {
        List<Piece> targets = new List<Piece>();

        Direction checkDirection = Direction.Up;
        if (_pieceType == typeof(Drone))
        {
            checkDirection = Direction.Down;
        }
        else if (_pieceType == typeof(Grunt))
        {
            checkDirection = Direction.Up;
        }

        if (checkDirection == Direction.Up)
        {
            Piece piece;
            int i, j;
            int maxI;

            // Top Left
            i = this._piece.CurrentX;
            j = this._piece.CurrentY;

            while (true)
            {
                i--;
                j++;
                maxI = this._piece.CurrentX - this._piece.ShootRange;
                if (i < maxI || i < 0 || j >= 8)
                {
                    break;
                }

                piece = BoardManager.Instance.Pieces[i, j];
                if (piece == null)
                {
                    continue;
                }
                else
                {
                    targets.Add(piece);
                    break;
                }
            }

            // Top Right
            i = this._piece.CurrentX;
            j = this._piece.CurrentY;
            maxI = this._piece.CurrentX + this._piece.ShootRange;
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
                    continue;
                }
                else
                {
                    targets.Add(piece);
                    break;
                }
            }
        }
        else if (checkDirection == Direction.Down)
        {
            Piece piece;
            int i, j;
            int maxI;

            // Down Left
            i = this._piece.CurrentX;
            j = this._piece.CurrentY;
            maxI = this._piece.CurrentX - this._piece.ShootRange;

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
                    continue;
                }
                else
                {
                    targets.Add(piece);
                    break;
                }
            }

            // Down Right
            i = this._piece.CurrentX;
            j = this._piece.CurrentY;
            maxI = this._piece.CurrentX + this._piece.ShootRange;

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
                    continue;
                }
                else
                {
                    targets.Add(piece);
                    break;
                }
            }
        }

        if (this._piece.IsHuman)
        {
            targets = targets.Where(t => !t.IsHuman).ToList();
        }
        else
        {
            targets = targets.Where(t => t.IsHuman).ToList();
        }

        return targets;
    }
}