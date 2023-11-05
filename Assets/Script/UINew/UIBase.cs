using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class UIBase : MonoBehaviour
{
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        AudioSource audioSource = AudioSystem.instance.clickSound;

        var buttons = root.Query<Button>().ToList();
        foreach(var btn in buttons)
        {
            btn.clicked += () =>
            {
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter)) return;
                audioSource.Play();
            };
        }
    }


}
