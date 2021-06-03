using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] bool hideDebugRenderers = false;

    List<MeshRenderer> debugRenderers = new List<MeshRenderer>();

    private void Start()
    {
        if (hideDebugRenderers)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Trigger");

            foreach (GameObject o in objs)
                debugRenderers.Add(o.GetComponent<MeshRenderer>());

            foreach (MeshRenderer r in debugRenderers)
                r.enabled = false;
        }
    }
}