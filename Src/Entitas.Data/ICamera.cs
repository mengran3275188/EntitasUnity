using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface ICameraService
    {
        Quaternion Rotation { get; }
        Vector3 WorldToScreenPoint(Vector3 pos);
        Vector3 WorldToViewportPoint(Vector3 pos);
        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
    }
}
