using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerManager : PieceManager
{
    #region Singleton
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        this.Pieces = new List<Piece>();
    }
    #endregion

    bool _inputEnabled = false;
    public bool InputEnabled { get { return _inputEnabled; } set { _inputEnabled = value; } }

    public bool IsTurnComplete { get; set; }

    public Piece SelectedPiece { get; set; }

    public void Attack()
    {
        if (this.SelectedPiece != null)
        {
            if (this.SelectedPiece.ActionConsumed)
            {
                Debug.Log("This unit has already consumed his action");
                return;
            }
            // Use currently selected unit
            // Scan for possible targets based on the selected unit attack type and range
            // mark the possible targets on the battle field
            // a player is in targeting mode. He must select a target or press cancel/escape

            this.SelectedPiece.ActionConsumed = true;
            this.SelectedPiece.WalkConsumed = true;

            // TODO extract this into action and subscribe somewhere.
            BoardHighlights.Instance.HideHighlights();
            this.IsTurnComplete = true;
            this.SelectedPiece.FinishTurn();
            this.SelectedPiece = null;
        }
        else
        {
            Debug.Log("Please select a piece");
        }
    }

    public void StartHumanTurn()
    {
        Debug.Log("Starting Human turn!");
        this.InputEnabled = true;
        this.IsTurnComplete = false;

        if (this.AllActionsAreConsumed())
        {
            this.RestoreWalkAndActions();
        }
    }
}