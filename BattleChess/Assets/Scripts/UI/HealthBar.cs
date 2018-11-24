using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Piece))]
public class HealthBar : MonoBehaviour
{
    private Transform _ui;

    private Image _healthSlider;

    private Transform _cam;

    private Piece _piece;

    public GameObject uiPrefab;

    public Transform target;

    private void Awake()
    {
        _cam = Camera.main.transform;

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.renderMode == RenderMode.WorldSpace)
            {
                _ui = Instantiate(uiPrefab, c.transform).transform;
                _healthSlider = _ui.GetChild(0).GetComponent<Image>();
                _ui.gameObject.SetActive(false);
                break;
            }
        }

        _piece = this.GetComponent<Piece>();
        _piece.OnHealthChange += OnHealthChange;
    }

    private void OnHealthChange(Piece obj)
    {
        if (_ui != null)
        {
            _ui.gameObject.SetActive(true);

            float healthPercent = _piece.CurrentHitpoints / (float)_piece.Hitpoints;
            _healthSlider.fillAmount = healthPercent;

            if (_piece.CurrentHitpoints <= 0)
            {
                Destroy(_ui.gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        if (_ui != null)
        {
            _ui.position = target.position;
            _ui.forward = -_cam.forward;
        }
    }
}