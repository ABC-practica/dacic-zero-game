using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] [Min(0.01f)] private float baseMusicVolume;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private AudioMixerGroup mixerGroup;
    private AudioSource musicSource;
    private Coroutine fadeOutCoroutine = null;
    private Coroutine fadeInCoroutine = null;

    private void Awake()
    {
        GameObject tempMusicPlayer = new GameObject("MusicPlayer");
        tempMusicPlayer.transform.SetParent(transform);
        musicSource = tempMusicPlayer.AddComponent<AudioSource>();
        ConfigAudioSource();
    }

    private void ConfigAudioSource()
    {
        musicSource.volume = 0f;
        musicSource.clip = backgroundMusic;
        musicSource.outputAudioMixerGroup = mixerGroup;
        musicSource.loop = true;
    }

    private void PlayMusic()
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }
        if (!musicSource.isPlaying)
            musicSource.Play();
        fadeInCoroutine = StartCoroutine(FadeSoundIn());
    }

    private void StopMusic()
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
        fadeOutCoroutine = StartCoroutine(FadeSoundOut());
        musicSource.Stop();
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
        fadeOutCoroutine = null;
    }
}
