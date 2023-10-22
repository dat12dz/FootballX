using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    static GameObject g;
    void Start()
    {
        g = gameObject;
    }

    public static void SetActive(bool active)
    {
        g.SetActive(active);
    }
}
