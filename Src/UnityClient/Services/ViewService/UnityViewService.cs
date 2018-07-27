using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClient
{
    public sealed class UnityViewService : Service, IViewService
    {
        public UnityViewService(Contexts contexts): base(contexts)
        {
        }
        public void CreateGameObject(uint resId, string resource, Vector3 position, Vector3 eular, Vector3 scale, float recyleTime)
        {
        }
    }
}
