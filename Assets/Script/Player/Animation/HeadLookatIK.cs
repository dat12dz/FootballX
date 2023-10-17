using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLookatIK : MonoBehaviour
{
    public Transform CameraLookat;
    void Start()
    {
        var trygetPlayer = transform.root.GetComponent<Player>();
        if (trygetPlayer != null)
        CameraLookat = trygetPlayer.PlayerLookAt;

    }

    // Update is called once per frame
    void Update()
    {
        if (CameraLookat)
        {
            transform.rotation = CameraLookat.transform.rotation;
            transform.position = CameraLookat.transform.position;
        }
    }
}
