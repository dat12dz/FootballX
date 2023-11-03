using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    byte value = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            UINew_LobbyScreen.Show();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            UINew_GoalScreen.Show("Goal!!!", "Abc", 10);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UINew_InGameScreen.DisableInGameScreen();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UINew_ChangeSceneEffect.Open();
            UINew_ChangeSceneEffect.Close();
        }
    }
}
