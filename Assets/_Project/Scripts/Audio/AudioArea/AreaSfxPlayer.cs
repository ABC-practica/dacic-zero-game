using UnityEngine;

public class AreaSfxPlayer : MonoBehaviour
{
    [SerializeField, Min(0f)] private float volumeInArea = 1f;
    [SerializeField] private Transform playerTransform;
    [SerializeField, Min(0.1f)] private float soundLimitRadius;
    private AudioSource audioSource;
    private EnvironmentalAudioArea area;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        area = GetComponent<EnvironmentalAudioArea>();
        audioSource.loop = true;
        audioSource.volume = volumeInArea;
    }

    private void Start()
    {
        audioSource.Play();
    }

    private void Update()
    {
        float distanceFromPlayer = area.GetDistanceToPolygon(playerTransform.position);
        audioSource.volume = volumeInArea * Mathf.Clamp01(1f - distanceFromPlayer / soundLimitRadius);
    }
}
