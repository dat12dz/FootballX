using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentUI : MonoBehaviour
{
    public static PermanentUI instance;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
