using System;
using Util;
using System.Collections.Generic;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// while($val<10)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class WhileCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      WhileCommand retCmd = new WhileCommand();
      retCmd.m_Condition = m_Condition.Clone();
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
      m_Condition.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Condition.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Condition.Value != 0) {
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
        if (callData.GetParamNum() > 0) {
          ScriptableData.ISyntaxComponent param = callData.GetParam(0);
          m_Condition.InitFromDsl(param);
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
    private IValue<int> m_Condition = new SkillValue<int>();
    private Queue<ICommand> m_CommandQueue = new Queue<ICommand>();
    private List<ICommand> m_LoadedCommands = new List<ICommand>();
    private int m_CurCount = 0;
  }
}
