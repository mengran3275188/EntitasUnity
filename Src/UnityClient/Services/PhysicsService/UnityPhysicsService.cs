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
        public void CreateBoxCollider(IView view, int layerId, long remainTime, Vector3 size, Vector3 offset, Quaternion rotation, MyAction<uint> callback)
        {
            if(view is UnityView unityView)
            {
                GameObject obj = new GameObject();
                obj.layer = layerId;
                BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.size = size;
                UnityCollider unityRigid = obj.AddComponent<UnityCollider>();

                obj.transform.parent = unityView.transform;
                obj.transform.localPosition = offset;
                obj.transform.localRotation = rotation;

                unityRigid.OnColliderTriggerEnter = callback;

                GameObject.Destroy(obj, remainTime / 1000.0f);
            }
        }
    }
}
