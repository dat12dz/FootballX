using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameInfo : MonoBehaviour
{
    public static StartGameInfo instance;
    public string PlayerName;
    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
