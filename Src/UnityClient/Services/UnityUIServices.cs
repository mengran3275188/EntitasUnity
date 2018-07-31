using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClient
{
    public class UnityUIServices : Service
    {
        public UnityUIServices(Contexts contexts) : base(contexts)
        {
        }
        public void Init()
        {
            m_UIRoot = GameObject.Find("GameLogic/Canvas/TempUIRoot").transform;
        }
        public void LoadUI(string path)
        {
            GameObject o =Resources.Load(path) as GameObject;
            GameObject ui = GameObject.Instantiate(o);

            ui.transform.SetParent(m_UIRoot, false);
        }
        public void Cleanup()
        {
            for(int i = m_UIRoot.transform.childCount - 1; i >= 0; --i)
            {
                GameObject.DestroyImmediate(m_UIRoot.GetChild(i).gameObject);
            }
        }

        private Transform m_UIRoot;
    }
}
