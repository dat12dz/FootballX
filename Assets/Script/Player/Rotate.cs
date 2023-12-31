﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

namespace Assets.Utlis
{
    public class Rotate : MonoBehaviour
    {
        public CameraShake cameraShake;
        [SerializeField] Player player;
        Camera PlayerCam;
        [SerializeField] public float MouseSen;

        [SerializeField] public Transform HeadObj;
        private void Start()
        {
            MouseSen = SettingDAO.Instance.gameplaySetting.MouseSen;
            PlayerCam = GetComponent<Camera>();
            if (!PlayerCam) Logging.LogObjectNull(nameof(PlayerCam));
            if (!HeadObj) Logging.LogObjectNull(nameof(HeadObj));
            player = transform.root.GetComponent<Player>(); 
            
        }
        private void Update()
        {
            cameraShake = GetComponent<CameraShake>();
            if (player.IsLocalPlayer)
            {
                var xMouse = Input.GetAxis("Mouse X");
                var yMouse = Input.GetAxis("Mouse Y");
                
                if (yMouse != 0 || xMouse != 0)
                RotateCam(xMouse, yMouse);
            }
            else
            {
                PlayerCam.enabled = false;
            }
        }
        float YRotate = 0;
        void RotateCam(float xMouse, float yMouse)
        {
            // clamp X
            
            YRotate -= yMouse * MouseSen * Time.deltaTime;

            YRotate = Math.Clamp(YRotate, -90, 90);

            var Xrotate = xMouse * MouseSen * Time.deltaTime;
            PlayerCam.transform.localRotation = Quaternion.Euler(YRotate, 0, 0);
            HeadObj.Rotate(new Vector3(0, Xrotate, 0) );
           var HeadYRotated = HeadObj.transform.localRotation.eulerAngles.y;
            var CameraXRotated = YRotate;
            transform.root.GetComponent<Move>().MoveCameraServerRpc(YRotate, HeadYRotated);
        }

    }
}
