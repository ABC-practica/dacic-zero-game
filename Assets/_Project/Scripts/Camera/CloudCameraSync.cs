using UnityEngine;

public class CloudCameraSync : MonoBehaviour
{
    public Material cloudMaterial;
    public Camera targetCamera;

    void Update()
    {
        if (cloudMaterial == null || targetCamera == null) return;

        cloudMaterial.SetVector("_CamPosition", targetCamera.transform.position);
        cloudMaterial.SetVector("_CamRotation", targetCamera.transform.forward);
    }
}