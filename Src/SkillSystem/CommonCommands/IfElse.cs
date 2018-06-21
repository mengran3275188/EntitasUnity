using System;
using Util;
using System.Collections.Generic;

namespace Skill.CommonCommands
{
  /// <summary>
  /// if(@val>0)
  /// {
  ///   createnpc(123);
  /// };
  /// 
  /// or
  /// 
  /// if(@val>0)
  /// {
  ///   createnpc(123);
  /// }
  /// else
  /// {
  ///   missioncomplete();
  /// };
  /// </summary>
  internal class IfElseCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      IfElseCommand retCmd = new IfElseCommand();
      retCmd.m_Condition = m_Condition.Clone();
      for (int i = 0; i < m_LoadedIfCommands.Count; i++)
      {
        retCmd.m_LoadedIfCommands.Add(m_LoadedIfCommands[i].Clone());
      }
      for (int i = 0; i < m_LoadedElseCommands.Count; i++)
      {
        retCmd.m_LoadedElseCommands.Add(m_LoadedElseCommands[i].Clone());
      }
      /*
      foreach (ISkillCommand cmd in m_LoadedIfCommands) {
        retCmd.m_LoadedIfCommands.Add(cmd.Clone());
      }
      foreach (ISkillCommand cmd in m_LoadedElseCommands) {
        retCmd.m_LoadedElseCommands.Add(cmd.Clone());
      }*/
      retCmd.IsCompositeCommand = true;
      return retCmd;
    }

    protected override void ResetState()
    {
      m_AlreadyExecute = false;
      foreach (ISkillCommand cmd in m_IfCommandQueue) {
        cmd.Reset();
      }
      m_IfCommandQueue.Clear();
      foreach (ISkillCommand cmd in m_ElseCommandQueue) {
        cmd.Reset();
      }
      m_ElseCommandQueue.Clear();
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Iterator = iterator;
      m_Arguments = args;
      m_Condition.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(SkillInstance instance)
    {
      m_Condition.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      ExecResult ret = ExecResult.Finished;
      if (m_IfCommandQueue.Count == 0 && m_ElseCommandQueue.Count==0 && !m_AlreadyExecute) {
        if (m_Condition.Value != 0) {
          PrepareIf();
          foreach (ISkillCommand cmd in m_IfCommandQueue) {
            cmd.Prepare(instance, m_Iterator, m_Arguments);
          }
          ret = ExecResult.Blocked;
        } else {
          PrepareElse();
          foreach (ISkillCommand cmd in m_ElseCommandQueue) {
            cmd.Prepare(instance, m_Iterator, m_Arguments);
          }
          ret = ExecResult.Blocked;
        }
        m_AlreadyExecute = true;
      } else {
        if (m_IfCommandQueue.Count > 0) {
          while (m_IfCommandQueue.Count > 0) {
            ISkillCommand cmd = m_IfCommandQueue.Peek();
            ExecResult result = cmd.Execute(instance, delta);
            if (result == ExecResult.Blocked)
            {
                break;
            }else if(result == ExecResult.Finished)
            {
                cmd.Reset();
                m_IfCommandQueue.Dequeue();
            }else if(result == ExecResult.Parallel)
            {
                instance.ParallelCommands.Add(m_IfCommandQueue.Dequeue());
            }
            else
            {
                LogUtil.Warn("Unknown skill exec result, skill {0}", instance.SkillId);
            }
          }
          ret = ExecResult.Blocked;
        }
        if (m_ElseCommandQueue.Count > 0) {
          while (m_ElseCommandQueue.Count > 0) {
            ISkillCommand cmd = m_ElseCommandQueue.Peek();
            ExecResult result = cmd.Execute(instance, delta);
            if (result == ExecResult.Blocked)
            {
                break;
            }else if(result == ExecResult.Finished)
            {
                cmd.Reset();
                m_ElseCommandQueue.Dequeue();
            }else if(result == ExecResult.Parallel)
            {
                instance.ParallelCommands.Add(m_ElseCommandQueue.Dequeue());
            }
            else
            {
                LogUtil.Warn("Unknown skill exec result, skill {0}", instance.SkillId);
            }
          }
          ret = ExecResult.Blocked;
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
            m_LoadedIfCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ISkillCommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if (null != cmd)
            m_LoadedIfCommands.Add(cmd);
        }*/
      }
    }

    protected override void Load(ScriptableData.StatementData statementData)
    {
      Load(statementData.First);
      ScriptableData.FunctionData functionData = statementData.Second;
      if (null != functionData && functionData.GetId() == "else") {
        for (int i = 0; i < functionData.Statements.Count; i++)
        {
          ISkillCommand cmd = SkillCommandManager.Instance.CreateCommand(functionData.Statements[i]);
          if (null != cmd)
            m_LoadedElseCommands.Add(cmd);
        }
        /*
        foreach (ScriptableData.ISyntaxComponent statement in functionData.Statements) {
          ISkillCommand cmd = StoryCommandManager.Instance.CreateCommand(statement);
          if (null != cmd)
            m_LoadedElseCommands.Add(cmd);
        }*/
      }
    }

    private void PrepareIf()
    {
      foreach (ISkillCommand cmd in m_IfCommandQueue) {
        cmd.Reset();
      }
      m_IfCommandQueue.Clear();
      foreach (ISkillCommand cmd in m_LoadedIfCommands) {
        m_IfCommandQueue.Enqueue(cmd);
      }
    }

    private void PrepareElse()
    {
      foreach (ISkillCommand cmd in m_ElseCommandQueue) {
        cmd.Reset();
      }
      m_ElseCommandQueue.Clear();
      foreach (ISkillCommand cmd in m_LoadedElseCommands) {
        m_ElseCommandQueue.Enqueue(cmd);
      }
    }

    private object m_Iterator = null;
    private object[] m_Arguments = null;
    private ISkillValue<int> m_Condition = new SkillValue<int>();
    private Queue<ISkillCommand> m_IfCommandQueue = new Queue<ISkillCommand>();
    private List<ISkillCommand> m_LoadedIfCommands = new List<ISkillCommand>();
    private Queue<ISkillCommand> m_ElseCommandQueue = new Queue<ISkillCommand>();
    private List<ISkillCommand> m_LoadedElseCommands = new List<ISkillCommand>();

    private bool m_AlreadyExecute = false;
  }
}
