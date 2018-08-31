using System;
using UnityEngine;

namespace Entitas.Data
{
    public interface IVolumetricLine
    {
        Vector3 StartPos { get; set; }
        Vector3 EndPos { get; set; }
        float LineWidth { get; set; }
    }
}
