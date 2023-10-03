using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageBox : MonoBehaviour
{
    static MessageBox instance;
    static VisualElement root;
    static VisualElement container;
    static VisualElement messageBox;
    static VisualElement messageBoxBackground;
    static VisualElement boxText;
    static VisualElement boxBtn;
    static Button confirmBtn;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        messageBox = root.Q<VisualElement>("message-box");
        messageBoxBackground = root.Q<VisualElement>("message-box-background");
        boxText = root.Q<VisualElement>("box-text");
        boxBtn = root.Q<VisualElement>("box-btn");
        confirmBtn = root.Q<Button>("confirm-btn");

    }

    public static async Task Show(string title, string text)
    {
        root.Q<Label>("title").text = title;
        root.Q<Label>("description").text = text;
        container.style.display = DisplayStyle.Flex;
        container.style.opacity = 1;
        messageBox.style.height = 200;
        boxText.style.display = DisplayStyle.Flex;
        boxBtn.style.display = DisplayStyle.Flex;
        await Task.Delay(150);
        messageBoxBackground.style.opacity = 1;

        confirmBtn.clicked += () =>
        {
            ResetMessageBox();
        };
    }

    public static async void ResetMessageBox()
    {
        messageBox.style.height = 0;
        messageBoxBackground.style.opacity = 0;
        boxText.style.display = DisplayStyle.None;
        boxBtn.style.display = DisplayStyle.None;
        await Task.Delay(150);
        container.style.display = DisplayStyle.None;
        container.style.opacity = 0;
    }
}