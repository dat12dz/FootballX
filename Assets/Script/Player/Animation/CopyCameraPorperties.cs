using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class CopyCameraPorperties : MonoBehaviour
{
    [SerializeField] Camera copyFrom;
    Camera thisCam;
    private void Start()
    {
        thisCam = GetComponent<Camera>();
    }

    private void Update()
    {
        thisCam.focalLength = copyFrom.focalLength;
    }
}
