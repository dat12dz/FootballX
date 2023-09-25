using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEye : MonoBehaviour
{
    
    void Start()
    {
        
        if (isLocalPlayer())
        // Set default player eyes tag to be camera
        tag = "MainCamera";
    }
    public bool isLocalPlayer()
    {
        return true;
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
