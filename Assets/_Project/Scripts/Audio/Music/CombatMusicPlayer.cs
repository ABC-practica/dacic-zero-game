using CombatSystem;
using EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public struct PauseSound : IEvent { }
public struct UnpauseSound : IEvent { }
public class CombatMusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> combatTracks = new();
    [SerializeField] private float baseMusicVolume = 1f;
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] [Min(0f)] private float fadeInTime = 2f;
    [SerializeField] [Min(0f)] private float fadeOutTime = 2f;
    [SerializeField] [Min(0f)] private float minimumMusicPlayTime = 5f;

    private AudioSource musicSource;
    private Coroutine fadeInCoroutine = null;
    private Coroutine fadeOutCoroutine = null;
    private Coroutine stopMusicCoroutine = null;
    private float minOutsideCombatTime = 10f;
    private float exitCombatTime = 0f;

    private void Awake()
    {
        if (combatTracks.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        GameObject tempObj = new GameObject("CombatMusicPlayer");
        tempObj.transform.SetParent(transform);
        musicSource = tempObj.AddComponent<AudioSource>();
        InitAudioSource();
    }

    private void InitAudioSource()
    {
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.outputAudioMixerGroup = mixerGroup;
        musicSource.clip = combatTracks[Random.Range(0, combatTracks.Count)];
    }

    private void OnEnable()
    {
        EventBus<PlayerEnterCombat>.AddActions(0, actionNoArgs: PlayMusic);
        EventBus<PlayerExitCombat>.AddActions(0, actionNoArgs: StopMusic);
    }

    private void OnDisable()
    {
        EventBus<PlayerEnterCombat>.RemoveActions(0, actionNoArgs: PlayMusic);
        EventBus<PlayerExitCombat>.RemoveActions(0, actionNoArgs: StopMusic);
    }

    private void PlayMusic()
    {
        if (Time.time - exitCombatTime >= minOutsideCombatTime)
            musicSource.clip = combatTracks[Random.Range(0, combatTracks.Count)];

        if (stopMusicCoroutine != null)
        {
            StopCoroutine(stopMusicCoroutine);
            stopMusicCoroutine = null;
        }

        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        if (!musicSource.isPlaying)
            musicSource.Play();

        EventBus<PauseSound>.Raise(gameObject.GetInstanceID(), new PauseSound());
        fadeInCoroutine = StartCoroutine(FadeSoundIn());
    }

    private void StopMusic()
    {
        stopMusicCoroutine = StartCoroutine(StopMusicCorutine());
    }

    private IEnumerator StopMusicCorutine()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, minimumMusicPlayTime - musicSource.time)
            );
        
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }

        EventBus<UnpauseSound>.Raise(gameObject.GetInstanceID(), new UnpauseSound());
        fadeOutCoroutine = StartCoroutine(FadeSoundOut());
    }

    private IEnumerator FadeSoundIn()
    {
        float currentProgress = Mathf.Min(1f, musicSource.volume / baseMusicVolume);
        float timeElapsed = fadeInTime * currentProgress;
        while (timeElapsed < fadeInTime)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / fadeInTime;
            musicSource.volume = Mathf.Lerp(0f, baseMusicVolume, progress);
            yield return null;
        }
        musicSource.volume = baseMusicVolume;
        fadeInCoroutine = null;
    }

    private IEnumerator FadeSoundOut()
    {
        float currentProgress = Mathf.Min(1f, musicSource.volume / baseMusicVolume);
        float timeElapsed = fadeOutTime * currentProgress;
        while (timeElapsed > 0f)
        {
            timeElapsed -= Time.deltaTime;
            float progress = timeElapsed / fadeOutTime;
            musicSource.volume = Mathf.Lerp(0f, baseMusicVolume, progress);
            yield return null;
        }
        musicSource.volume = 0f;
        musicSource.Stop();
        exitCombatTime = Time.time;
        fadeOutCoroutine = null;
    }
}
