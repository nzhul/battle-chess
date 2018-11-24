using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    #region Singleton
    private static BoardManager _instance;

    public static BoardManager Instance
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
    }
    #endregion

    private const float TILE_SIZE = 1f;

    private const float TILE_OFFSET = .5f;

    private int _selectionX = -1;

    private int _selectionY = -1;

    private bool[,] _allowedMoves;

    private Quaternion _faceCameraOrientation = Quaternion.Euler(0, 180, 0);

    public LayerMask layerMask;

    public PieceInfoPanel infoPanel;

    public int InitialPiecesCount { get; set; }

    public BoardComposition CurrentScenario { get; set; }

    public Piece[,] Pieces { get; set; }

    public event Action OnBoardInit;

    private void Update()
    {
        UpdateSelection();

#if UNITY_EDITOR
        DrawDebugBoard();
#endif

        if (Input.GetMouseButtonDown(0) && PlayerManager.Instance.InputEnabled && !EventSystem.current.IsPointerOverGameObject())
        {
            if (this._selectionX >= 0 && this._selectionY >= 0)
            {
                if (PlayerManager.Instance.SelectedPiece == null)
                {
                    SelectPiece(this._selectionX, this._selectionY);
                }
                else
                {
                    Piece otherPiece = this.Pieces[this._selectionX, this._selectionY];
                    if (otherPiece == null && !PlayerManager.Instance.SelectedPiece.WalkConsumed)
                    {
                        MovePiece(this._selectionX, this._selectionY);
                    }
                    else if (otherPiece != null && otherPiece != PlayerManager.Instance.SelectedPiece && !otherPiece.ActionConsumed)
                    {
                        SelectPiece(this._selectionX, this._selectionY);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && GameManager.Instance.HasLevelStarted)
        {
            if (this._selectionX >= 0 && this._selectionY >= 0)
            {
                Piece piece = this.Pieces[this._selectionX, this._selectionY];
                if (piece != null)
                {
                    infoPanel.ShowPanel(piece);
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && GameManager.Instance.HasLevelStarted)
        {
            infoPanel.HidePanel();
        }
    }

    public void SelectPiece(int x, int y)
    {
        if (this.Pieces[x, y] == null)
        {
            return;
        }

        BoardHighlights.Instance.HideHighlights();

        if (this.Pieces[x, y].ActionConsumed || !this.Pieces[x, y].IsHuman)
        {
            return;
        }

        if (!this.Pieces[x, y].WalkConsumed)
        {
            this._allowedMoves = this.Pieces[x, y].PossibleMoves();
            BoardHighlights.Instance.HighlightAllowedMoves(this._allowedMoves);
        }

        PlayerManager.Instance.SelectedPiece = this.Pieces[x, y];
        BoardHighlights.Instance.HighlightSelection(x, y);
    }

    public void InitBoard()
    {
        this.Pieces = new Piece[8, 8];

        foreach (var entity in this.CurrentScenario.Entities)
        {
            if (entity.PiecePrefab.IsHuman)
            {
                Piece spawnedPiece = SpawnPiece(entity.PiecePrefab.gameObject, entity.X, entity.Y, Quaternion.identity);
                if (spawnedPiece != null)
                {
                    PlayerManager.Instance.Pieces.Add(spawnedPiece);
                }
            }

            if (!entity.PiecePrefab.IsHuman)
            {
                Piece spawnedPiece = SpawnPiece(entity.PiecePrefab.gameObject, entity.X, entity.Y, _faceCameraOrientation);
                if (spawnedPiece != null)
                {
                    EnemyManager.Instance.Pieces.Add(spawnedPiece);
                }
            }
        }

        if (this.OnBoardInit != null)
        {
            this.OnBoardInit();
        }
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void MovePiece(int x, int y)
    {
        if (this._allowedMoves[x, y])
        {
            this.Pieces[PlayerManager.Instance.SelectedPiece.CurrentX, PlayerManager.Instance.SelectedPiece.CurrentY] = null;
            PlayerManager.Instance.SelectedPiece.motor.Move(this.GetTileCenter(x, y));
            PlayerManager.Instance.SelectedPiece.SetPosition(x, y);
            this.Pieces[x, y] = PlayerManager.Instance.SelectedPiece;
        }

        BoardHighlights.Instance.HideHighlights();
        PlayerManager.Instance.SelectedPiece = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, layerMask))
        {
            this._selectionX = (int)hit.point.x;
            this._selectionY = (int)hit.point.z;
        }
        else
        {
            this._selectionX = -1;
            this._selectionY = -1;
        }
    }

    private Piece SpawnPiece(GameObject prefab, int x, int y, Quaternion orientation)
    {
        if (this.Pieces[x, y] != null)
        {
            Debug.LogWarning("{0}:{1} is already occpied! Invalid Composition!");
            return null;
        }

        GameObject instance = Instantiate(prefab, this.GetTileCenter(x, y), orientation);
        instance.transform.SetParent(this.transform);
        this.Pieces[x, y] = instance.GetComponent<Piece>();
        this.Pieces[x, y].SetPosition(x, y);
        this.InitialPiecesCount++;

        return instance.GetComponent<Piece>();
    }

    private void DrawDebugBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        if (_selectionX >= 0 && _selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * _selectionY + Vector3.right * _selectionX, Vector3.forward * (_selectionY + 1) + Vector3.right * (_selectionX + 1));

            Debug.DrawLine(Vector3.forward * (_selectionY + 1) + Vector3.right * _selectionX, Vector3.forward * _selectionY + Vector3.right * (_selectionX + 1));
        }
    }
}