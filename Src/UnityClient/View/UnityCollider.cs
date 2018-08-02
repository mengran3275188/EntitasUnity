using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Util;

namespace UnityClient
{
    public sealed class UnityCollider : MonoBehaviour
    {
        public MyAction<uint> OnColliderTriggerEnter;

        private void OnTriggerEnter(Collider collider)
        {
            UnityView view = collider.gameObject.GetComponent<UnityView>();
            if(null != view)
            {
                if(view.Entity is GameEntity gameEntity)
                {
                    if(gameEntity.hasId)
                    {
                        if(null != OnColliderTriggerEnter)
                            OnColliderTriggerEnter(gameEntity.id.value);
                    }
                }
            }
        }
    }
}
