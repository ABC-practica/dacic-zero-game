using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint: MonoBehaviour
{
    CheckPointManager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<CheckPointManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("ENTERED CEHKCPOINT");
        manager.SaveCheckPoint(transform);
        gameObject.SetActive(false);
    }
}
