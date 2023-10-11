using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IPlayerModel : MonoBehaviour
{
   
   public PlayerThumbnailReference Thumbnail;
   public abstract void IdleAnim();
    public abstract void SelectedAnim(); 
}

