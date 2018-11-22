using UnityEngine;
using UnityEngine.UI;

public class LevelPicker : MonoBehaviour
{
    private BoardComposition[] regularScenarios;
    private BoardComposition[] testScenarios;

    public RectTransform _regularScenariosContainer;
    public Button _scenarioBtnPrefab;

    private void Awake()
    {
        regularScenarios = Resources.LoadAll<BoardComposition>("BoardCompositions/Scenarios");
        testScenarios = Resources.LoadAll<BoardComposition>("BoardCompositions/TestScenarios");
    }

    private void Start()
    {
        InitializeScenarioButtons();
    }

    private void InitializeScenarioButtons()
    {
        Common.Empty(_regularScenariosContainer.transform);

        if (this.regularScenarios != null && this.regularScenarios.Length > 0)
        {
            foreach (var scenario in this.regularScenarios)
            {
                Button scenarioBtn = Instantiate<Button>(_scenarioBtnPrefab, _regularScenariosContainer);
                scenarioBtn.name = scenario.Name + "_Btn";
                scenarioBtn.onClick.AddListener(delegate { OnHeroButtonPressed(scenario); });

                Text scenarioNameText = scenarioBtn.transform.Find("Name").GetComponent<Text>();
                scenarioNameText.text = scenario.Name;

                Image scenarioPreviewImage = scenarioBtn.transform.Find("PreviewImage").GetComponent<Image>();
                scenarioPreviewImage.sprite = scenario.PreviewImage;
            }
        }
        else
        {
            Debug.LogWarning("Cannot load regular scenarios!");
        }
    }

    private void OnHeroButtonPressed(BoardComposition scenario)
    {
        BoardManager.Instance.CurrentScenario = scenario;
        GameManager.Instance.PlayLevel();
    }
}