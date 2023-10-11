using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerModel : MonoBehaviour
{
    public PlayerThumbnailReference Thumbnail;
    Animator animator;
    const string SELECT_ANIM_CLIP = "Selected";
    const string IDLE_ANIM_CLIP = "idle";
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    [ContextMenu("PlayIdle")]
    public void IdleAnim()
    {
        animator.PlayInFixedTime(IDLE_ANIM_CLIP);
    }
    [ContextMenu("Play Selection animation")]
    public void SelectedAnim()
    {
        animator.PlayInFixedTime(SELECT_ANIM_CLIP);
    }
    void CloseEye()
    {
        float x = 0;
       
      //  DOTween.To(() => x,)
    }
    void Update()
    {
        
    }
}
