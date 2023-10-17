using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Charector array",menuName = "DatDauTechonologies/Charector array",order = 0)]
internal class AllCharectorAssetReference : ScriptableObject
{
    public PlayerModelBase[] CharArray;
}
