using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TransitionInit : MonoBehaviour
{
    public static TransitionInit instance;
    [SerializeField] RenderTexture texture;
    [SerializeField] Camera cam;
    Vector3 camDefaultPos;
    void Start()
    {
        camDefaultPos = cam.transform.position;
        texture.width = Screen.currentResolution.width;
        texture.height = Screen.currentResolution.height;
        instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }
    
    public void StartRendering()
    {
        gameObject.SetActive(true);
        cam.transform.DOMove(new Vector3(camDefaultPos.x, camDefaultPos.y, camDefaultPos.z - 2), 1f).SetEase(Ease.OutCubic);

    }
    public void StopRendering()
    {
        cam.transform.position = camDefaultPos;
        gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
}
