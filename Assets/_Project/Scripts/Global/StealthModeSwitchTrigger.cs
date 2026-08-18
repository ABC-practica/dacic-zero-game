using PlayerController;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class StealthModeSwitchTrigger : MonoBehaviour
{
    [SerializeField] bool isEnabled;

    public void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        var stealthComponent = other.GetComponent<PlayerStealthController>();
        if(stealthComponent != null)
        {
            stealthComponent.SetStealthMode(isEnabled);
        }
    }
}
