using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMaleModel : IPlayerModel
{
    [Serializable]
    class TeamReference
    {
        [SerializeField]
        public Material body;
        [SerializeField]
        public Material face;
    }

    [SerializeField]
    TeamReference redTeamRef, BlueTeamRef;

    public override void RedTeamInit()
    {
        faceRender.material = redTeamRef.face;
        bodyRender.material = redTeamRef.body;
    }

    public override void BlueTeamInit()
    {
        faceRender.material = BlueTeamRef.face;
        bodyRender.material = BlueTeamRef.body;
    }
    public override void IdleAnim()
    {
       
    }


    public override void SelectedAnim()
    {
       
    }

    // Start is called before the first frame update
   
}
