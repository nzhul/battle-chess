using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; set; }
    private bool[,] AllowedMoves { get; set; }

    public Piece[,] Pieces { get; set; }
    private Piece selectedPiece;

    private const float TILE_SIZE = 1f;
    private const float TILE_OFFSET = .5f;

    private int selectionX = -1; // TODO: use Coord struct instead.
    private int selectionY = -1;

    public List<GameObject> piecePrefabs;
    private List<GameObject> activePieces;

    private Quaternion faceCameraOrientation = Quaternion.Euler(0, 180, 0);

    public bool IsHumanTurn = true;

    public LayerMask layerMask;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnAllPieces();
    }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        if (Input.GetMouseButtonDown(0))
        {
            if (this.selectionX >= 0 && this.selectionY >= 0)
            {
                if (this.selectedPiece == null)
                {
                    // Select the piece
                    SelectPiece(this.selectionX, this.selectionY);
                }
                else
                {
                    // // try to move the piece
                    MovePiece(this.selectionX, this.selectionY);
                }
            }
        }
    }

    private void SelectPiece(int x, int y)
    {
        if (this.Pieces[x, y] == null)
        {
            return;
        }

        if (this.Pieces[x, y].IsHuman != IsHumanTurn)
        {
            return;
        }

        this.AllowedMoves = this.Pieces[x, y].PossibleMoves();
        this.selectedPiece = this.Pieces[x, y];
        BoardHighlights.Instance.HighlightAllowedMoves(this.AllowedMoves);
    }

    private void MovePiece(int x, int y)
    {
        if (this.AllowedMoves[x, y])
        {
            this.Pieces[this.selectedPiece.CurrentX, this.selectedPiece.CurrentY] = null;
            this.selectedPiece.transform.position = this.GetTileCenter(x, y);
            this.selectedPiece.SetPosition(x, y);
            this.Pieces[x, y] = this.selectedPiece;
            this.IsHumanTurn = !this.IsHumanTurn;
        }

        BoardHighlights.Instance.HideHighlights();
        this.selectedPiece = null;
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

    private void SpawnPiece(int index, int x, int y, Quaternion orientation)
    {
        GameObject instance = Instantiate(piecePrefabs[index], this.GetTileCenter(x, y), orientation);
        instance.transform.SetParent(this.transform);
        this.Pieces[x, y] = instance.GetComponent<Piece>();
        this.Pieces[x, y].SetPosition(x, y);
        this.activePieces.Add(instance);
    }

    // TODO: change this hardcoded configuration by using some kind of battle configuration. Scriptable objects ?
    private void SpawnAllPieces()
    {
        this.activePieces = new List<GameObject>();
        this.Pieces = new Piece[8, 8];

        // Spawn human force

        SpawnPiece(0, 3, 4, Quaternion.identity);
        SpawnPiece(0, 3, 0, Quaternion.identity);
        SpawnPiece(1, 4, 0, Quaternion.identity);
        SpawnPiece(2, 5, 0, Quaternion.identity);

        // Spawn AI force
        SpawnPiece(3, 2, 6, faceCameraOrientation);
        SpawnPiece(3, 5, 6, faceCameraOrientation);
        SpawnPiece(3, 3, 6, faceCameraOrientation);
        SpawnPiece(4, 4, 6, faceCameraOrientation);
        SpawnPiece(5, 4, 7, faceCameraOrientation);

    }

    private Vector3 GetTileCenter(int x, int y)
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
