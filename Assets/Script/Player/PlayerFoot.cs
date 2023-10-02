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
        if (NetworkManager.Singleton.IsServer)
        if (collision.gameObject.layer == ballMask)
        {
            thisPlayer.Shootball(Ball.instance);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
