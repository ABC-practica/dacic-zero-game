using Unity.VisualScripting;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] int InteractionsToOpen = 1;
    int interactionCount = 0;
    public void Interact()
    {
        interactionCount++;
        if(interactionCount>= InteractionsToOpen)
        {
            open();
        }
    }

    private void open()
    {
        gameObject.SetActive(false);
    }
}
