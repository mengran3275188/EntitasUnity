using System;
using Util;
using System.Collections.Generic;

namespace Skill.CommonCommands
{
  /// <summary>
  /// while($val<10)
  /// {
  ///   createnpc($$);
  ///   wait(100);
  /// };
  /// </summary>
  internal class WhileCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      WhileCommand retCmd = new WhileCommand();
      retCmd.m_Condition = m_Condition.Clone();
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
      m_Condition.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(SkillInstance instance)
    {
      m_Condition.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Blocked;
      while (ret == ExecResult.Blocked) {
        if (m_CommandQueue.Count == 0) {
          if (m_Condition.Value != 0) {
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
        if (callData.GetParamNum() > 0) {
          ScriptableData.ISyntaxComponent param = callData.GetParam(0);
          m_Condition.InitFromDsl(param);
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
    private ISkillValue<int> m_Condition = new SkillValue<int>();
    private Queue<ISkillCommand> m_CommandQueue = new Queue<ISkillCommand>();
    private List<ISkillCommand> m_LoadedCommands = new List<ISkillCommand>();
    private int m_CurCount = 0;
  }
}
