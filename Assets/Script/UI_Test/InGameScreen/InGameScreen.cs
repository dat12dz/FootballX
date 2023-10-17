using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameScreen : MonoBehaviour
{
    static private VisualElement root;
    static private VisualElement container;
    static private VisualElement scoreBoard;
    static private Label redTeamScore;
    static private Label blueTeamScore;
    static private Label matchTime;
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
        matchTime = root.Q<Label>("match-time");
        kickBar = root.Q<VisualElement>("kick-bar");
        kickBarProgress = root.Q<VisualElement>("kick-bar-progress");
        kickBarOverrideBackground = root.Q<VisualElement>("kick-bar-override-background");
        InitStyle();

    }

    private void Update()
    {
        ProgressBar(value);
        
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
