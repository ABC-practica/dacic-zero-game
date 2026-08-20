using Detection;
using HP;
using PlayerController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    Dictionary<GameObject, Vector3> SavedPositions = new();
    Dictionary<GameObject, bool> SavedStates = new();
    Transform Player;
    
    public Vector3 savedPosition;
    public Quaternion savedAngle;

    private void Start()
    {
        Player = FindFirstObjectByType<PlayerMovementController>().transform;
    }

    private void OnEnable()
    {
        initializeObjects();
    }

    void initializeObjects() 
    {
        GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<DetectionSystem>())
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
        Time.timeScale = 1f;
        var PlayerHPComponent = Player.GetComponent<HPComponent>();
        var PlayerCamerController = Player.GetComponent<CameraController>();

        PlayerHPComponent.CurrentHealth = PlayerHPComponent.MaxHealth;
        PlayerHPComponent.ForceUpdateHealth();

        PlayerCamerController.cameraSpeed = CameraController.CAMERA_SPEED;

        Player.transform.position = savedPosition;
        Player.GetComponent<CameraController>().SetCameraRotation(savedAngle);

        foreach(GameObject obj in SavedPositions.Keys)
        {
            var detectionSystem = obj.GetComponent<DetectionSystem>();
            if(detectionSystem.ClosestTarget != null)
                detectionSystem.ClosestTarget.Awareness = 0;
            obj.transform.position = SavedPositions[obj];
            obj.SetActive(SavedStates[obj]);
        }
    }
}
