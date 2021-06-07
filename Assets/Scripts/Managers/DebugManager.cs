using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] bool hideDebugStuff = false;

    private void Start()
    {
        if (hideDebugStuff)
        {
            // disable objects tagged "Debug"
            GameObject[] debugObjects = GameObject.FindGameObjectsWithTag("Debug");

            foreach (GameObject o in debugObjects)
                o.SetActive(false);

            // disable mesh renderers on objects tagged "Trigger"
            GameObject[] triggers = GameObject.FindGameObjectsWithTag("Trigger");

            foreach (GameObject t in triggers)
                t.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}