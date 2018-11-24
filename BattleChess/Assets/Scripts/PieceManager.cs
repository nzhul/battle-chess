using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public List<Piece> Pieces { get; set; }

    public bool AreAllPiecesDead()
    {
        foreach (var piece in this.Pieces)
        {
            if (!piece.IsDead)
            {
                return false;
            }
        }

        return true;
    }

    public void RestoreWalkAndActions()
    {
        foreach (var piece in this.Pieces)
        {
            if (!piece.IsDead)
            {
                piece.WalkConsumed = false;
                piece.ActionConsumed = false;
            }
        }
    }

    public bool HaveRemainingPiecesToAct()
    {
        bool result = false;

        foreach (var piece in this.Pieces)
        {
            if (!piece.ActionConsumed)
            {
                result = true;
            }
        }

        return result;
    }

    public void CompletePiecesTurns()
    {
        foreach (var piece in this.Pieces)
        {
            piece.IsTurnComplete = true;
        }
    }
}