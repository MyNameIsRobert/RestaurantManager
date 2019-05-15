using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestarauntEditor : MonoBehaviour
{
    public Camera editorCam;
    public GameObject currentHeldObject;
    public LevelManager.GridNode currentGrid;
    public LevelManager lmanager;
    public Camera playerCam;
    public PlayerController playerController;
    public bool editing;
    Vector3Int lastIndexes;
    GameObject selectedCube;
    public GameObject emptyOccupant;
    public Transform emptyOccupant_GarbageObject;
    public GameObject selectionCube;
    public Transform selectedNode_GarbageObject;
    public int currentHeight = 0;
    public int previousHeight = -1;
    bool clickAndDrag_Running = false;
    Vector3Int currentIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void ClearGuideCubes()
    {
        foreach(Transform child in emptyOccupant_GarbageObject)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < lmanager.levelGrid.GetLength(0); i++)
        {
            for (int j = 0; j < lmanager.levelGrid.GetLength(2); j++)
            {
                LevelManager.GridNode node = lmanager.levelGrid[i, currentHeight, j];
                node.occupant = null;
            }
        }
    }
    void SpawnCubes()
    {
        for(int i = 0; i < lmanager.levelGrid.GetLength(0); i++)
        {
            for(int j = 0; j < lmanager.levelGrid.GetLength(2); j++)
            {
                LevelManager.GridNode node = lmanager.levelGrid[i, currentHeight, j];
                if (node.isEmptySpace)
                {
                    LevelManager.GridOccupant occupant = new LevelManager.GridOccupant();
                    occupant.occupantObject = Instantiate(emptyOccupant);
                    occupant.xBound = new Vector2(0, 0);
                    occupant.yBound = new Vector2(0, 0);
                    occupant.zBound = new Vector2(0, 0);

                    node.occupant = occupant;
                    occupant.occupantObject.transform.position = node.position;
                    occupant.occupantObject.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares * .95f, lmanager.sizeOfVertical_Sides * .95f, lmanager.sizeOfHorizontal_Squares *.95f);
                    occupant.occupantObject.transform.SetParent(emptyOccupant_GarbageObject);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!editing)
            {
                playerCam.enabled = false;
                editorCam.enabled = true;
                editing = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.enabled = false;
            }
            else
            {
                playerCam.enabled = true;
                editorCam.enabled = false;
                editing = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.enabled = true;
            }
        }
        if (!editing)
        {
            Debug.Log("Not editing!");
            return;
        }
        if(currentHeight != previousHeight)
        {
            ClearGuideCubes();
            SpawnCubes();
            previousHeight = currentHeight;
        }
        Vector3 mousePos = editorCam.ScreenToWorldPoint(Input.mousePosition);

        currentIndex = lmanager.FindGridNodeOfLocation(mousePos);

        if(currentIndex.x != -1)
        {
            if(Input.GetButtonDown("Use"))
            {
                if (!clickAndDrag_Running) 
                    StartCoroutine(ClickAndDrag(currentIndex));
            }
            if (lastIndexes != currentIndex)
            {
                if (selectedCube)
                    Destroy(selectedCube);
            }
            currentGrid = lmanager.levelGrid[currentIndex.x, currentIndex.y, currentIndex.z];
            if (!selectedCube)
            {
                selectedCube = SpawnSelectionCubes(currentIndex, currentIndex);
            }
            lastIndexes = currentIndex;
        }
    }

    IEnumerator ClickAndDrag(Vector3Int startingIndex)
    {
        clickAndDrag_Running = true;
        Vector3Int lastIndex = startingIndex;
        while(!Input.GetButtonUp("Use"))
        {
            if(currentIndex != lastIndex)
            {
                ClearSelectionCubes();
                SpawnSelectionCubes(startingIndex, currentIndex);
                lastIndex = currentIndex;
            }
            yield return null;
        }
        ClearSelectionCubes();
        clickAndDrag_Running = false;
    }
    void ClearSelectionCubes()
    {
        foreach(Transform child in selectedNode_GarbageObject)
        {
            Destroy(child.gameObject);
        }
    }
    GameObject SpawnSelectionCubes(Vector3Int start, Vector3Int end)
    {
        if (start == end)
        {
            GameObject go = Instantiate(selectionCube, selectedNode_GarbageObject);
            go.transform.position = lmanager.levelGrid[start.x, currentHeight, start.z].position;
            go.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares, lmanager.sizeOfVertical_Sides, lmanager.sizeOfHorizontal_Squares);
            return go;
        }
        else
        {
            if (start.x > end.x)
            {
                if (start.z > end.z)
                {
                    for(int i = end.x; i <= start.x; i++)
                    {
                        for(int j = end.z; j <= start.z; j++)
                        {
                            GameObject go = Instantiate(selectionCube, selectedNode_GarbageObject);
                            go.transform.position = lmanager.levelGrid[i, currentHeight, j].position;
                            go.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares, lmanager.sizeOfVertical_Sides, lmanager.sizeOfHorizontal_Squares);
                        }
                    }
                }
                else
                {
                    for (int i = end.x; i <= start.x; i++)
                    {
                        for(int j = start.z; j <= end.z; j++)
                        {
                            GameObject go = Instantiate(selectionCube, selectedNode_GarbageObject);
                            go.transform.position = lmanager.levelGrid[i, currentHeight, j].position;
                            go.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares, lmanager.sizeOfVertical_Sides, lmanager.sizeOfHorizontal_Squares);
                        }
                    }
                }
            }
            else
            {
                if (start.z > end.z)
                {
                    for (int i = start.x; i <= end.x; i++)
                    {
                        for (int j = end.z; j <= start.z; j++)
                        {
                            GameObject go = Instantiate(selectionCube, selectedNode_GarbageObject);
                            go.transform.position = lmanager.levelGrid[i, currentHeight, j].position;
                            go.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares, lmanager.sizeOfVertical_Sides, lmanager.sizeOfHorizontal_Squares);
                        }
                    }
                }
                else
                {
                    for (int i = start.x; i <= end.x; i++)
                    {
                        for (int j = start.z; j <= end.z; j++)
                        {
                            GameObject go = Instantiate(selectionCube, selectedNode_GarbageObject);
                            go.transform.position = lmanager.levelGrid[i, currentHeight, j].position;
                            go.transform.localScale = new Vector3(lmanager.sizeOfHorizontal_Squares, lmanager.sizeOfVertical_Sides, lmanager.sizeOfHorizontal_Squares);
                        }
                    }
                }
            }
            return null;
        }

    }
}
