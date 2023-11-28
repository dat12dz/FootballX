using Assets.Script.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]

class MVPAnimationRef
{
    [SerializeField] public Transform MvpAnimationRoot;
    
    [SerializeField]
    public Camera MVPCamera;
    
    [SerializeField]
   public  Camera Player_ShowingCam;
    [SerializeField] string MvpAnimClip,MVpCameraClipAnim;
    Camera Playereye;
    Animator ModelAnimator;
    public Action OnCleanUp;
    public async Task Play(Animator playerModelAnimator, Camera playereye, bool usingRootMotion = false)
    {

        playerModelAnimator.applyRootMotion = usingRootMotion;
        if (playereye)
            playereye.gameObject.SetActive(false);
        Playereye = playereye;
        ModelAnimator = playerModelAnimator;
        //  MvpAnimationRoot.SetParent( playerModelAnimator.transform,false);
        MvpAnimationRoot.gameObject.SetActive(true);
        var t = playerModelAnimator.PlayAndWait(MvpAnimClip);
        MVPCamera.GetComponent<Animator>().Play(MVpCameraClipAnim);


        await t;
        //    CleanUp(playereye);
    }
    public void CleanUp()
    {
        if (Playereye)
            Playereye.gameObject.SetActive(true);
        MvpAnimationRoot.gameObject.SetActive(false);
        ModelAnimator.applyRootMotion = false;
        ModelAnimator.transform.localPosition = Vector3.zero;
        ModelAnimator.transform.localRotation = Quaternion.Euler(Vector3.zero);
        if (OnCleanUp != null) OnCleanUp();
    }
    public void CreateRef()
    {

    }
}

