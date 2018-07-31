using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class UnityInputServices : Service
    {
        public UnityInputServices(Contexts contexts) : base(contexts)
        {
        }
        public void ListenKeyPressState(KeyCode[] codes)
        {
            foreach(KeyCode code in codes)
            {
                if (!m_KeysForListen.Contains((int)code))
                    m_KeysForListen.Add((int)code);
            }
        }
        public void Update()
        {
            foreach(int c in m_KeysForListen)
            {
                if(Input.GetKeyDown((KeyCode)c))
                {
                    m_KeyPressed[c] = true;
                    var e = _contexts.input.CreateEntity();
                    e.AddKeyDown((KeyCode)c);
                }
            }
        }

        private bool[] m_KeyPressed = new bool[c_MaxKeyCount]; // UnityEngine.KeyCode enum max
        private const int c_MaxKeyCount = 510;
        private HashSet<int> m_KeysForListen = new HashSet<int>();
    }
}
