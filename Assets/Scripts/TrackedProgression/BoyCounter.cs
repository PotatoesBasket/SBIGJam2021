using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyCounter : MonoBehaviour
{
    [SerializeField] GameObject eventToDisable = null;
    [SerializeField] GameObject eventToEnable = null;

    bool boysSatisfied = false;

    private void Update()
    {
        if (boysSatisfied == false)
        {
            if (GameManager.current.boyCounter >= 3)
            {
                boysSatisfied = true;
                eventToDisable.SetActive(false);
                eventToEnable.SetActive(true);
            }
        }
    }
}