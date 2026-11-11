using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSounds : MonoBehaviour
{
    public AudioClip enemyDefeatedSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayEnemyDefeated() => Play(enemyDefeatedSound);
}
