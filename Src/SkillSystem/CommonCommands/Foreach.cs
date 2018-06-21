using System;
using Util;
using System.Collections;
using System.Collections.Generic;

namespace Skill.CommonCommands
{
  /// <summary>
  /// foreach(v1,v2,v3)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class ForeachCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      ForeachCommand retCmd = new ForeachCommand();
      for (int i = 0; i < m_LoadedIterators.Count; i++ )
      {
        retCmd.m_LoadedIterators.Add(m_LoadedIterators[i].Clone());
      }
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
      }
      /*
      foreach (ISkillValue<object> val in m_LoadedIterators) {
        retCmd.m_LoadedIterators.Add(val.Clone());
      }
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        retCmd.m_LoadedCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_Iterators.Clear();
      for (int i = 0; i < m_LoadedIterators.Count; i++)
      {
        m_Iterators.Enqueue(m_LoadedIterators[i]);
      }
      /*
      foreach (ISkillValue<object> val in m_LoadedIterators) {
        m_Iterators.Enqueue(val);
      }*/
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Arguments = args;
      foreach (ISkillValue<object> val in m_Iterators) {
        val.Evaluate(iterator, args);
      }
    }

    protected override void UpdateVariables(SkillInstance instance)
    {
      foreach (ISkillValue<object> val in m_Iterators) {
        val.Evaluate(instance);
      }
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while(ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Iterators.Count > 0) {
            Prepare();
            ISkillValue<object> val = m_Iterators.Dequeue();
            foreach (ISkillCommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, val.Value, m_Arguments);
            }
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
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
          ret = ExecResult.Blocked;
          if (m_CommandQueue.Count > 0) {
            break;
          }
        }
      }
      return ret;
    }

    protected override void Load(ScriptableData.FunctionData functionData)
    {
      ScriptableData.CallData callData = functionData.Call;
      if (null != callData) {
        for (int i = 0; i < callData.GetParamNum(); ++i) {
          ScriptableData.ISyntaxComponent param = callData.GetParam(i);
          SkillValue val = new SkillValue();
          val.InitFromDsl(param);
          m_LoadedIterators.Add(val);
          m_Iterators.Enqueue(val);
        }
        for (int i = 0; i < functionData.Statements.Count; i++)
        {
          ISkillCommand cmd = SkillCommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ISkillCommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if(null!=cmd)
            m_LoadedCommands.Add(cmd);
        }*/
      }
    }

    private void Prepare()
    {
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private Queue<ISkillValue<object>> m_Iterators = new Queue<ISkillValue<object>>();
    private Queue<ISkillCommand> m_CommandQueue = new Queue<ISkillCommand>();
    private List<ISkillValue<object>> m_LoadedIterators = new List<ISkillValue<object>>();
    private List<ISkillCommand> m_LoadedCommands = new List<ISkillCommand>();
  }
  /// <summary>
  /// looplist(list)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class LoopListCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      LoopListCommand retCmd = new LoopListCommand();
      retCmd.m_LoadedList = m_LoadedList.Clone();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        retCmd.m_LoadedCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_Iterators.Clear();
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Arguments = args;
      m_LoadedList.Evaluate(iterator, args);
      TryUpdateValue();
    }

    protected override void UpdateVariables(SkillInstance instance)
    {
      m_LoadedList.Evaluate(instance);
      TryUpdateValue();
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Iterators.Count > 0) {
            Prepare();
            object val = m_Iterators.Dequeue();
            foreach (ISkillCommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, val, m_Arguments);
            }
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
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
          ret = ExecResult.Blocked;
          if (m_CommandQueue.Count > 0) {
            break;
          }
        }
      }
      return ret;
    }

    protected override void Load(ScriptableData.FunctionData functionData)
    {
      ScriptableData.CallData callData = functionData.Call;
      if (null != callData) {
        if (callData.GetParamNum() > 0) {
          m_LoadedList.InitFromDsl(callData.GetParam(0));
          TryUpdateValue();
        }
        for (int i = 0; i < functionData.Statements.Count; i++)
        {
          ISkillCommand cmd = SkillCommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ISkillCommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }*/
      }
    }

    private void TryUpdateValue()
    {
      if (m_LoadedList.HaveValue && m_Iterators.Count == 0) {
        foreach (object obj in m_LoadedList.Value) {
          m_Iterators.Enqueue(obj);
        }
      }
    }

    private void Prepare()
    {
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private Queue<object> m_Iterators = new Queue<object>();
    private Queue<ISkillCommand> m_CommandQueue = new Queue<ISkillCommand>();
    private ISkillValue<IEnumerable> m_LoadedList = new SkillValue<IEnumerable>();
    private List<ISkillCommand> m_LoadedCommands = new List<ISkillCommand>();
  }
  /// <summary>
  /// loop(count)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class LoopCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      LoopCommand retCmd = new LoopCommand();
      retCmd.m_Count = m_Count.Clone();

      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        retCmd.m_LoadedCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_CurCount = 0;
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Arguments = args;
      m_Count.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(SkillInstance instance)
    {
      m_Count.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_CurCount < m_Count.Value) {
            Prepare();
            foreach (ISkillCommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, m_CurCount, m_Arguments);
            }
            ++m_CurCount;
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
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
          ret = ExecResult.Blocked;
          if (m_CommandQueue.Count > 0) {
            break;
          }
        }
      }
      return ret;
    }

    protected override void Load(ScriptableData.FunctionData functionData)
    {
      ScriptableData.CallData callData = functionData.Call;
      if (null != callData) {
        if(callData.GetParamNum()>0) {
          ScriptableData.ISyntaxComponent param = callData.GetParam(0);
          m_Count.InitFromDsl(param);
        }

        for (int i = 0; i < functionData.Statements.Count; i++)
        {
          ISkillCommand cmd = SkillCommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ISkillCommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }*/
      }
    }

    private void Prepare()
    {
      foreach (ISkillCommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();

      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private ISkillValue<int> m_Count = new SkillValue<int>();
    private Queue<ISkillCommand> m_CommandQueue = new Queue<ISkillCommand>();
    private List<ISkillCommand> m_LoadedCommands = new List<ISkillCommand>();

    private int m_CurCount = 0;
  }
}
