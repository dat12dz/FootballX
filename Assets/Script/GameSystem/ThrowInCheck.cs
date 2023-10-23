using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ThrowInCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if(NetworkManager.Singleton.IsServer)
        {
            if (collision.collider != null)
            {
                var Ball = collision.gameObject.GetComponent<Ball>();
                var ThrowInMaker = Ball.lastToucher;
                var Team = Ball.lastToucher;
                
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
