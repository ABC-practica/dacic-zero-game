using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint: MonoBehaviour
{
    [SerializeField] CheckPointManager manager;
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("ENTERED CEHKCPOINT");
        manager.SaveCheckPoint(transform);
        gameObject.SetActive(false);
    }
}
