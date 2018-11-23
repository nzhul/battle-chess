using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public string VictoryText = "VICTORY";
    public string DefeatText = "DEFEAT";

    public Text OutcomeText;
    public Text OutcomeReasonText;

    public void UpdateText()
    {
        if (GameManager.Instance.IsGameOver)
        {
            this.OutcomeText.text = this.GetOutComeText(GameManager.Instance.Winner);
            this.OutcomeReasonText.text = GameManager.Instance.EndGameReason;
        }
    }

    public void ActivateWithDelay()
    {
        gameObject.SetActive(true);
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private string GetOutComeText(Faction winner)
    {
        if (winner == Faction.Human)
        {
            return VictoryText;
        }
        else
        {
            return DefeatText;
        }
    }
}