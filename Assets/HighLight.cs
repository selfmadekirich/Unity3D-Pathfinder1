using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    private float timeOfStart = -1f;
    private float scaleFactor = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeOfStart > 0)
        {
            if (timeOfStart >= Time.time - 10 && timeOfStart < Time.time)
            {
                //  Увеличивать размер текущего элемента на немного при определённых условиях
                //scaleFactor += 0.04f;
                //transform.localScale = Vector3.one * scaleFactor;
                GetComponent<Renderer>().material.color = Color.red;
            }
            //else GetComponent<Renderer>().material.color = Color.blue;
        }
    }
    
    /// <summary>
    /// Устанавливает время начала роста
    /// </summary>
    /// <param name="tm"></param>
    public void setStartTime(float tm)
    {
        timeOfStart = tm;
    }
}
