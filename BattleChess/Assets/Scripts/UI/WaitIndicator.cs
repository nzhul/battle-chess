using UnityEngine;
using UnityEngine.UI;

public class WaitIndicator : MonoBehaviour
{
    private Transform _ui;

    private Image _waitIcon;

    private Transform _cam;

    private Piece _piece;

    public GameObject uiPrefab;

    public Transform target;

    private bool shouldDisplay;

    private void Awake()
    {
        _cam = Camera.main.transform;

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.renderMode == RenderMode.WorldSpace)
            {
                _ui = Instantiate(uiPrefab, c.transform).transform;
                _ui.gameObject.SetActive(false);
                break;
            }
        }

        _piece = this.GetComponent<Piece>();
        _piece.OnActionStatusChange += Self_OnActionStatusChange;
    }

    private void Self_OnActionStatusChange(Piece piece)
    {
        if (_ui != null)
        {
            this.shouldDisplay = piece.ActionConsumed;

            if (_piece.CurrentHitpoints <= 0)
            {
                Destroy(_ui.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (this.shouldDisplay)
            {
                _ui.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            if (this.shouldDisplay)
            {
                _ui.gameObject.SetActive(false);
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