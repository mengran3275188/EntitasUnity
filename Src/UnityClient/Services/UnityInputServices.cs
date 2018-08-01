using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityDelegate;
using UnityEngine;
using Util;

namespace UnityClient
{
    public class UnityInputServices : Service
    {
        public UnityInputServices(Contexts contexts) : base(contexts)
        {
        }
        public bool IsKeyPressed(Keyboard.Code c)
        {
            return m_KeyPressed[(int)c];
        }
        public void ListenKeyPressState(params Keyboard.Code[] codes)
        {
            foreach(Keyboard.Code code in codes)
            {
                if (!m_KeysForListen.Contains((int)code))
                    m_KeysForListen.Add((int)code);
            }
        }
        public void ListenKeyboardEvent(Keyboard.Code c, MyAction<int, int> handler)
        {
            if (!m_KeysForListen.Contains((int)c))
                m_KeysForListen.Add((int)c);

            m_KeyboardHandlers[(int)c] = handler;

        }

        public void Tick()
        {
            foreach(int c in m_KeysForListen)
            {
                if(Input.GetKeyDown((KeyCode)c))
                {
                    m_KeyPressed[c] = true;
                    FireKeyboard(c, (int)Keyboard.Event.Down);
                }else if(Input.GetKeyUp((KeyCode)c))
                {
                    m_KeyPressed[c] = false;
                    FireKeyboard(c, (int)Keyboard.Event.Up);
                }
                if(!Input.GetKey((KeyCode)c))
                {
                    m_KeyPressed[c] = false;
                }
            }
        }
        private void FireKeyboard(int c, int e)
        {
            MyAction<int, int> handler;
            if (m_KeyboardHandlers.TryGetValue(c, out handler))
                if(null != GfxMoudle.Instance.LogicInvoker)
                    GfxMoudle.Instance.LogicInvoker.QueueAction<int, int>(handler, c, e);
        }
        private void FireMouse(int c, int e)
        {
            MyAction<int, int> handler;
            if (m_MouseHandlers.TryGetValue(c, out handler))
                if(null != GfxMoudle.Instance.LogicInvoker)
                    GfxMoudle.Instance.LogicInvoker.QueueAction<int, int>(handler, c, e);
        }

        private bool[] m_KeyPressed = new bool[(int)Keyboard.Code.MaxNum];
        private HashSet<int> m_KeysForListen = new HashSet<int>();

        private Dictionary<int, MyAction<int, int>> m_KeyboardHandlers = new Dictionary<int, MyAction<int, int>>();
        private Dictionary<int, MyAction<int, int>> m_MouseHandlers = new Dictionary<int, MyAction<int, int>>();
    }
}
