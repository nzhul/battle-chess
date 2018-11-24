using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class Sensor : MonoBehaviour
{
    public Piece ClosestEnemyPiece { get; set; }

    public List<Piece> AttackTargets { get; set; }

    private Piece _piece;

    private void Awake()
    {
        this._piece = GetComponent<Piece>();
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
    }

    public void DetectPossibleAttackTargets()
    {
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

    // AdjacentTargets are combination of Diagonal and Orthagonal + limiting the piece range to 1
    private List<Piece> FindAdjacentTargets()
    {
        List<Piece> diagonalTargets = this.FindDiagonalTargets();
        List<Piece> orthogonalTargets = this.FindOrthagonalTargets();
        List<Piece> adjacentTargets = diagonalTargets.Concat(orthogonalTargets).ToList();

        return adjacentTargets;
    }

    private List<Piece> FindOrthagonalTargets()
    {
        List<Piece> targets = new List<Piece>();

        Piece piece;
        int i;
        int maxI;

        // Right
        i = this._piece.CurrentX;
        maxI = this._piece.CurrentX + this._piece.ShootRange;

        while (true)
        {
            i++;
            if (i > maxI || i >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, this._piece.CurrentY];
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


        // Left
        i = this._piece.CurrentX;
        maxI = this._piece.CurrentX - this._piece.ShootRange;

        while (true)
        {
            i--;
            if (i < maxI || i < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[i, this._piece.CurrentY];
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


        // Up
        i = this._piece.CurrentY;
        maxI = this._piece.CurrentY + this._piece.ShootRange;

        while (true)
        {
            i++;
            if (i > maxI || i >= 8)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[this._piece.CurrentX, i];
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


        // Down
        i = this._piece.CurrentY;
        maxI = this._piece.CurrentY - this._piece.ShootRange;

        while (true)
        {
            i--;
            if (i < maxI || i < 0)
            {
                break;
            }

            piece = BoardManager.Instance.Pieces[this._piece.CurrentX, i];
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

    private List<Piece> FindDiagonalTargets()
    {
        List<Piece> targets = new List<Piece>();

        Piece piece;
        int i, j;
        int maxI;

        // Top Left
        i = this._piece.CurrentX;
        j = this._piece.CurrentY;
        maxI = this._piece.CurrentX - this._piece.ShootRange;

        while (true)
        {
            i--;
            j++;
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