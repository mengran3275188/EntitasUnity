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
            m_UIRoot = GameObject.Find("GameLogic/Canvas").transform;
        }
        public void LoadUI(string path)
        {
            GameObject o =Resources.Load(path) as GameObject;
            GameObject ui = GameObject.Instantiate(o);

            ui.transform.SetParent(m_UIRoot, false);
        }

        private Transform m_UIRoot;
    }
}
