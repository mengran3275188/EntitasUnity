using System;
using System.Collections.Generic;
using UnityEngine;
using Util;
using System.Linq;
using System.Text;

namespace ScriptableData
{
    public delegate void MessageHandlerDelegate(CSharpInstance instance, long time, long deltaTime);

    public class CSharpMessageHandler
    {
        public string MessageId
        {
            get { return m_MessageId; }
            set { m_MessageId = value; }
        }
        public MessageHandlerDelegate Delegate
        {
            set { m_Delegate = value; }
        }
        public CSharpMessageHandler Clone()
        {
            CSharpMessageHandler handler = new CSharpMessageHandler();
            handler.m_MessageId = m_MessageId;
            handler.m_Delegate = m_Delegate;
            return handler;
        }
        public void Load()
        {
        }
        public void Reset()
        {
        }
        public void Prepare()
        {
        }
        public bool IsFinished()
        {
            return false;
        }
        public void Tick(CSharpInstance instance, long delta)
        {
            if (delta > 0)
                m_Delegate(instance, instance.SkillTime, delta);
        }
        public void Analyze(CSharpInstance instance)
        {
        }
        public void Trigger(CSharpInstance instance, object[] args)
        {
        }

        private string m_MessageId = "";
        private object[] m_Arguments = null;
        private MessageHandlerDelegate m_Delegate;
    }
    public class CSharpInstance : IInstance
    {
        public long Time
        {
            get { return m_LastTickTime; }
        }
        public long SkillTime
        {
            get { return m_CurTime; }
        }
        public int Id
        {
            get { return m_Id; }
        }
        public uint SenderId
        {
            get { return m_SenderId; }
            set { m_SenderId = value; }
        }
        public Vector3 SenderPosition
        {
            get { return m_SenderPosition; }
            set { m_SenderPosition = value; }
        }
        public float SenderDirection
        {
            get { return m_SenderDir; }
            set { m_SenderDir = value; }
        }
        public object Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }
        public bool IsTerminated
        {
            get { return m_IsTerminated; }
            set { m_IsTerminated = value; }
        }
        public object Context
        {
            get { return m_Context; }
            set { m_Context = value; }
        }
        public Vector3 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }
        public bool DisableMoveInput
        {
            get { return m_DisableMoveInput; }
            set { m_DisableMoveInput = value; }
        }
        public bool DisableRotationInput
        {
            get { return m_DisableRotationInput; }
            set { m_DisableRotationInput = value; }
        }
        public Dictionary<string, object> LocalVariables
        {
            get { return m_LocalVariables; }
        }
        public Dictionary<string, object> GlobalVariables
        {
            get { return m_GlobalVariables; }
            set { m_GlobalVariables = value; }
        }
        public List<ICommand> ParallelCommands
        {
            get { return m_ParallelCommands; }
        }
        public IInstance Clone()
        {
            CSharpInstance instance = new CSharpInstance();
            foreach (var handler in m_MessageHandlers)
            {

                instance.m_MessageHandlers.Add(handler.Clone());
                string msgId = handler.MessageId;
                if (!instance.m_MessageQueues.ContainsKey(msgId))
                {
                    instance.m_MessageQueues.Add(msgId, new Queue<MessageInfo>());
                }
            }
            /*
            foreach (StoryMessageHandler handler in m_MessageHandlers) {
              instance.m_MessageHandlers.Add(handler.Clone());
            }*/
            instance.m_Id = m_Id;
            return instance;
        }
        public virtual bool Init()
        {
            return true;
        }
        public void Reset()
        {
            m_IsTerminated = false;
            int ct = m_MessageHandlers.Count;
            for (int i = ct - 1; i >= 0; --i)
            {
                CSharpMessageHandler handler = m_MessageHandlers[i];
                handler.Reset();
                Queue<MessageInfo> queue;
                if (m_MessageQueues.TryGetValue(handler.MessageId, out queue))
                {
                    queue.Clear();
                }
            }

            m_DisableMoveInput = false;
            m_DisableRotationInput = false;
            m_Velocity = Vector3.zero;

            m_ActiveHandlers.Clear();
            m_ParallelCommands.Clear();
            m_LocalVariables.Clear();
        }
        public void Start()
        {
            m_LastTickTime = 0;
            m_CurTime = 0;

            SendMessage("start");
        }
        public void SendMessage(string msgId, params object[] args)
        {
            MessageInfo msgInfo = new MessageInfo();
            msgInfo.m_MsgId = msgId;
            msgInfo.m_Args = args;
            Queue<MessageInfo> queue;
            if (m_MessageQueues.TryGetValue(msgId, out queue))
            {
                queue.Enqueue(msgInfo);
            }
        }
        public void ClearMessage(params string[] msgIds)
        {
            int len = msgIds.Length;
            if (len <= 0)
            {
                foreach (Queue<MessageInfo> queue in m_MessageQueues.Values)
                {
                    queue.Clear();
                }
            }
            else
            {
                for (int i = 0; i < len; ++i)
                {
                    string msgId = msgIds[i];
                    Queue<MessageInfo> queue;
                    if (m_MessageQueues.TryGetValue(msgId, out queue))
                    {
                        queue.Clear();
                    }
                }
            }
        }
        public void Tick(long curTime)
        {
            const int c_MaxMsgCountPerTick = 256;
            long delta = 0;
            if (m_LastTickTime == 0)
            {
                m_LastTickTime = curTime;
            }
            else
            {
                delta = curTime - m_LastTickTime;
                m_LastTickTime = curTime;
                m_CurTime += delta;
            }

            foreach (CSharpMessageHandler handler in m_MessageHandlers)
            {
                string msgId = handler.MessageId;
                Queue<MessageInfo> queue;
                if (m_MessageQueues.TryGetValue(msgId, out queue))
                {
                    for (int msgCt = 0; msgCt < c_MaxMsgCountPerTick && queue.Count > 0; ++msgCt)
                    {
                        MessageInfo info = queue.Dequeue();
                        CSharpMessageHandler activeHandler = handler.Clone();
                        activeHandler.Trigger(this, info.m_Args);
                        m_ActiveHandlers.Add(activeHandler);
                    }
                }
            }
            int ct = m_ParallelCommands.Count;
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                ICommand cmd = m_ParallelCommands[ix];
                if (cmd.Execute(this, delta) == ExecResult.Finished)
                {
                    cmd.Reset();
                    m_ParallelCommands.Remove(cmd);
                }
            }
            ct = m_ActiveHandlers.Count;
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                long dt = delta;
                CSharpMessageHandler handler = m_ActiveHandlers[ix];
                handler.Tick(this, dt);
                if (handler.IsFinished())
                    m_ActiveHandlers.RemoveAt(ix);
            }
        }
        public void Analyze()
        {
        }
        public void AddVariable(string varName, object val)
        {
            if (varName.StartsWith("@@"))
            {
                AddLocalVariable(varName, val);
            }
            else if (varName.StartsWith("@"))
            {
                AddGlobalVariable(varName, val);
            }
            else
            {
                Util.LogUtil.Error("AddVariable var name {0} is invalid! var name should starts with @ or @@.", varName);
            }
        }
        public void AddMessageHandler(string msgId, MessageHandlerDelegate action)
        {
            CSharpMessageHandler handler = new CSharpMessageHandler();
            handler.MessageId = msgId;
            handler.Delegate = action;
            m_MessageHandlers.Add(handler);
        }

        private void AddLocalVariable(string varName, object val)
        {
            if (m_LocalVariables.ContainsKey(varName))
            {
                m_LocalVariables[varName] = val;
            }
            else
            {
                m_LocalVariables.Add(varName, val);
            }
        }
        private void AddGlobalVariable(string varName, object val)
        {
            if (m_GlobalVariables.ContainsKey(varName))
            {
                m_GlobalVariables[varName] = val;
            }
            else
            {
                m_LocalVariables.Add(varName, val);
            }
        }
        private class MessageInfo
        {
            public string m_MsgId = null;
            public object[] m_Args = null;
        }

        private long m_CurTime = 0;
        private long m_LastTickTime = 0;

        private Dictionary<string, object> m_LocalVariables = new Dictionary<string, object>();
        private Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();

        private int m_Id = 0;
        private bool m_IsTerminated = false;
        private object m_Context = null;
        private object m_Target = null;

        private uint m_SenderId = 0;
        private Vector3 m_SenderPosition = Vector3.zero;
        private float m_SenderDir = 0;

        private Vector3 m_Velocity = Vector3.zero;
        private bool m_DisableRotationInput = false;
        private bool m_DisableMoveInput = false;

        private Dictionary<string, Queue<MessageInfo>> m_MessageQueues = new Dictionary<string, Queue<MessageInfo>>();
        private List<CSharpMessageHandler> m_ActiveHandlers = new List<CSharpMessageHandler>();
        private List<CSharpMessageHandler> m_MessageHandlers = new List<CSharpMessageHandler>();
        private List<ICommand> m_ParallelCommands = new List<ICommand>();
    }
}
