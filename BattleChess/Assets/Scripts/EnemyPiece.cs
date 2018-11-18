using System.Collections;
using UnityEngine;

public class EnemyPiece : Piece
{
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
            Debug.Log("Enemy plays: " + gameObject.name);
            // detect closest human piece
            // _enemySensor.UpdateSensor();

            // wait
            yield return new WaitForSeconds(0f);

            if (true) // _enemySensor.HumanPieceFound
            {
                // move towards closest human piece.

                // while wait for moving to complete

                // Attack if possible

                // complete the turn and set this.ActionConsumed = true;

                this.FinishTurn();
            }
        }
    }
}