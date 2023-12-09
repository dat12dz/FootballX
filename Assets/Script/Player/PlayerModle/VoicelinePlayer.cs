using UnityEngine;

[RequireComponent(typeof(AudioSource))]
internal class VoicelinePlayer : MonoBehaviour
{
    AudioSource thisAudioSource;
    [SerializeField] AudioClip pick;
    private void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
    }
    public void PlayPickSound()
    {
        thisAudioSource.clip = pick;
        thisAudioSource.Play();
    }
}

