using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Completed;

public class PathFinder
{
    

    class Node
    {

        public int x;
        public int y;
        public float G;
        public float H;
        public float F;
        public Node parent;
        public Cell cell;
        public Node(int x, int y, float G, float F, float H, Node parent, Cell c)
        {
            this.x = x;
            this.y = y;
            this.G = G;
            this.H = H;
            this.F = F;
            this.parent = parent;
            this.cell = c;
        }

    }


    List<Node> openList;
    List<Node> closeList;
    List<Node> neighbours;
    List<Node> finalPath;

    Node start;
    Node end;
    
    Cell[,] map;
    int mapWidth;
    int mapHeight;

    public PathFinder()
    {
        openList = new List<Node>();
        closeList = new List<Node>();
        neighbours = new List<Node>();
        finalPath = new List<Node>();
    }

    public void FindPath(Cell startCell, Cell goalCell, Cell[,] map, bool targetCellMustBeFree)
    {

        this.map = map;
        this.mapWidth = map.GetLength(0);
        this.mapHeight = map.GetLength(1);
        //set the start and end nodes to the cells  in parameter
        start = new Node((int)startCell.coordinates.x, (int)startCell.coordinates.y, 0, 0, 0, null, startCell);
        end = new Node((int)goalCell.coordinates.x, (int)goalCell.coordinates.y, 0, 0, 0, null, goalCell);

        openList.Add(start);
        bool keepSearching = true;
        bool pathExists = true;

        while ((keepSearching) && (pathExists))
        {
            Node currentNode = ExtractBestNodeFromOpenList();
            if (currentNode == null)
            {
                pathExists = false;
                break;
            }
            closeList.Add(currentNode);
            if (NodeIsGoal(currentNode))
                keepSearching = false;
            else {
            
                    FindValidFourNeighbours(currentNode);
               

                foreach (Node neighbour in neighbours)
                {
                    if (FindInCloseList(neighbour) != null)
                        continue;
                    Node inOpenList = FindInOpenList(neighbour);
                    if (inOpenList == null)
                    {
                        openList.Add(neighbour);
                    }
                    else {
                        if (neighbour.G < inOpenList.G)
                        {
                            inOpenList.G = neighbour.G;
                            inOpenList.F = inOpenList.G + inOpenList.H;
                            inOpenList.parent = currentNode;
                        }
                    }
                }
            }
        }

        if (pathExists)
        {
            Node n = FindInCloseList(end);
            while (n != null)
            {
                finalPath.Add(n);
                n = n.parent;
            }
        }
    }

 

    public List<Cell> CellsFromPath()
    {
        List<Cell> path = new List<Cell>();
        foreach (Node n in finalPath)
        {
            path.Add(n.cell);
        }

        if (path.Count != 0)
        {
            path.Reverse();
            path.RemoveAt(0);
        }
        return path;
    }

    Node ExtractBestNodeFromOpenList()
    {
        float minF = float.MaxValue;
        Node bestOne = null;
        foreach (Node n in openList)
        {
            if (n.F < minF)
            {
                minF = n.F;
                bestOne = n;
            }
        }
        if (bestOne != null)
            openList.Remove(bestOne);
        return bestOne;
    }

    bool NodeIsGoal(Node n)
    {
        return ((n.x == end.x) && (n.y == end.y));
    }

    void FindValidFourNeighbours(Node n)
    {
        neighbours.Clear();
     
        if ((n.y - 1 >= 0) && ((map[n.x, n.y - 1].IsWalkable())))
        {
            Node vn = PrepareNewNodeFrom(n, 0, -1);
            neighbours.Add(vn);
        
        }
        if ((n.y + 1 <= mapHeight - 1) && ((map[n.x, n.y + 1].IsWalkable())) )
        {
            Node vn = PrepareNewNodeFrom(n, 0, +1);
            neighbours.Add(vn);
    
        }
        if ((n.x - 1 >= 0) && ((map[n.x - 1, n.y].IsWalkable())) )
        {
           
            Node vn = PrepareNewNodeFrom(n, -1, 0);
            neighbours.Add(vn);
        }
        if ((n.x + 1 <= mapWidth - 1) && ((map[n.x + 1, n.y].IsWalkable())) )
        {
           
            Node vn = PrepareNewNodeFrom(n, 1, 0);
            neighbours.Add(vn);
        }
    }

    Node PrepareNewNodeFrom(Node n, int x, int y)
    {
        Node newNode = new Node(n.x + x, n.y + y, 0, 0, 0, n, map[n.x + x, n.y + y]);
        newNode.G = n.G; 
        newNode.H = Heuristic(newNode);
        newNode.F = newNode.G + newNode.H;
        newNode.parent = n;
        return newNode;
    }

    float Heuristic(Node n)
    {
        // Manhattan distance
        
        float dx = Mathf.Abs(n.x - end.x);
        float dy = Mathf.Abs(n.y - end.y);
        return dx + dy;
        

    }


    Node FindInCloseList(Node n)
    {
        foreach (Node nn in closeList)
        {
            if ((nn.x == n.x) && (nn.y == n.y))
                return nn;
        }
        return null;
    }

    Node FindInOpenList(Node n)
    {
        foreach (Node nn in openList)
        {
            if ((nn.x == n.x) && (nn.y == n.y))
                return nn;
        }
        return null;
    }
}