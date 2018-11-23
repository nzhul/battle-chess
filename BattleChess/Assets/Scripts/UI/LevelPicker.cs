using UnityEngine;
using UnityEngine.UI;

public class LevelPicker : MonoBehaviour
{
    private BoardComposition[] _regularScenarios;
    private BoardComposition[] _testScenarios;

    public RectTransform regularScenariosContainer;
    public RectTransform testScenariosContainer;
    public Button scenarioBtnPrefab;

    private void Awake()
    {
        _regularScenarios = Resources.LoadAll<BoardComposition>("BoardCompositions/Scenarios");
        _testScenarios = Resources.LoadAll<BoardComposition>("BoardCompositions/TestScenarios");
    }

    private void Start()
    {
        InitializeScenarioButtons();
    }

    private void InitializeScenarioButtons()
    {
        Common.Empty(regularScenariosContainer.transform);
        if (this._regularScenarios != null && this._regularScenarios.Length > 0)
        {
            foreach (var scenario in this._regularScenarios)
            {
                this.AppendScenario(scenario, regularScenariosContainer);
            }
        }
        else
        {
            Debug.LogWarning("Cannot load regular scenarios!");
        }

        Common.Empty(testScenariosContainer.transform);
        if (this._testScenarios != null && this._testScenarios.Length > 0)
        {
            foreach (var scenario in this._testScenarios)
            {
                this.AppendScenario(scenario, testScenariosContainer);
            }
        }
        else
        {
            Debug.LogWarning("Cannot load regular scenarios!");
        }
    }

    private void AppendScenario(BoardComposition scenario, RectTransform container)
    {
        Button scenarioBtn = Instantiate<Button>(scenarioBtnPrefab, container);
        scenarioBtn.name = scenario.Name + "_Btn";
        scenarioBtn.onClick.AddListener(delegate { OnHeroButtonPressed(scenario); });

        Text scenarioNameText = scenarioBtn.transform.Find("Name").GetComponent<Text>();
        scenarioNameText.text = scenario.Name;

        Image scenarioPreviewImage = scenarioBtn.transform.Find("PreviewImage").GetComponent<Image>();
        scenarioPreviewImage.sprite = scenario.PreviewImage;
    }

    private void OnHeroButtonPressed(BoardComposition scenario)
    {
        BoardManager.Instance.CurrentScenario = scenario;
        GameManager.Instance.PlayLevel();
    }
}