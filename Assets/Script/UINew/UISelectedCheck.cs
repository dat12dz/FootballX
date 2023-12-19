using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.UINew
{
    class UISelectedCheck : MonoBehaviour
    {
        public static bool isUISelected;
        GraphicRaycaster raycaster;
        PointerEventData pointerEventData;
        EventSystem eventSystem;
        void Start()
        {
            // Get the necessary components
            raycaster = GetComponent<GraphicRaycaster>();
            eventSystem = GetComponent<EventSystem>();
        }

        void Update()
        {
            // Check for user input (e.g., mouse click or touch)
            if (Input.anyKeyDown)
            {
                // Create a pointer event data with the current input position
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = Input.mousePosition;

                // Perform the raycast
                Raycast(pointerEventData);
            }
        }

        void Raycast(PointerEventData pointerEventData)
        {
            // Create a list to store the raycast results
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast from the camera through the pointer position
            raycaster.Raycast(pointerEventData, results);

            // Process the raycast results
            foreach (RaycastResult result in results)
            {
                // Do something with the UI element (result.gameObject)
                Debug.Log("Hit UI Element: " + result.gameObject.name);
            }
        }
    }
}
