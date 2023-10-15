using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    int value = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            LobbyScreen.Show();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GoalScreen.EnableGoalScreen();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            InGameScreen.EnableInGameScreen();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            InGameScreen.DisableInGameScreen();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            value--;
            Debug.Log($"is press left {value}");
            InGameScreen.ProgressBar(value);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log($"is press right {value}");
            value++;
            InGameScreen.ProgressBar(value);
        }

    }
}
