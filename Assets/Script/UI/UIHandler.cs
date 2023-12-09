using Assets.Utlis;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CheckNullProperties]

public class UIHandler : WaitForInstaceNotNull<UIHandler>
{

    private void Start()
    {
       instance = this;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
