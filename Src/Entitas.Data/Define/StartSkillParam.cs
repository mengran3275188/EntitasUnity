using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public class StartSkillParam
    {
        public int Id;
        public int Category;
        public uint SenderId;
        public Vector3 SenderPosition;
        public float SenderDirection;
    }
    public class StartBuffParam
    {
        public int Id;
        public uint SenderId;
        public Vector3 SenderPosition;
        public float SenderDirection;
    }
}
