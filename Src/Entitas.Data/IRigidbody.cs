using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface IRigidbody
    {
        int Layer { set; }
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        Quaternion Rotation { get; set; }
        bool IsKinematic { get; set; }
        void MovePosition(Vector3 pos);
    }
}
