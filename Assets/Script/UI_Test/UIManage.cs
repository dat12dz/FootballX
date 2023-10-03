using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManage : MonoBehaviour
{
    //UIManage instance;
    [SerializeField] private UIDocument MainMenu;
    [SerializeField] private UIDocument SettingScreen;
    [SerializeField] private UIDocument InGameScreen;
    [SerializeField] private UIDocument PauseScreen;
    [SerializeField] private UIDocument OptionsScreen;

    void Start()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        //DontDestroyOnLoad(gameObject);

        var rootMainMenu = MainMenu.rootVisualElement;
        var rootSettingScreen = SettingScreen.rootVisualElement;
        var rootInGameScreen = InGameScreen.rootVisualElement;
        var rootPauseScreen = PauseScreen.rootVisualElement;
        var rootOptionsScreen = OptionsScreen.rootVisualElement;

        /* Init button */
        // MainMenu button
        Button playBtn = rootMainMenu.Q<Button>("play-btn");
        Button settingBtn = rootMainMenu.Q<Button>("setting-btn");
        Button exitBtn = rootMainMenu.Q<Button>("exit-btn");

        // SettingScreen button
        Button backBtn = rootSettingScreen.Q<Button>("back-btn");

        // InGameScreen button
        Button pauseBtn = rootInGameScreen.Q<Button>("pause-btn");

        // Options button
        Button musicBtn = rootOptionsScreen.Q<Button>("music-btn");
        Button soundBtn = rootOptionsScreen.Q<Button>("sound-btn");
        Button confirmOptionsScreenBtn = rootOptionsScreen.Q<Button>("confirm-btn");
        Button cancelOptionsScreenBtn = rootOptionsScreen.Q<Button>("cancel-btn");

        // PauseScreen button
        Button resumeBtn = rootPauseScreen.Q<Button>("resume-btn");
        Button optionsBtn = rootPauseScreen.Q<Button>("options-btn");
        Button mainMenuBtn = rootPauseScreen.Q<Button>("main-menu-btn");

        /* Button click */
        // MainMenu button
        playBtn.clicked += () =>
        {
            DisplayDisable(rootMainMenu);
            DisplayEnable(rootInGameScreen);
        };

        settingBtn.clicked += () =>
        {
            DisplayDisable(rootMainMenu);
            DisplayEnable(rootSettingScreen);
        };

        exitBtn.clicked += async () =>
        {
            /* Show messageBox */
            //await MessageBox.EnableMessageBox("Abc");
            /* Show goalScreen*/
            //ShowGoalScreen.EnableGoalScreen();
        };

        // SettingScreen button
        backBtn.clicked += () =>
        {
            DisplayDisable(rootSettingScreen);
            DisplayEnable(rootMainMenu);
        };


        // InGameScreen button
        pauseBtn.clicked += () =>
        {
            DisplayEnable(rootPauseScreen);
        };

        // PauseScreen button
        resumeBtn.clicked += () =>
        {
            SceneManager.LoadScene(1);
            DisplayDisable(rootPauseScreen);
        };

        optionsBtn.clicked += () =>
        {
            DisplayDisable(rootPauseScreen);
            DisplayEnable(rootOptionsScreen);
        };

        mainMenuBtn.clicked += () =>
        {
            DisplayDisable(rootPauseScreen);
            DisplayDisable(rootInGameScreen);
            DisplayEnable(rootMainMenu);
        };

        // Options button
        musicBtn.clicked += () =>
        {
            
        };

        soundBtn.clicked += () =>
        {

        };

        confirmOptionsScreenBtn.clicked += () =>
        {
            DisplayDisable(rootOptionsScreen);
            DisplayEnable(rootPauseScreen);
        };

        cancelOptionsScreenBtn.clicked += () =>
        {
            DisplayDisable(rootOptionsScreen);
            DisplayEnable(rootPauseScreen);
        };
    }

    void DisplayEnable(VisualElement root)
    {
        root.Q<VisualElement>("container").style.display = DisplayStyle.Flex;
        root.Q<VisualElement>("container").style.opacity = 1;
    }
    void DisplayDisable(VisualElement root)
    {
        root.Q<VisualElement>("container").style.display = DisplayStyle.None;
        root.Q<VisualElement>("container").style.opacity = 0;
    }
}