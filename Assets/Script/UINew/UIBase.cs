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
        AudioSource[] audioSource = AudioSystem.instance.audioSource;

        var buttons = root.Query<Button>();

        foreach(var btn in buttons.ToList())
        {
            btn.clicked += () =>
            {
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter)) return;

                if(btn.name == "swap-btn")
                {
                    audioSource[1].Play();
                    return;
                }

                audioSource[0].Play();
            };
        }
    }


}
