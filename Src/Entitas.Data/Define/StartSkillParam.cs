using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public class StartSkillParam
    {
        public int SkillId;
        public uint SenderId;
        public Vector3 SenderPosition;
        public float SenderDirection;
    }
    public class StartBuffParam
    {
        public int BuffId;
        public uint SenderId;
        public Vector3 SenderPosition;
        public float SenderDirection;
    }
}
