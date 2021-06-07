using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeParent : MonoBehaviour
{
    [SerializeField] GameObject parentObject = null;

    [SerializeField] bool matchXPosition = true;
    [SerializeField] bool matchYPosition = true;
    [SerializeField] bool matchZPosition = true;

    [SerializeField] bool matchXRotation = true;
    [SerializeField] bool matchYRotation = true;
    [SerializeField] bool matchZRotation = true;
    
    Vector3 positionOffset = Vector3.zero;

    private void Start()
    {
        positionOffset = transform.position - parentObject.transform.position;
    }

    private void Update()
    {
        if (parentObject != null)
        {
            Vector3 position = new Vector3(
                matchXPosition ? parentObject.transform.position.x : transform.position.x,
                matchYPosition ? parentObject.transform.position.y : transform.position.y,
                matchZPosition ? parentObject.transform.position.z : transform.position.z);

            Vector3 rotation = new Vector3(
                matchXRotation ? parentObject.transform.rotation.eulerAngles.x : transform.rotation.eulerAngles.x,
                matchYRotation ? parentObject.transform.rotation.eulerAngles.y : transform.rotation.eulerAngles.y,
                matchZRotation ? parentObject.transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z);

            transform.position = position + positionOffset;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}