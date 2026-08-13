using HP;
using Interaction;
using MBT;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    Dictionary<GameObject, Vector3> SavedPositions = new();
    Dictionary<GameObject, bool> SavedStates = new();

    [SerializeField] Transform Player;
    
    public Vector3 savedPosition;
    public Quaternion savedAngle;

    private void OnEnable()
    {
        initializeObjects();
    }

    void initializeObjects() 
    {
        GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<NavMeshAgent>())
            {
                SavedPositions[obj] = obj.transform.position;
            }
        }
    }

    public void SaveCheckPoint(Transform checkpoint)
    {
        savedPosition = checkpoint.position;
        savedAngle = checkpoint.rotation;
        
        foreach (GameObject obj in SavedPositions.Keys.ToList())
        {
            SavedPositions[obj] = obj.transform.position;
            SavedStates[obj] = obj.activeInHierarchy;
        }
    }

    public void loadCheckPoint()
    {
        var PlayerHPComponent = Player.GetComponent<HPComponent>();
        PlayerHPComponent.CurrentHealth = PlayerHPComponent.MaxHealth;

        foreach(GameObject obj in SavedPositions.Keys)
        {
            obj.transform.position = SavedPositions[obj];
            obj.SetActive(SavedStates[obj]);
        }
    }

}
