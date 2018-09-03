using System;
using Util;
using System.Collections;
using System.Collections.Generic;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// foreach(v1,v2,v3)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class ForeachCommand : AbstractCommand
  {
    public override ICommand Clone()
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
      foreach (IValue<object> val in m_LoadedIterators) {
        retCmd.m_LoadedIterators.Add(val.Clone());
      }
      foreach (ICommand cmd in m_LoadedCommands) {
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
      foreach (IValue<object> val in m_LoadedIterators) {
        m_Iterators.Enqueue(val);
      }*/
      foreach (ICommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Arguments = args;
      foreach (IValue<object> val in m_Iterators) {
        val.Evaluate(iterator, args);
      }
    }

    protected override void UpdateVariables(IInstance instance)
    {
      foreach (IValue<object> val in m_Iterators) {
        val.Evaluate(instance);
      }
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while(ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Iterators.Count > 0) {
            Prepare();
            IValue<object> val = m_Iterators.Dequeue();
            foreach (ICommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, val.Value, m_Arguments);
            }
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
            ICommand cmd = m_CommandQueue.Peek();
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
                LogUtil.Warn("Unknown skill exec result, skill {0}", instance.Id);
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
          ICommand cmd = CommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ICommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if(null!=cmd)
            m_LoadedCommands.Add(cmd);
        }*/
      }
    }

    private void Prepare()
    {
      foreach (ICommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ICommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private Queue<IValue<object>> m_Iterators = new Queue<IValue<object>>();
    private Queue<ICommand> m_CommandQueue = new Queue<ICommand>();
    private List<IValue<object>> m_LoadedIterators = new List<IValue<object>>();
    private List<ICommand> m_LoadedCommands = new List<ICommand>();
  }
  /// <summary>
  /// looplist(list)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class LoopListCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      LoopListCommand retCmd = new LoopListCommand();
      retCmd.m_LoadedList = m_LoadedList.Clone();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
      }
      /*
      foreach (ICommand cmd in m_LoadedCommands) {
        retCmd.m_LoadedCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_Iterators.Clear();
      foreach (ICommand cmd in m_CommandQueue) {
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

    protected override void UpdateVariables(IInstance instance)
    {
      m_LoadedList.Evaluate(instance);
      TryUpdateValue();
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Iterators.Count > 0) {
            Prepare();
            object val = m_Iterators.Dequeue();
            foreach (ICommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, val, m_Arguments);
            }
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
            ICommand cmd = m_CommandQueue.Peek();
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
                LogUtil.Warn("Unknown skill exec result, skill {0}", instance.Id);
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
          ICommand cmd = CommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ICommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
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
      foreach (ICommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ICommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private Queue<object> m_Iterators = new Queue<object>();
    private Queue<ICommand> m_CommandQueue = new Queue<ICommand>();
    private IValue<IEnumerable> m_LoadedList = new SkillValue<IEnumerable>();
    private List<ICommand> m_LoadedCommands = new List<ICommand>();
  }
  /// <summary>
  /// loop(count)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class LoopCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      LoopCommand retCmd = new LoopCommand();
      retCmd.m_Count = m_Count.Clone();

      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        retCmd.m_LoadedCommands.Add(m_LoadedCommands[i].Clone());
      }
      /*
      foreach (ICommand cmd in m_LoadedCommands) {
        retCmd.m_LoadedCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_CurCount = 0;
      foreach (ICommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Arguments = args;
      m_Count.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Count.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_CurCount < m_Count.Value) {
            Prepare();
            foreach (ICommand cmd in m_CommandQueue) {
              cmd.Prepare(instance, m_CurCount, m_Arguments);
            }
            ++m_CurCount;
            ret = ExecResult.Blocked;
          } else {
            ret = ExecResult.Finished;
          }
        } else {
          while (m_CommandQueue.Count > 0) {
            ICommand cmd = m_CommandQueue.Peek();
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
                LogUtil.Warn("Unknown skill exec result, skill {0}", instance.Id);
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
          ICommand cmd = CommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ICommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if (null != cmd)
            m_LoadedCommands.Add(cmd);
        }*/
      }
    }

    private void Prepare()
    {
      foreach (ICommand cmd in m_CommandQueue) {
        cmd.Reset();
      }
      m_CommandQueue.Clear();

      for (int i = 0; i < m_LoadedCommands.Count; i++)
      {
        m_CommandQueue.Enqueue(m_LoadedCommands[i]);
      }
      /*
      foreach (ICommand cmd in m_LoadedCommands) {
        m_CommandQueue.Enqueue(cmd);
      }*/
    }

    private object[] m_Arguments = null;
    private IValue<int> m_Count = new SkillValue<int>();
    private Queue<ICommand> m_CommandQueue = new Queue<ICommand>();
    private List<ICommand> m_LoadedCommands = new List<ICommand>();

    private int m_CurCount = 0;
  }
}
