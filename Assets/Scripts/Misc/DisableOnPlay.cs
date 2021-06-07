using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnPlay : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();

        gameObject.SetActive(false);
    }
}