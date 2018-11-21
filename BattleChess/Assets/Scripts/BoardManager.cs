using System;
using System.Collections.Generic;
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

    private bool[,] AllowedMoves { get; set; }

    public Piece[,] Pieces { get; set; }

    private const float TILE_SIZE = 1f;
    private const float TILE_OFFSET = .5f;

    private int selectionX = -1; // TODO: use Coord struct instead.
    private int selectionY = -1;

    public List<GameObject> piecePrefabs;
    private List<GameObject> activePieces;

    private Quaternion faceCameraOrientation = Quaternion.Euler(0, 180, 0);

    public bool IsHumanTurn = true;

    public LayerMask layerMask;

    public event Action OnBoardInit;

    public int InitialPiecesCount { get; set; }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        if (Input.GetMouseButtonDown(0) && PlayerManager.Instance.InputEnabled && !EventSystem.current.IsPointerOverGameObject())
        {
            if (this.selectionX >= 0 && this.selectionY >= 0)
            {
                if (PlayerManager.Instance.SelectedPiece == null)
                {
                    SelectPiece(this.selectionX, this.selectionY);
                }
                else
                {
                    Piece otherPiece = this.Pieces[this.selectionX, this.selectionY];
                    if (otherPiece == null && !PlayerManager.Instance.SelectedPiece.WalkConsumed)
                    {
                        MovePiece(this.selectionX, this.selectionY);
                    }
                    else if (otherPiece != null && otherPiece != PlayerManager.Instance.SelectedPiece && !otherPiece.ActionConsumed)
                    {
                        SelectPiece(this.selectionX, this.selectionY);
                    }
                }
            }
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
            this.AllowedMoves = this.Pieces[x, y].PossibleMoves();
            BoardHighlights.Instance.HighlightAllowedMoves(this.AllowedMoves);
        }

        PlayerManager.Instance.SelectedPiece = this.Pieces[x, y];
        BoardHighlights.Instance.HighlightSelection(x, y);
    }

    private void MovePiece(int x, int y)
    {
        if (this.AllowedMoves[x, y])
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
            this.selectionX = (int)hit.point.x;
            this.selectionY = (int)hit.point.z;
        }
        else
        {
            this.selectionX = -1;
            this.selectionY = -1;
        }
    }

    private Piece SpawnPiece(int index, int x, int y, Quaternion orientation)
    {
        GameObject instance = Instantiate(piecePrefabs[index], this.GetTileCenter(x, y), orientation);
        instance.transform.SetParent(this.transform);
        this.Pieces[x, y] = instance.GetComponent<Piece>();
        this.Pieces[x, y].SetPosition(x, y);
        this.activePieces.Add(instance);
        this.InitialPiecesCount++;

        return instance.GetComponent<Piece>();
    }

    // TODO: change this hardcoded configuration by using some kind of battle configuration. Scriptable objects ?
    public void InitBoard()
    {
        this.activePieces = new List<GameObject>();
        this.Pieces = new Piece[8, 8];

        // 0 Grunt
        // 1 JumpShip
        // 2 Tank
        // 3 Drone
        // 4 Dreadnought
        // 5 CommandUnit
        // 6 Human Dreadnought

        // Spawn human force
        PlayerManager.Instance.Pieces.Add(SpawnPiece(2, 4, 3, Quaternion.identity)); // Tank
        PlayerManager.Instance.Pieces.Add(SpawnPiece(0, 3, 0, Quaternion.identity)); // Grunt
        PlayerManager.Instance.Pieces.Add(SpawnPiece(1, 4, 0, Quaternion.identity)); // Jumpship

        // Spawn AI force
        EnemyManager.Instance.Pieces.Add(SpawnPiece(3, 2, 6, faceCameraOrientation)); // Drone
        EnemyManager.Instance.Pieces.Add(SpawnPiece(3, 5, 6, faceCameraOrientation)); // Drone
        EnemyManager.Instance.Pieces.Add(SpawnPiece(3, 3, 6, faceCameraOrientation)); // Drone
        EnemyManager.Instance.Pieces.Add(SpawnPiece(4, 1, 7, faceCameraOrientation)); // Dreadnought
        EnemyManager.Instance.Pieces.Add(SpawnPiece(5, 3, 7, faceCameraOrientation)); // CommandUnit

        // tanks in enemy line
        //PlayerManager.Instance.Pieces.Add(SpawnPiece(0, 1, 1, Quaternion.identity));
        //PlayerManager.Instance.Pieces.Add(SpawnPiece(2, 7, 1, Quaternion.identity));



        // Drone at human start
        //EnemyManager.Instance.Pieces.Add(SpawnPiece(3, 0, 0, faceCameraOrientation));


        // Dreadnought Scenario
        // EnemyManager.Instance.Pieces.Add(SpawnPiece(4, 3, 7, faceCameraOrientation)); // Dreadnought


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

    private void DrawBoard()
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

        // Draw the selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX, Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX, Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }
}
