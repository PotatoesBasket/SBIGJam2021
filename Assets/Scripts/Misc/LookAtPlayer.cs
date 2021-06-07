using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private void Update()
    {
        transform.forward = new Vector3(
            GameManager.current.player.transform.position.x - transform.position.x,
            0,
            GameManager.current.player.transform.position.z - transform.position.z).normalized;
    }
}