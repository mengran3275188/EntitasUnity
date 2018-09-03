using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace ScriptableData
{
    public interface IInstance
    {
        int Id { get; }
        long Time { get; }
        long SkillTime { get; }
        uint SenderId { get; set; }
        Vector3 SenderPosition { get; set; }
        float SenderDirection { get; set; }
        object Target { get; set; }
        bool IsTerminated { get; set; }
        bool DisableMoveInput { get; set; }
        bool DisableRotationInput { get; set; }
        object Context { get; set; }
        Vector3 Velocity { get; set; }
        List<ICommand> ParallelCommands { get; }
        Dictionary<string, object> LocalVariables { get; }
        Dictionary<string, object> GlobalVariables { get; set; }
        void Start();
        void Tick(long curTime);
        void SendMessage(string msgId, params object[] args);
        void ClearMessage(params string[] msgIds);
        void AddVariable(string varName, object val);
        void Reset();
        IInstance Clone();
    }

    public sealed class MessageHandler
    {
        public string MessageId
        {
            get { return m_MessageId; }
        }
        public MessageHandler Clone()
        {
            MessageHandler handler = new MessageHandler();
            foreach (ICommand cmd in m_LoadedCommands)
            {
                handler.m_LoadedCommands.Add(cmd.Clone());
            }
            handler.m_MessageId = m_MessageId;
            return handler;
        }
        public void Load(ScriptableData.FunctionData messageHandlerData)
        {
            ScriptableData.CallData callData = messageHandlerData.Call;
            if (null != callData && callData.HaveParam())
            {
                string[] args = new string[callData.GetParamNum()];
                for (int i = 0; i < callData.GetParamNum(); ++i)
                {
                    args[i] = callData.GetParamId(i);
                }
                m_MessageId = string.Join(":", args);
            }
            RefreshCommands(messageHandlerData);
        }
        public void Reset()
        {
            foreach (ICommand cmd in m_CommandQueue)
            {
                cmd.Reset();
            }
            m_CommandQueue.Clear();
        }
        public void Prepare()
        {
            Reset();
            foreach (var command in m_LoadedCommands)
            {
                m_CommandQueue.Enqueue(command);
            }
        }
        public bool IsFinished()
        {
            return m_CommandQueue.Count == 0;
        }
        public void Tick(Instance instance, long delta)
        {
            while (m_CommandQueue.Count > 0)
            {
                ICommand cmd = m_CommandQueue.Peek();
                ExecResult result = cmd.Execute(instance, delta);
                if (result == ExecResult.Blocked)
                {
                    break;
                }
                else if (result == ExecResult.Finished)
                {
                    cmd.Reset();
                    m_CommandQueue.Dequeue();
                }
                else if (result == ExecResult.Parallel)
                {
                    instance.ParallelCommands.Add(m_CommandQueue.Dequeue());
                }
                else
                {
                    LogUtil.Warn("Unknown skill exec result, skill {0}", instance.Id);
                }
            }
        }
        public void Analyze(Instance instance)
        {
            foreach (ICommand cmd in m_LoadedCommands)
            {
                cmd.Analyze(instance);
            }
        }
        public void Trigger(Instance instance, object[] args)
        {
            Prepare();
            m_Arguments = args;
            foreach (ICommand cmd in m_CommandQueue)
            {
                cmd.Prepare(instance, args.Length, args);
            }
        }
        private void RefreshCommands(ScriptableData.FunctionData handlerData)
        {
            m_LoadedCommands.Clear();
            foreach (var statement in handlerData.Statements)
            {
                ICommand cmd = CommandManager.Instance.CreateCommand(statement);
                if (null != cmd)
                    m_LoadedCommands.Add(cmd);
            }
        }

        private string m_MessageId = "";

        private Queue<ICommand> m_CommandQueue = new Queue<ICommand>();
        private object[] m_Arguments = null;
        private List<ICommand> m_LoadedCommands = new List<ICommand>();
    }
    public sealed class Instance : IInstance
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
            Instance instance = new Instance();
            foreach (KeyValuePair<string, object> pair in m_PreInitedLocalVariables)
            {
                instance.m_PreInitedLocalVariables.Add(pair.Key, pair.Value);
            }
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
        public bool Init(ScriptableData.ScriptableDataInfo config)
        {
            bool ret = false;
            ScriptableData.FunctionData skill = config.First;
            if (null != skill && (skill.GetId() == "skill" || skill.GetId() == "script"))
            {
                ret = true;
                ScriptableData.CallData callData = skill.Call;
                if (null != callData && callData.HaveParam())
                {
                    m_Id = int.Parse(callData.GetParamId(0));
                }
                for (int i = 0; i < skill.Statements.Count; i++)
                {
                    if (skill.Statements[i].GetId() == "local")
                    {
                        ScriptableData.FunctionData sectionData = skill.Statements[i] as ScriptableData.FunctionData;
                        if (null != sectionData)
                        {
                            for (int j = 0; j < sectionData.Statements.Count; j++)
                            {
                                ScriptableData.CallData defData = sectionData.Statements[j] as ScriptableData.CallData;
                                if (null != defData && defData.HaveId() && defData.HaveParam())
                                {
                                    string id = defData.GetId();
                                    if (id.StartsWith("@") && !id.StartsWith("@@"))
                                    {
                                        SkillValue val = new SkillValue();
                                        val.InitFromDsl(defData.GetParam(0));
                                        if (!m_PreInitedLocalVariables.ContainsKey(id))
                                        {
                                            m_PreInitedLocalVariables.Add(id, val.Value);
                                        }
                                        else
                                        {
                                            m_PreInitedLocalVariables[id] = val.Value;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogUtil.Error("Story {0} DSL, local must be a function !", m_Id);
                        }
                    }
                    else if (skill.Statements[i].GetId() == "onmessage")
                    {
                        ScriptableData.FunctionData sectionData = skill.Statements[i] as ScriptableData.FunctionData;
                        if (null != sectionData)
                        {
                            MessageHandler handler = new MessageHandler();
                            handler.Load(sectionData);
                            string msgId = handler.MessageId;
                            if (!m_MessageQueues.ContainsKey(msgId))
                            {
                                m_MessageHandlers.Add(handler);
                                m_MessageQueues.Add(msgId, new Queue<MessageInfo>());
                            }
                            else
                            {
                                LogUtil.Error("Story {0} DSL, onmessage {1} duplicate, discard it !", m_Id, msgId);
                            }
                        }
                        else
                        {
                            LogUtil.Error("Story {0} DSL, onmessage must be a function !", m_Id);
                        }
                    }
                    else
                    {
                        LogUtil.Error("Instance::Init, unknown part {0}", skill.Statements[i].GetId());
                    }
                }
            }
            else
            {
                LogUtil.Error("Instance::Init, isn't story DSL");
            }
            //LogSystem.Debug("Instance.Init message handler num:{0} {1}", m_MessageHandlers.Count, ret);
            return ret;
        }
        public void Reset()
        {
            m_IsTerminated = false;
            int ct = m_MessageHandlers.Count;
            for (int i = ct - 1; i >= 0; --i)
            {
                MessageHandler handler = m_MessageHandlers[i];
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
            foreach (KeyValuePair<string, object> pair in m_PreInitedLocalVariables)
            {
                m_LocalVariables.Add(pair.Key, pair.Value);
            }
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
            else
            {
                //忽略没有处理的消息
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
            foreach (MessageHandler handler in m_MessageHandlers)
            {
                string msgId = handler.MessageId;
                Queue<MessageInfo> queue;
                if (m_MessageQueues.TryGetValue(msgId, out queue))
                {
                    for (int msgCt = 0; msgCt < c_MaxMsgCountPerTick && queue.Count > 0; ++msgCt)
                    {
                        MessageInfo info = queue.Dequeue();
                        MessageHandler activeHandler = handler.Clone();
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
                MessageHandler handler = m_ActiveHandlers[ix];
                handler.Tick(this, dt);
                if (handler.IsFinished())
                    m_ActiveHandlers.RemoveAt(ix);

            }
        }
        public void Analyze()
        {
            foreach (MessageHandler handler in m_MessageHandlers)
            {
                handler.Analyze(this);
            }
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
        private Dictionary<string, object> m_GlobalVariables = null;

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
        private List<MessageHandler> m_ActiveHandlers = new List<MessageHandler>();
        private List<MessageHandler> m_MessageHandlers = new List<MessageHandler>();
        private List<ICommand> m_ParallelCommands = new List<ICommand>();
        private Dictionary<string, object> m_PreInitedLocalVariables = new Dictionary<string, object>();
    }
}
