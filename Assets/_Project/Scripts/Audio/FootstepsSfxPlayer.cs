using EventBus;
using Newtonsoft.Json;
using Surface;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootstepsSfxPlayer : MonoBehaviour
{
    [System.Serializable]
    private struct FootstepSound
    {
        public SurfaceType Surface;
        public AudioClip AssociatedAudio;
    }

    [SerializeField] private List<FootstepSound> footstepSounds = new List<FootstepSound>();
    [SerializeField] private float baseFootstepPitch = 1f;
    [SerializeField] private float baseMovementSpeed = 7f;
    [SerializeField] private float minimumMovementSpeed = 0.1f;
    [SerializeField] private AudioSource audioSource;

    private Dictionary<SurfaceType, AudioClip> soundBoard;
    private float baseVolume;

    private void Awake()
    {
        audioSource.loop = true;
        soundBoard = footstepSounds.ToDictionary(
            x => x.Surface,
            x => x.AssociatedAudio
            );
        baseVolume = audioSource.volume;
    }

    private void OnEnable()
    {
        EventBus<EntityMoved>.AddActions(gameObject.GetInstanceID(), HandleMoveEvent);
    }

    private void OnDisable()
    {
        EventBus<EntityMoved>.RemoveActions(gameObject.GetInstanceID(), HandleMoveEvent);
    }

    private void HandleMoveEvent(EntityMoved moveEvent)
    {
        float speed = new Vector3(moveEvent.Velocity.x, 0f, moveEvent.Velocity.z).magnitude;
        if (!moveEvent.IsGrounded || speed < minimumMovementSpeed)
        {
            audioSource.Stop();
            return;
        }
        float volume = baseVolume;
        if (speed < 5)
        {
            speed += 2;
            volume /= 2;
        }
        if (!soundBoard.TryGetValue(moveEvent.Surface, out AudioClip sound))
            return;
        PlaySound(sound, speed / baseMovementSpeed * baseFootstepPitch, volume);
    }

    private void PlaySound(AudioClip sound, float pitch, float volume)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        if (audioSource.clip != sound || !audioSource.isPlaying)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }
    }
}
