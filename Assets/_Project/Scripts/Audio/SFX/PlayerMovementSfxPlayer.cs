using PlayerController;
using UnityEngine;

public class PlayerMovementSfxPlayer : MonoBehaviour
{

    [SerializeField] private InputReader inputReader;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private float jumpPitch = 1f;
    [SerializeField] private AudioClip crouchSfx;
    [SerializeField] private float crouchPitch = 1f;
    [SerializeField] private float pitchVariation = 0f;
    [SerializeField] private AudioSource audioSource;

    private PlayerMovementController controller;
    
    private void Awake()
    {
        controller = GetComponent<PlayerMovementController>();
    }

    private void OnEnable()
    {
        inputReader.Jump += PlayJumpSound;
        inputReader.Crouch += PlayCrouchSound;
    }

    private void PlayJumpSound()
    {
        if (!controller.Grounded)
            return;
        float randomPitch = jumpPitch + Random.Range(-pitchVariation, pitchVariation);
        PlaySound(jumpSfx, randomPitch);
    }

    private void PlayCrouchSound(bool isHeld)
    {
        if (!isHeld)
            return;
        float randomPitch = crouchPitch + Random.Range(-pitchVariation, pitchVariation);
        PlaySound(crouchSfx, randomPitch);
    }

    private void PlaySound(AudioClip clip, float pitch)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }
}
