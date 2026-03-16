using UnityEngine;
using UnityEngine.SceneManagement;
public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        SceneManager.LoadScene(1);
    }
}
