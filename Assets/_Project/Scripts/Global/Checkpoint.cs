using Unity.VisualScripting;
using UnityEngine;
using Weapons;


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
        var bow = FindFirstObjectByType<Weapons.Bow>();
        if (bow != null) { 
            bow.SetAmmo(10);
        bow.Arrows.text = "10";
        }
    }
}
