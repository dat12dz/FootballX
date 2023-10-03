using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

class NameTagAutoAdjust : MonoBehaviour
{
    public Player parent;
    public Transform background;
    TextMeshPro thisText;
    private void Start()
    {
        thisText = GetComponent<TextMeshPro>();
    
    }
    private void Update()
    {
        if (parent.IsLocalPlayer) gameObject.SetActive(false);
        if (parent.IsClient)
        {
            var localCam = Player.localPlayer.Playereyes;
            transform.LookAt(localCam.transform.position);
            transform.Rotate(0, 180, 0);
        }
    }
}
