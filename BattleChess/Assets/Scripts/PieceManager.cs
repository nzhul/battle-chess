using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    //TODO: this can be made as a property {get;set;}
    public List<Piece> Pieces;

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
        // Debug.Log("Restoring " + GetType().Name + " walk and actions!");
        foreach (var piece in this.Pieces)
        {
            if (!piece.IsDead)
            {
                piece.WalkConsumed = false;
                piece.ActionConsumed = false;
            }
        }
    }

    public bool AllActionsAreConsumed()
    {
        foreach (var piece in this.Pieces)
        {
            if (piece.ActionConsumed == false)
            {
                return false;
            }
        }
        return true;
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
        // are there any pieces that have not completed their action ?
        //return this.Pieces.Any(p => !p.ActionConsumed);
    }
}