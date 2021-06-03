using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] Material mat = null;
    [SerializeField] float stretchLimit = 0;

    private void Update()
    {
        float remap = Mathf.InverseLerp(-1, 1, Input.GetAxis("Vertical"));
        float stretch = Mathf.Lerp(stretchLimit, -stretchLimit, remap);

        transform.localScale = new Vector3(1 + stretch, 1 + stretch, 1);

        if (Input.GetButton("Button"))
            mat.SetTextureOffset("_MainTex", new Vector2(0.5f, 0.0f));
        else
            mat.SetTextureOffset("_MainTex", new Vector2(0.0f, 0.0f));
    }
}