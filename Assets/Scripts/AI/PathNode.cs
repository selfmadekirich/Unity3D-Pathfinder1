using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : Priority_Queue.FastPriorityQueueNode
{
    public bool walkable;            //  Проходим узел сетки
    public Vector3 worldPosition;    //  Координаты узла
    public GameObject body;         //  Тело узла
    private PathNode parent = null;  //  Родительский узел
    private float distance = float.PositiveInfinity;
    public Vector2Int gridIndex;
    public PathNode ParentNode
    {
        get => parent;
        set => SetParent(value);
    }

    private void SetParent(PathNode parentNode)
    {
        parent = parentNode;
        if (parent != null)
            distance = parent.distance + PathNode.Dist(parent, this);
        else
            distance = float.PositiveInfinity;
    }

    public float Distance
    {
        get => distance;
        set => distance = value;    //   Плохо!
    }

    public PathNode(GameObject prefab, bool walkable, Vector3 position)
    {
        this.walkable = walkable;
        worldPosition = position;
        SetParent(null);
        body = GameObject.Instantiate(prefab, position, Quaternion.identity);
    }

    public static float Dist(PathNode source, PathNode dest)
    {
        float baseDist = Vector3.Distance(source.worldPosition, dest.worldPosition);
        if (dest.worldPosition.y > source.worldPosition.y)
            return baseDist + 10 * (dest.worldPosition.y - source.worldPosition.y);
        else
            return baseDist + 5*(source.worldPosition.y - dest.worldPosition.y);
    }

    public void Illuminate()
    {
        body.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void Fade()
    {
        body.GetComponent<Renderer>().material.color = Color.gray;
    }

    public void Highlight()
    {
        body.GetComponent<Renderer>().material.color = Color.red;
    }
}
