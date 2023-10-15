using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IPlayerModel : MonoBehaviour
{
    [SerializeField]
   public SkinnedMeshRenderer faceRender, bodyRender;
    [SerializeField]
    public Transform head;
    public Texture2D Thumbnail;
   public abstract void IdleAnim();
    public abstract void SelectedAnim();
    public abstract void RedTeamInit();
    public abstract void BlueTeamInit();
}

