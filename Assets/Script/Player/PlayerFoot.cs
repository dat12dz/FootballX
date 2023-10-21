using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerFoot : MonoBehaviour
{
    LayerMask ballMask;
  [SerializeField]  Player thisPlayer;
    Ball ball = Ball.instance;
    
    void Start()
    {
        ballMask = LayerMask.NameToLayer("Ball");
        thisPlayer = transform.root.GetComponent<Player>(); 
        if (thisPlayer == null )
        {
            Logging.LogError("Không thể tìm thấy người chơi");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Ball ball;
        if (NetworkManager.Singleton.IsClient)
        {
            ball = Ball.instance;
        }
        else
        ball = thisPlayer.System.sceneReference.ball;
        if (NetworkManager.Singleton.IsServer)
        if (collision.gameObject.layer == ballMask)
        {
            thisPlayer.Shootball(ball);
        }
     ;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
