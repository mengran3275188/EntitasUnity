using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Skill
{
    public sealed class SkillMessageHandler
    {

        public string MessageId
        {
            get { return m_MessageId; }
        }
        public SkillMessageHandler Clone()
        {
            SkillMessageHandler handler = new SkillMessageHandler();
            foreach (ISkillCommand cmd in m_LoadedCommands)
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
            foreach (ISkillCommand cmd in m_CommandQueue)
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
        public void Tick(SkillInstance instance, long delta)
        {
            while (m_CommandQueue.Count > 0)
            {
                ISkillCommand cmd = m_CommandQueue.Peek();
                ExecResult result = cmd.Execute(instance, delta);
                if (result == ExecResult.Blocked)
                {
                    break;
                }else if(result == ExecResult.Finished)
                {
                    cmd.Reset();
                    m_CommandQueue.Dequeue();
                }else if(result == ExecResult.Parallel)
                {
                    instance.ParallelCommands.Add(m_CommandQueue.Dequeue());
                }
                else
                {
                    LogUtil.Warn("Unknown skill exec result, skill {0}", instance.SkillId);
                }
            }
        }
        public void Analyze(SkillInstance instance)
        {
            foreach (ISkillCommand cmd in m_LoadedCommands)
            {
                cmd.Analyze(instance);
            }
        }
        public void Trigger(SkillInstance instance, object[] args)
        {
            Prepare();
            m_Arguments = args;
            foreach (ISkillCommand cmd in m_CommandQueue)
            {
                cmd.Prepare(instance, args.Length, args);
            }
        }
        private void RefreshCommands(ScriptableData.FunctionData handlerData)
        {
            m_LoadedCommands.Clear();
            foreach (var statement in handlerData.Statements)
            {
                ISkillCommand cmd = SkillCommandManager.Instance.CreateCommand(statement);
                if (null != cmd)
                    m_LoadedCommands.Add(cmd);
            }
        }

        private string m_MessageId = "";

        private Queue<ISkillCommand> m_CommandQueue = new Queue<ISkillCommand>();
        private object[] m_Arguments = null;
        private List<ISkillCommand> m_LoadedCommands = new List<ISkillCommand>();
    }
    public sealed class SkillInstance
    {

        public int SkillId
        {
            get { return m_SkillId; }
        }
        public object Sender
        {
            get { return m_Sender; }
            set { m_Sender = value; }
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
        public Dictionary<string, object> LocalVariables
        {
            get { return m_LocalVariables; }
        }
        public Dictionary<string, object> GlobalVariables
        {
            get { return m_GlobalVariables; }
            set { m_GlobalVariables = value; }
        }
        public List<ISkillCommand> ParallelCommands
        {
            get { return m_ParallelCommands; }
        }
        public SkillInstance Clone()
        {
            SkillInstance instance = new SkillInstance();
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
            instance.m_SkillId = m_SkillId;
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
                    m_SkillId = int.Parse(callData.GetParamId(0));
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
                            LogUtil.Error("Story {0} DSL, local must be a function !", m_SkillId);
                        }
                    }
                    else if (skill.Statements[i].GetId() == "onmessage")
                    {
                        ScriptableData.FunctionData sectionData = skill.Statements[i] as ScriptableData.FunctionData;
                        if (null != sectionData)
                        {
                            SkillMessageHandler handler = new SkillMessageHandler();
                            handler.Load(sectionData);
                            string msgId = handler.MessageId;
                            if (!m_MessageQueues.ContainsKey(msgId))
                            {
                                m_MessageHandlers.Add(handler);
                                m_MessageQueues.Add(msgId, new Queue<MessageInfo>());
                            }
                            else
                            {
                                LogUtil.Error("Story {0} DSL, onmessage {1} duplicate, discard it !", m_SkillId, msgId);
                            }
                        }
                        else
                        {
                            LogUtil.Error("Story {0} DSL, onmessage must be a function !", m_SkillId);
                        }
                    }
                    else
                    {
                        LogUtil.Error("SkillInstance::Init, unknown part {0}", skill.Statements[i].GetId());
                    }
                }
                /*
                foreach (ScriptableData.ISyntaxComponent info in story.Statements) {
                  if (info.GetId() == "local") {
                    ScriptableData.FunctionData sectionData = info as ScriptableData.FunctionData;
                    if (null != sectionData) {
                      foreach (ScriptableData.ISyntaxComponent def in sectionData.Statements) {
                        ScriptableData.CallData defData = def as ScriptableData.CallData;
                        if (null != defData && defData.HaveId() && defData.HaveParam()) {
                          string id = defData.GetId();
                          if (id.StartsWith("@") && !id.StartsWith("@@")) {
                            SkillValue val = new SkillValue();
                            val.InitFromDsl(defData.GetParam(0));
                            if (!m_PreInitedLocalVariables.ContainsKey(id)) {
                              m_PreInitedLocalVariables.Add(id, val.Value);
                            } else {
                              m_PreInitedLocalVariables[id] = val.Value;
                            }
                          }
                        }
                      }
                    } else {
                      LogSystem.Error("Story {0} DSL, local must be a function !", m_StoryId);
                    }
                  } else if (info.GetId() == "onmessage") {
                    ScriptableData.FunctionData sectionData = info as ScriptableData.FunctionData;
                    if (null != sectionData) {
                      StoryMessageHandler handler = new StoryMessageHandler();
                      handler.Load(sectionData);
                      m_MessageHandlers.Add(handler);
                    } else {
                      LogSystem.Error("Story {0} DSL, onmessage must be a function !", m_StoryId);
                    }
                  } else {
                    LogSystem.Error("SkillInstance::Init, unknown part {0}", info.GetId());
                  }
                }*/
            }
            else
            {
                LogUtil.Error("SkillInstance::Init, isn't story DSL");
            }
            //LogSystem.Debug("SkillInstance.Init message handler num:{0} {1}", m_MessageHandlers.Count, ret);
            return ret;
        }
        public void Reset()
        {
            m_IsTerminated = false;
            int ct = m_MessageHandlers.Count;
            for (int i = ct - 1; i >= 0; --i)
            {
                SkillMessageHandler handler = m_MessageHandlers[i];
                handler.Reset();
                Queue<MessageInfo> queue;
                if (m_MessageQueues.TryGetValue(handler.MessageId, out queue))
                {
                    queue.Clear();
                }
            }
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
            int ct = m_ParallelCommands.Count;
            for(int ix = ct - 1; ix >= 0; --ix)
            {
                ISkillCommand cmd = m_ParallelCommands[ix];
                if(cmd.Execute(this, delta) == ExecResult.Finished)
                {
                    cmd.Reset();
                    m_ParallelCommands.Remove(cmd);
                }
            }
            ct = m_ActiveHandlers.Count;
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                long dt = delta;
                SkillMessageHandler handler = m_ActiveHandlers[ix];
                handler.Tick(this, dt);
                if (handler.IsFinished())
                    m_ActiveHandlers.RemoveAt(ix);

            }
            foreach(SkillMessageHandler handler in m_MessageHandlers)
            {
                string msgId = handler.MessageId;
                Queue<MessageInfo> queue;
                if (m_MessageQueues.TryGetValue(msgId, out queue))
                {
                    for (int msgCt = 0; msgCt < c_MaxMsgCountPerTick && queue.Count > 0; ++msgCt)
                    {
                        MessageInfo info = queue.Dequeue();
                        SkillMessageHandler activeHandler = handler.Clone();
                        activeHandler.Trigger(this, info.m_Args);
                        m_ActiveHandlers.Add(activeHandler);
                    }
                }
            }
        }
        public void Analyze()
        {
            foreach (SkillMessageHandler handler in m_MessageHandlers)
            {
                handler.Analyze(this);
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

        private int m_SkillId = 0;
        private bool m_IsTerminated = false;
        private object m_Context = null;
        private object m_Sender = null;
        private object m_Target = null;
        private Dictionary<string, Queue<MessageInfo>> m_MessageQueues = new Dictionary<string, Queue<MessageInfo>>();
        private List<SkillMessageHandler> m_ActiveHandlers = new List<SkillMessageHandler>();
        private List<SkillMessageHandler> m_MessageHandlers = new List<SkillMessageHandler>();
        private List<ISkillCommand> m_ParallelCommands = new List<ISkillCommand>();
        private Dictionary<string, object> m_PreInitedLocalVariables = new Dictionary<string, object>();
    }
}
