using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{
    public BoxCollider buildingArea;
    public float sizeOfHorizontal_Squares = .3f;
    public float sizeOfVertical_Sides = .5f;
    public GridNode[,,] levelGrid;
    public List<GridNode> registerPostion = new List<GridNode>();
    public List<GridNode> customerRegisterPosition = new List<GridNode>();
    public List<GridNode> doorPosition = new List<GridNode>();
    public List<GridNode> pickupPositon = new List<GridNode>();
    public List<GridNode> pickupPositionEmployee= new List<GridNode>();
    public List<GridNode> trashCanPositions = new List<GridNode>();
    public List<GridNode> waitingAreaPositions = new List<GridNode>();
    public GameObject cube;
    public bool calculate = false;
    public float searchSize = .1f;
    public Node spawnCubeHere;
    public bool spawnCube;
    public bool clearChildren;
    
    public Transform gridCubeHolder;
    [System.Serializable]
    public class GridNode
    {
        public Vector3 position;
        public bool isWalkable;
        public int dirtiness;
        public GridOccupant occupant;
        public bool isEmptySpace;
        public Vector3Int gridIndex_Location;
        public GridNode(Vector3 p, bool iW, GridOccupant o, Vector3Int index)
        {
            position = p;
            isWalkable = iW;
            occupant = o;
            dirtiness = 0;
            gridIndex_Location = index;
        }
        public void CopyFrom(GridNode n)
        {
            position = n.position;
            isWalkable = n.isWalkable;
            dirtiness = n.dirtiness;
            occupant = n.occupant;
            gridIndex_Location = n.gridIndex_Location;
        }
        public Vector3Int getGround()
        {
            return new Vector3Int(gridIndex_Location.x, 0, gridIndex_Location.z);
        }
        public bool PositionWithinNode(Vector3 pos, float nodeWidth)
        {
            float xLower = position.x - nodeWidth /2  , xUpper = position.x + nodeWidth/2;
            float zLower = position.z - nodeWidth / 2, zUpper = position.z + nodeWidth / 2;
            if(pos.x > xLower && pos.x < xUpper)
            {
                if (pos.z > zLower && pos.z < zUpper)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class GridOccupant 
    {
        public GameObject occupantObject;
        public Vector2 xBound, yBound, zBound;
        public void GetOccupantBounds(out Vector2 tempX, out Vector2 tempY, out Vector2 tempZ)
        {
            tempX = xBound;
            tempY = yBound;
            tempZ = zBound;
        }
    }



    [System.Serializable]
    public struct Node
    {
        public bool enabled;
        public int x, y, z;
        public Node(int xx, int yy, int zz)
        {
            x = xx;
            y = yy;
            z = zz;
            enabled = true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public Node GetGroundNode()
        {
            return new Node(x, 0, z);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x: " + x + ", y: " + y + ", z: " + z;
        }

        public static bool operator==(Node n, Node m)
        {
            bool equal = false;
            if(n.x == m.x && n.y == m.y && n.z == m.z)
            {
                equal = true;
            }
            return equal;
        }
        public static bool operator !=(Node n, Node m)
        {
            bool equal = false;
            if (n.x == m.x && n.y == m.y && n.z == m.z)
            {
                equal = true;
            }
            return !equal;
        }
        public void CopyFrom(Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }


    }
    private void Start()
    {
        CalculateGridWithBuildingArea();
    }
    private void Update()
    {
        if(calculate)
        {
            CalculateGridWithBuildingArea();
            calculate = false;
        }
        if(spawnCube)
        {
            SpawnCube();
            spawnCube = false;
        }
        if(clearChildren)
        {
            foreach(Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
            clearChildren = false;
        }
    }
    void ClearNodeLists()
    {
        registerPostion.Clear();
        customerRegisterPosition.Clear();
        doorPosition.Clear();
        pickupPositon.Clear();
        pickupPositionEmployee.Clear();
        trashCanPositions.Clear();
        waitingAreaPositions.Clear();
    }
    public GameObject SpawnCube()
    {
        if (levelGrid == null)
        {
            Debug.Log("Level grid is null or spawnCubeHere is null");
            return null;
        }
        Vector3 pos = levelGrid[spawnCubeHere.x, spawnCubeHere.y, spawnCubeHere.z].position;
        GameObject go =Instantiate(cube);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(sizeOfHorizontal_Squares, sizeOfVertical_Sides, sizeOfHorizontal_Squares);
        go.transform.SetParent(transform);
        return go;
    }
    private void Reset()
    {
        CalculateGridWithBuildingArea();
    }
    public void CalculateGridWithBuildingArea()
    {
        ClearNodeLists();
        float xSize = 0, ySize = 0, zSize = 0;
        xSize = buildingArea.bounds.max.x - buildingArea.bounds.min.x;
        ySize = buildingArea.bounds.max.y - buildingArea.bounds.min.y;
        zSize = buildingArea.bounds.max.z - buildingArea.bounds.min.z;

        if(xSize % sizeOfHorizontal_Squares != 0)
        {
            xSize += .5f;
            xSize = (int)xSize;
            xSize += sizeOfHorizontal_Squares;
            Debug.Log("xSize needed increasing");
        }
        if (ySize % sizeOfVertical_Sides != 0)
        {
            ySize += .5f;
            ySize = (int)ySize;
            ySize += sizeOfVertical_Sides;
            Debug.Log("ySize needed increasing");
        }
        if (zSize % sizeOfHorizontal_Squares != 0)
        {
            zSize += .5f;
            zSize = (int)zSize;
            zSize += sizeOfHorizontal_Squares;
            Debug.Log("zSize needed increasing");
        }
        int xSquares, ySquares, zSquares;
        //Setting number of grid squares within our bounds
        xSquares = (int)(xSize / sizeOfHorizontal_Squares);
        zSquares = (int)(zSize / sizeOfHorizontal_Squares);
        ySquares = (int)(ySize / sizeOfVertical_Sides);

        //Creating new grid
        levelGrid = new GridNode[xSquares,ySquares,zSquares];
        ClearNodeLists();
        //Cycling through grid, spawning a gameObject at each spot underneath level manager, and adding it to the grid
        int count = 0;
        for(int i = 0; i < ySquares; i++)
        {
            for(int j = 0; j < xSquares; j++)
            {
                for(int k = 0; k < zSquares; k++)
                {
                    float xPos, yPos, zPos;
                    xPos = buildingArea.bounds.max.x - (j * sizeOfHorizontal_Squares);
                    yPos = buildingArea.bounds.min.y + (i * sizeOfVertical_Sides);
                    zPos = buildingArea.bounds.max.z - (k * sizeOfHorizontal_Squares);
                    
                    Vector3 pos = new Vector3(xPos, yPos, zPos);
                    levelGrid[j, i, k] = new GridNode(pos, true, null, new Vector3Int(j, i, k));
                    GridNode currentNode = levelGrid[j, i, k];
                    currentNode.isEmptySpace = true;
                    Collider[] colliders = Physics.OverlapSphere(pos, searchSize);
                    foreach(Collider c in colliders)
                    {
                        if(c.CompareTag("Register"))
                        {
                            registerPostion.Add(currentNode);
                            customerRegisterPosition.Add(levelGrid[j, i, k + 1]);
                            break;
                        }
                        else if(c.CompareTag("Door"))
                        {
                            GridNode groundNode = levelGrid[currentNode.getGround().x, currentNode.getGround().y, currentNode.getGround().z];
                            doorPosition.Add(groundNode);
                        }
                        else if(c.CompareTag("Waiting Area"))
                        {
                            GridNode groundNode = levelGrid[currentNode.getGround().x, currentNode.getGround().y, currentNode.getGround().z];
                            waitingAreaPositions.Add(groundNode);
                        }
                        else if(c.CompareTag("Pickup"))
                        {
                            GridNode groundNode = levelGrid[currentNode.getGround().x, currentNode.getGround().y, currentNode.getGround().z];
                            pickupPositon.Add(groundNode);

                        }
                    }
                    count++;
                }
            }
        }
        TrimWaitingArea();

    }

    void TrimWaitingArea()
    {
        List<GridNode> finalNodes = new List<GridNode>();
        foreach(GridNode n in waitingAreaPositions)
        {
            bool add = true;
            foreach(GridNode m in finalNodes)
            {
                if (m.position != n.position)
                    add = true;
                else
                {
                    add = false;
                    break;
                }
            }
            if (add)
                finalNodes.Add(n);
        }
        waitingAreaPositions = finalNodes;
    }

    public Vector3Int FindGridNodeOfLocation(Vector3 pos)
    {
        float xLower = levelGrid[0, 0, 0].position.x + (sizeOfHorizontal_Squares / 2), xUpper = levelGrid[levelGrid.GetLength(0) - 1, 0, 0].position.x;
        float zLower = levelGrid[0, 0, 0].position.z, zUpper = levelGrid[0, 0, levelGrid.GetLength(2) - 1].position.z;
        int xIndex = -1, yIndex = 0, zIndex = -1;
        
        for(int i = 0; i < levelGrid.GetLength(0); i++)
        {
            for(int j = 0; j < levelGrid.GetLength(2); j++)
            {
                if(levelGrid[i,0,j].PositionWithinNode(pos, sizeOfHorizontal_Squares))
                {
                    xIndex = i;
                    zIndex = j;
                    break;
                }
            }
            if (xIndex > -1)
                break;
        }
        

        if (xIndex > -1 && zIndex > -1)
        {
            return new Vector3Int(xIndex, yIndex, zIndex);
        }
        else
        {
            if (xIndex > -1)
                Debug.Log(pos.z + " z did not fit within " + zLower + ", " + zUpper);
            else if (zIndex > -1)
                Debug.Log(pos.x + " x did not fit within " + xLower + ", " + xUpper);
            else { }
                //Debug.Log(pos.x + " z x did not fit within " + xLower + ", " + xUpper + " and " + pos.z + " did not fit within " + zLower + ", " + zUpper);


            return new Vector3Int(-1, yIndex, -1);
        }
    }
}
