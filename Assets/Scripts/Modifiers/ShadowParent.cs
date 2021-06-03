using UnityEngine;

public class ShadowParent : MonoBehaviour
{
    [SerializeField] GameObject parentObject = null;
    [SerializeField] LayerMask layerMask = 0;

    Vector3 groundPosition;
    Vector3 groundNormal;

    private void Start()
    {
        if (parentObject == null)
            Debug.LogWarning("ShadowParent object " + gameObject.name + " has no parent object assigned.");
    }

    private void Update()
    {
        if (parentObject == null)
            return;

        if (Physics.Raycast(parentObject.transform.position, Vector3.down, out RaycastHit hit, 5000, layerMask, QueryTriggerInteraction.Ignore))
        {
            groundPosition = hit.point;
            groundNormal = hit.normal;
        }

        transform.position = groundPosition + new Vector3(0, 0.01f, 0);
        transform.up = groundNormal;
    }

    private void OnDrawGizmosSelected()
    {
        if (parentObject == null)
            return;

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, groundPosition);
    }
}