using UnityEngine;
using UnityEngine.UI;

public class PieceInfoPanel : MonoBehaviour
{
    public Color humanColor;
    public Color aiColor;
    public Color consumedColor;
    public Color availibleColor;

    Text pieceName;
    Text attack;
    Text hitpoints;
    Text speed;
    Text range;
    Text detection;
    Text attackMultiple;
    Image walkIndicator;
    Image actionIndicator;
    Image panelImage;

    public string attackTemplate = "Attack: {0}";
    public string hitpointsTemplate = "Hitpoints: {0}:{1}";
    public string speedTemplate = "Speed: {0}";
    public string rangeTemplate = "Range: {0}";
    public string detectionTemplate = "Detection: {0}";
    public string attackMultipleTemplate = "Attack multiple: {0}";

    private bool isInit;

    public void ShowPanel(Piece piece)
    {
        gameObject.SetActive(true);

        if (!isInit)
        {
            this.Init();
        }

        pieceName.text = piece.PieceType.Name;
        attack.text = string.Format(attackTemplate, piece.AttackPower);
        hitpoints.text = string.Format(hitpointsTemplate, piece.CurrentHitpoints, piece.Hitpoints);
        speed.text = string.Format(speedTemplate, piece.Speed);
        range.text = string.Format(rangeTemplate, piece.ShootRange);
        detection.text = string.Format(detectionTemplate, piece.DetectionMethod);
        attackMultiple.text = string.Format(attackMultipleTemplate, piece.AttackMethod == AttackMethod.All ? "Yes" : "No");
        walkIndicator.color = piece.WalkConsumed ? consumedColor : availibleColor;
        actionIndicator.color = piece.ActionConsumed ? consumedColor : availibleColor;
        panelImage.color = piece.IsHuman ? humanColor : aiColor;
    }

    private void Init()
    {
        panelImage = GetComponent<Image>();
        pieceName = transform.Find("Name").GetComponent<Text>();
        attack = transform.Find("Attack").GetComponent<Text>();
        hitpoints = transform.Find("HP").GetComponent<Text>();
        speed = transform.Find("Speed").GetComponent<Text>();
        range = transform.Find("Range").GetComponent<Text>();
        detection = transform.Find("Detection").GetComponent<Text>();
        attackMultiple = transform.Find("AttackMultiple").GetComponent<Text>();
        walkIndicator = transform.Find("WalkIndicator").GetComponent<Image>();
        actionIndicator = transform.Find("ActionIndicator").GetComponent<Image>();

        this.isInit = true;
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}