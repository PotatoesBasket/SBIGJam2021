using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] float speed = 1;

    float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime * speed;

        if (timer >= 1.0f)
        {
            timer = 0;
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}
