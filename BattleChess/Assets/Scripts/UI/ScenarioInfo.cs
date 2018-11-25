using UnityEngine;
using UnityEngine.UI;

public class ScenarioInfo : MonoBehaviour
{
    public Text Scenario;
    public Text Round;
    public Text Human;
    public Text AI;

    public string scenarioTemplate = "SCENARIO: {0}";
    public string roundTemplate = "ROUND: {0}";
    public string humanTemplate = "HUMAN PIECES: {0}";
    public string aiTemplate = "AI PIECES: {0}";

    private void Start()
    {
        BoardManager.Instance.OnBoardInit += OnBoardInit;
    }

    private void OnBoardInit()
    {
        this.Scenario.text = string.Format(scenarioTemplate, BoardManager.Instance.CurrentScenario.Name);
        this.Round.text = string.Format(roundTemplate, 1);
        this.Human.text = string.Format(humanTemplate, PlayerManager.Instance.Pieces.Count);
        this.AI.text = string.Format(aiTemplate, EnemyManager.Instance.Pieces.Count);

        RoundManager.Instance.OnNewRound += OnNewRound;

        foreach (var piece in PlayerManager.Instance.Pieces)
        {
            piece.OnDeath += OnHumanPieceDeath;
        }

        foreach (var piece in EnemyManager.Instance.Pieces)
        {
            piece.OnDeath += OnAIPieceDeath;
        }
    }

    private void OnAIPieceDeath()
    {
        this.AI.text = string.Format(aiTemplate, EnemyManager.Instance.Pieces.Count);
    }

    private void OnHumanPieceDeath()
    {
        this.Human.text = string.Format(humanTemplate, PlayerManager.Instance.Pieces.Count);
    }

    private void OnNewRound(int currentRound)
    {
        this.Round.text = string.Format(roundTemplate, currentRound);
    }
}