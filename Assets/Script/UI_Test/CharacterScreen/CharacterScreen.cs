using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterScreen : MonoBehaviour
{
    [SerializeField] AllCharectorAssetReference allChar;
    VisualElement root;
    VisualElement container;
    VisualElement characterScreen;
    VisualElement unityContentContainer;
    Button selectBtn;
    Button redClothes;
    Button blueClothes;
    Button[] charBtns;
    int charIndex;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        characterScreen = root.Q<VisualElement>("character-screen");
        unityContentContainer = root.Q<VisualElement>("unity-content-container");
        selectBtn = root.Q<Button>("select-btn");
        redClothes = root.Q<Button>("red-clothes");
        blueClothes = root.Q<Button>("blue-clothes");
        AddCharacter();
    }

    void AddCharacter()
    {
        for (int i = 0; i < allChar.CharArray.Length; i++)
        {
            Button b = new Button();
            b.AddToClassList("character");
            b.AddToClassList("btn-handle");
            b.style.backgroundImage = allChar.CharArray[i].Thumbnail;
            unityContentContainer.Add(b);
            b.clicked += () =>
            {
                charIndex = i;
            };
        }
    }

    private void Update()
    {
        Debug.Log(charIndex);
    }
}
