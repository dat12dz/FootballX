using Assets.Script.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.UI
{
    class Navigator : MonoBehaviour
    {
        public static Navigator instance;
       public Ball ball;
        Canvas root;
        RectTransform rectTransform;
        public Vector2 offset;
        private void Start()
        {
            instance =this;
            root = transform.root.GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (ball != null)
            {
                rectTransform.anchoredPosition = SceneHelper.ConvertToCanvasSpace(root,ball.transform.position) + offset;
            }
        }

    }
}
