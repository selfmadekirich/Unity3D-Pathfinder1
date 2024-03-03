using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject nodeModel;      //   Шаблон узла сетки
    [SerializeField] private Terrain landscape;
    [SerializeField] private float gridDelta = 20;
    private int updateAtFrame = 0;

    private PathNode[,] grid = null;

    private void CheckWalkableNodes()
    {
        foreach(PathNode node in grid)
        {
            //node.walkable = true;
            node.walkable = !Physics.CheckSphere(node.body.transform.position, 1) && (node.worldPosition.y < 80);
            if (node.walkable)
                node.Illuminate();
            else
                node.Fade();
        }
    }

    private List<Vector2Int> GetNeighbours(Vector2Int current)
    {
        List<Vector2Int> nodes = new List<Vector2Int>();

        for (int x = current.x - 1; x <= current.x + 1; ++x)
            for (int y = current.y - 1; y <= current.y + 1; ++y)
                if(x >= 0 && x < grid.GetLength(0) && y>=0 && y < grid.GetLength(1) && (x != current.x || y != current.y) && grid[x,y].walkable)
                nodes.Add(new Vector2Int(x, y));
        return nodes;
    }

    void CalculatePath(Vector2Int startNode, Vector2Int finishNode,Action<PathNode,PathNode,PathNode, Priority_Queue.FastPriorityQueue<PathNode>> f)
    {
        CheckWalkableNodes();
        foreach(PathNode node in grid)
            node.ParentNode = null;

        PathNode start = grid[startNode.x, startNode.y];
        start.ParentNode = null;
        start.Distance = 0;
        HashSet<Vector2Int> marked = new HashSet<Vector2Int>();

        // Нужна очередь с приоритетом!!!
        Vector3 terrainSize = landscape.terrainData.bounds.size;
        int sizeX = (int)(terrainSize.x / gridDelta);
        int sizeZ = (int)(terrainSize.z / gridDelta);
        Priority_Queue.FastPriorityQueue<PathNode> pq = new Priority_Queue.FastPriorityQueue<PathNode>(sizeX*sizeZ*2);
        
        //Queue<Vector2Int> nodes = new Queue<Vector2Int>();
        pq.Enqueue(start, 0);

        float startTime = Time.time;

        var target = grid[finishNode.x, finishNode.y];

        while(pq.Count > 0)
        {
            //  Вытаскиваем очередную вершину из списка
            PathNode current = pq.Dequeue();
            current.body.GetComponent<HighLight>().setStartTime(startTime);
            startTime += 0.08f;
            pq.ResetNode(current);

            Vector2Int currentIndex = current.gridIndex;
            if (currentIndex == finishNode) break;
            //PathNode current = grid[currentIndex.x, currentIndex.y];

            if (marked.Contains(current.gridIndex))
                continue;
            marked.Add(current.gridIndex);

            var neighbours = GetNeighbours(currentIndex);
            //Debug.Log("Neighbours : " + neighbours.Count);
            foreach(var node in neighbours)
            {
                PathNode next = grid[node.x, node.y];
                if (!next.walkable)
                    continue;
                f(next, current, target,pq);
            }
        }

        Debug.Log("Path build! HighLighting it!");
        PathNode pathPoint = grid[finishNode.x, finishNode.y];
        while(pathPoint != null)
        {
            pathPoint.Highlight();
            pathPoint = pathPoint.ParentNode;
        }
    }

    // Метод вызывается однократно при создании экземпляра класса
    void Start()
    {
        Vector3 terrainSize = landscape.terrainData.bounds.size;
        int sizeX = (int)(terrainSize.x / gridDelta);
        int sizeZ = (int)(terrainSize.z / gridDelta);

        grid = new PathNode[sizeX, sizeZ];
        for(int x = 0; x<sizeX; ++x)
            for(int z = 0; z<sizeZ; ++z)
            {
                Vector3 position = new Vector3(x * gridDelta, 0, z * gridDelta);
                position.y = landscape.SampleHeight(position) + 25;
                grid[x, z] = new PathNode(nodeModel, true, position);
                //  Каждый узел массива знает своё место
                grid[x, z].gridIndex = new Vector2Int(x, z);
                grid[x, z].Fade();
            }
        CheckWalkableNodes();
    }
    
    void DeikstraFunction(PathNode next,PathNode current, PathNode target, Priority_Queue.FastPriorityQueue<PathNode> pq)
    {
        if (next.Distance > current.Distance + PathNode.Dist(current, next))
        {
            next.ParentNode = current;
            if (pq.Contains(next))
                pq.UpdatePriority(next, next.Distance);
            else
                pq.Enqueue(next, next.Distance);
        }
    }

    void A_star_Euclidean(PathNode next, PathNode current, PathNode target, Priority_Queue.FastPriorityQueue<PathNode> pq)
    {
        var metric = current.Distance + PathNode.Dist(next, target);
        if (next.Distance > current.Distance + PathNode.Dist(next,target))
        {
            next.ParentNode = current;
            if (pq.Contains(next))
                pq.UpdatePriority(next, metric);
            else
                pq.Enqueue(next, metric);
        }
    }


    // Метод вызывается каждый кадр
    void Update()
    {
        if (Time.frameCount < updateAtFrame) return;
        updateAtFrame = Time.frameCount + 5000;
        //Debug.Log("Pathfinding started");

        CalculatePath(new Vector2Int(0, 0), new Vector2Int(grid.GetLength(0) - 3, grid.GetLength(1) - 3), A_star_Euclidean);
    }
}
