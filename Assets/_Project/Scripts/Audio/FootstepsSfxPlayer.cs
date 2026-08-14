using EventBus;
using Newtonsoft.Json;
using Surface;
using System.Collections;
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
    [SerializeField] private float fadeOutDuration = 0.2f;
    private Dictionary<SurfaceType, AudioClip> soundBoard;
    private float baseVolume;
    private Coroutine fadeOutCoroutine = null;

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
            fadeOutCoroutine = StartCoroutine(FadeOutStop());
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

    private IEnumerator FadeOutStop()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = baseVolume;
        fadeOutCoroutine = null;
    }

    private void PlaySound(AudioClip sound, float pitch, float volume)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        audioSource.volume = volume;
        audioSource.pitch = pitch;
        if (audioSource.clip != sound || !audioSource.isPlaying)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }
    }
}
