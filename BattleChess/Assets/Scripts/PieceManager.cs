using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public List<Piece> Pieces;

    protected void RestoreWalkAndActions()
    {
        Debug.Log("Restoring " + GetType().Name + " walk and actions!");
        foreach (var piece in this.Pieces)
        {
            piece.WalkConsumed = false;
            piece.ActionConsumed = false;
        }
    }

    protected bool AllActionsAreConsumed()
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
}