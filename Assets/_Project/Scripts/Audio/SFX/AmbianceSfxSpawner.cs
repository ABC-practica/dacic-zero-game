using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbianceSfxSpawner : MonoBehaviour
{
    [SerializeField] private List<AudioClip> ambianceSounds = new();
    [SerializeField] private float minimumRange = 1f;
    [SerializeField] private float maximumRange = 2f;
    [SerializeField] private float maximulHeight = 2f;
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] [Range(0f, 1f)] private float chanceToPlay = 0f;
    [SerializeField] private float baseVolume = 1f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float playTryRate = 1f;
    private AudioClip lastPlayed = null;
    private void Start()
    {
        InvokeRepeating("PlayRandom", 0f, playTryRate);
    }

    private void PlayRandom()
    {
        if (Random.Range(0f, 1f) > chanceToPlay)
            return;
        int indexClip = Random.Range(0, ambianceSounds.Count);
        AudioClip clip = ambianceSounds[indexClip];
        if (clip == lastPlayed)
        {
            indexClip += Random.Range(1, ambianceSounds.Count);
            clip = ambianceSounds[indexClip % ambianceSounds.Count];
        }

        float radius = Random.Range(minimumRange, maximumRange);
        float height = Random.Range(0, maximulHeight);
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 soundPosition = new Vector3(
            playerTransform.position.x + Mathf.Sin(angle) * radius,
            playerTransform.position.y + height,
            playerTransform.position.z + Mathf.Cos(angle) * radius
            );
        PlaySound(soundPosition, clip);
    }

    private void PlaySound(Vector3 position, AudioClip sound)
    {
        Debug.Log(sound);
        lastPlayed = sound;
        GameObject tempObj = new GameObject("tempAudio");
        tempObj.transform.position = position;
        AudioSource audioSource = tempObj.AddComponent<AudioSource>();
        audioSource.volume = baseVolume;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.spatialBlend = 1f;
        audioSource.PlayOneShot(sound);
        Destroy(tempObj, sound.length);
    }
}
