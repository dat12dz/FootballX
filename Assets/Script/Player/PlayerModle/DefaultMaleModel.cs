using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMaleModel : PlayerModelBase
{
    [Serializable]
    class TeamReference
    {
        public GameObject model;
    }

    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;
  
    const string IDLE_ANIM_CLIP = "idlle";
    const string SELECT_ANIM_CLIP = "Root_Selected";
    const int SELECLAYER_LAYER = 1;
    public override void RedTeamInit()
    {
        redTeamRef.model.SetActive(true);
        animator = redTeamRef.model.GetComponent<Animator>();
        ActiveModel = redTeamRef.model;
        BlueTeamRef.model.SetActive(false);
    }
    
    public override void BlueTeamInit()
    {
        redTeamRef.model.SetActive(false);
        animator = BlueTeamRef.model.GetComponent<Animator>();
        ActiveModel = BlueTeamRef.model;
        BlueTeamRef.model.SetActive(true);
    }
    public override void IdleAnim()
    {
        animator.SetLayerWeight(SELECLAYER_LAYER, 0F);
        animator.Play(IDLE_ANIM_CLIP);
    }


    public override void SelectedAnim()
    {
        animator.SetLayerWeight(SELECLAYER_LAYER, 0.86F);
        animator.Play(SELECT_ANIM_CLIP, SELECLAYER_LAYER);
    }
    protected override void Start()
    {
        base.Start();
    }
    // Start is called before the first frame update

}
