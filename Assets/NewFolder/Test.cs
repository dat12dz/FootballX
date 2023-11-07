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
            UINew_MVPScreen.Show();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UINew_MVPScreen.ResetStyle();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UINew_ChangeSceneEffect.Open();
            UINew_ChangeSceneEffect.Close();
        }
    }
}
