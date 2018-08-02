using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class UnityPhysicsService : Service
    {
        public UnityPhysicsService(Contexts contexts) : base(contexts)
        {
        }
        public void CreateBoxCollider(IView view, long remainTime, Vector3 size, Vector3 offset, MyAction<uint> callback)
        {
            if(view is UnityView unityView)
            {
                GameObject obj = new GameObject();
                BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.size = size;
                UnityCollider unityRigid = obj.AddComponent<UnityCollider>();

                obj.transform.parent = unityView.transform;
                obj.transform.localPosition = offset;

                unityRigid.OnColliderTriggerEnter = callback;

                GameObject.Destroy(obj, remainTime / 1000.0f);
            }
        }
    }
}
