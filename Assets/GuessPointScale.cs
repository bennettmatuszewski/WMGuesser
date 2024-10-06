using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuessPointScale : MonoBehaviour
{
    public Camera camera;
    public float minScale;
    public float maxScale;
    [HideInInspector]public float calculatedScale;
    public bool noUpdate;

    void Update()
    {
        if (noUpdate)
        {
            return;
        }
        float t = Mathf.InverseLerp(1, 35, camera.orthographicSize);
        calculatedScale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(calculatedScale, calculatedScale, 0);
    }
}
