using Assets.Utlis;
using System;
using Unity.Netcode;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Assets.Script.NetCode;

public class UINew_InGameScreen : WaitForInstaceNotNull<UINew_InGameScreen>
{
    static private VisualElement root;
    static private VisualElement container;
    static private VisualElement scoreBoard;
    static private Label redTeamScore;
    static private Label blueTeamScore;
    static private VisualElement matchTime;
    static private Label minLabel;
    static private Label secLabel;
    static private VisualElement kickBar;
    static private VisualElement kickBarProgress;
    static private VisualElement kickBarOverrideBackground;
    [SerializeField] byte value;
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("container");
        scoreBoard = root.Q<VisualElement>("score-board");
        redTeamScore = root.Q<Label>("red-team-score");
        blueTeamScore = root.Q<Label>("blue-team-score");
        matchTime = root.Q<VisualElement>("match-time");
        minLabel = root.Q<Label>("min-label");
        secLabel = root.Q<Label>("sec-label");
        kickBar = root.Q<VisualElement>("kick-bar");
        kickBarProgress = root.Q<VisualElement>("kick-bar-progress");
        kickBarOverrideBackground = root.Q<VisualElement>("kick-bar-override-background");
        InitStyle();
        EnableInGameScreen();

        //  CheckNullOrNot();
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
        {
            Destroy(gameObject);
        }

        GameSystem game = SceneHelper.GetGameSystem(gameObject.scene);
        game.OnTimeChange += ShowTime;
        game.OnScoreChange += ShowScore;
        //Btn_Disconnect.onClick.AddListener(Btn_disCOnnect);
        
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        instance = this;
    }

    void Btn_disCOnnect()
    {
        //  NetworkSystem.instance.TryDisconnectoall();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
    void CheckNullOrNot()
    {
        //Logging.CheckNLogObjectNull(pnl_ingameInformer, nameof(pnl_ingameInformer));
        Logging.CheckNLogObjectNull(blueTeamScore, nameof(blueTeamScore));
        Logging.CheckNLogObjectNull(matchTime, nameof(matchTime));
        Logging.CheckNLogObjectNull(redTeamScore, nameof(redTeamScore));
    }
    void ShowScore(int Red, int Blue, GameSystem.Team Goaler)
    {
        Action res = null;
        if (Goaler == GameSystem.Team.red)
        {
            res = () => UINew_GoalScreen.Show("Goalllll!!", "Red team gooalll", 10, UINew_GoalScreen.ColorName.red_black);
        }
        if (Goaler == GameSystem.Team.blue)
        {
            res = () => UINew_GoalScreen.Show("Goalllll!!", "Blue team gooalll", 10, UINew_GoalScreen.ColorName.blue_white);
        }
        MainThreadDispatcher.ExecuteInMainThread(res);
        var RedScore = Red.ToString();
        var BlueScore = Blue.ToString();
        MainThreadDispatcher.ExecuteInMainThread(() =>
        {
            redTeamScore.text = RedScore;
            blueTeamScore.text = BlueScore;
        });

    }
    void ShowTime(int time)
    {
        var Minute = time / 60;
        var Second = time % 60;
        var ShowMinString = Minute <= 9 ? $"0{Minute}" : Minute.ToString();
        var ShowSecondString = Second <= 9 ? $"0{Second}" : Second.ToString();

        MainThreadDispatcher.ExecuteInMainThread(() =>
        {
            minLabel.text = ShowMinString;
            secLabel.text = ShowSecondString;
        });

    }
    public void ShowInformation(string Title, string Infomation, int sec)
    {
        MainThreadDispatcher.ExecuteInMainThread(() => {
            UINew_GoalScreen.Show(Title, Infomation, sec, UINew_GoalScreen.ColorName.black_white); 
        });
    }
    bool isCursorLocked = true;

    void LockCursor()
    {

        if (isCursorLocked)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            LockCursor();
            ProgressBar(value);
        }
    }

    public static void ProgressBar(byte value)
    {
        float c = 1 - ((255/100) * (float)value / 255);
        kickBarOverrideBackground.style.backgroundColor = new Color(1, c, c);
        kickBarProgress.style.width = new StyleLength(new Length(value, LengthUnit.Percent));
    }

    public static async void EnableInGameScreen()
    {
        scoreBoard.style.translate = new StyleTranslate(new Translate(0, 0));
        await Task.Delay(200);
        await Task.Delay(800);
        matchTime.style.opacity = 1;
        matchTime.style.bottom = new StyleLength(new Length(-26, LengthUnit.Percent));
        await Task.Delay(200);
    }

    public static void DisableInGameScreen()
    {
        scoreBoard.style.translate = new StyleTranslate(new Translate(0, -120));
        matchTime.style.opacity = 0;
        matchTime.style.bottom = new StyleLength(new Length(26, LengthUnit.Percent));
    }

    void InitStyle()
    {
        scoreBoard.style.translate = new StyleTranslate(new Translate(0, -120));
        matchTime.style.opacity = 0;
        matchTime.style.bottom = new StyleLength(new Length(26, LengthUnit.Percent));
    }
}
