using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SoundPlayer
{

    public AudioSource[] SoundTrack;
    public void PlayRandomSound()
    {

        SoundTrack[UnityEngine.Random.Range(0, SoundTrack.Length)].Play();
    }
}

