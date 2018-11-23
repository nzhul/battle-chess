using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{

    public static BoardHighlights Instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.highlights = new List<GameObject>();
    }

    public void HighlightAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < moves.GetLength(0); i++)
        {
            for (int j = 0; j < moves.GetLength(1); j++)
            {
                if (moves[i, j])
                {
                    GameObject go = this.GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + .5f, 0, j + .5f); //TODO: Use TILE_SIZE / 2;
                }
            }
        }
    }

    public void HighlightSelection(int x, int y)
    {
        GameObject go = this.GetHighlightObject();
        go.SetActive(true);
        go.GetComponent<MeshRenderer>().material.color = Color.green;
        go.transform.position = new Vector3(x + .5f, 0, y + .5f); //TODO: Use TILE_SIZE / 2;
    }

    public void HideHighlights()
    {
        foreach (GameObject go in this.highlights)
        {
            go.GetComponent<MeshRenderer>().material.color = Color.white;
            go.SetActive(false);
        }
    }

    private GameObject GetHighlightObject()
    {
        GameObject instance = highlights.Find(x => !x.activeSelf);
        if (instance == null)
        {
            instance = Instantiate(highlightPrefab);
            this.highlights.Add(instance);
        }

        return instance;
    }
}