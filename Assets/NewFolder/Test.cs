using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    byte value = 0;
    UINew_MVPScreen mvp;
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        mvp = new UINew_MVPScreen(root);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            mvp.Show();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            mvp.ResetStyle();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UINew_ChangeSceneEffect.Open();
            UINew_ChangeSceneEffect.Close();
        }
    }
}
