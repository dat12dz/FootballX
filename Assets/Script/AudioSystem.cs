using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem instance;
    public AudioSource[] audioSource;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
}
