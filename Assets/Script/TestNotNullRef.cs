using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNotNullRef : MonoBehaviour
{
 GameObject test = null;
    [ContextMenu("Set GameObject")]
    public void SetupgameObject()
    {
        test = GameObject.Find("Test");
    }
    async void Start()
    {
        await VariableHelper.WaitForVariableNotNullAsync(() => test, 50000);
        Debug.Log("hello world");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
