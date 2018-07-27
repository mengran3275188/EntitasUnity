using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface IChunk
    {
        BoxCollider[] GetBaseCollider();
        BoxCollider[] GetDoorsCollider();
        BoxCollider[] GetTriggers();
    }
}
