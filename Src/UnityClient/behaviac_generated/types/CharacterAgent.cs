﻿// -------------------------------------------------------------------------------
// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
// -------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

///<<< BEGIN WRITING YOUR CODE FILE_INIT
using UnityEngine;

///<<< END WRITING YOUR CODE

public class CharacterAgent : behaviac.Agent
///<<< BEGIN WRITING YOUR CODE CharacterAgent
///<<< END WRITING YOUR CODE
{
	public bool CanUseSkill()
	{
///<<< BEGIN WRITING YOUR CODE CanUseSkill
        return !IsSkillActivite() && !IsBuffActivite();
///<<< END WRITING YOUR CODE
	}

	public float DistanceToPlayer()
	{
///<<< BEGIN WRITING YOUR CODE DistanceToPlayer
        var context = Contexts.sharedInstance.game;
        var mainPlayer = context.mainPlayerEntity;
        var self = GetOwner();
        float dis = Vector3.Distance(mainPlayer.position.Value, self.position.Value);
        return dis;
///<<< END WRITING YOUR CODE
	}

	public void EscapeFromPlayer()
	{
///<<< BEGIN WRITING YOUR CODE EscapeFromPlayer
        var context = Contexts.sharedInstance.game;
        var mainPlayer = context.mainPlayerEntity;
        var self = GetOwner();

        if (IsSkillActivite() || IsBuffActivite())
            return;

        float dir = Mathf.Atan2(self.position.Value.x - mainPlayer.position.Value.x, self.position.Value.z - mainPlayer.position.Value.z);
        Vector3 force = new Vector3(self.position.Value.x - mainPlayer.position.Value.x, 0, self.position.Value.z - mainPlayer.position.Value.z);
        self.ReplaceRotation(dir);
        self.ReplaceMovement(force.normalized * self.attr.Value.MoveSpeed);
        self.physics.Rigid.Velocity = force;
///<<< END WRITING YOUR CODE
	}

	public void FaceToTarget()
	{
///<<< BEGIN WRITING YOUR CODE FaceToTarget
        var context = Contexts.sharedInstance.game;
        var mainPlayer = context.mainPlayerEntity;
        var self = GetOwner();

        if (IsSkillActivite() || IsBuffActivite())
            return ;

        float dir = Mathf.Atan2(mainPlayer.position.Value.x - self.position.Value.x, mainPlayer.position.Value.z - self.position.Value.z);
        self.ReplaceRotation(dir);
///<<< END WRITING YOUR CODE
	}

	public behaviac.EBTStatus MoveToPlayer()
	{
///<<< BEGIN WRITING YOUR CODE MoveToPlayer
        var context = Contexts.sharedInstance.game;
        var mainPlayer = context.mainPlayerEntity;
        var self = GetOwner();

        if (IsSkillActivite() || IsBuffActivite())
            return behaviac.EBTStatus.BT_SUCCESS;

        float dir = Mathf.Atan2(mainPlayer.position.Value.x- self.position.Value.x, mainPlayer.position.Value.z - self.position.Value.z);
        Vector3 force = new Vector3(mainPlayer.position.Value.x- self.position.Value.x, 0, mainPlayer.position.Value.z - self.position.Value.z);
        self.ReplaceRotation(dir);
        self.ReplaceMovement(force.normalized * self.attr.Value.MoveSpeed);
		return behaviac.EBTStatus.BT_SUCCESS;
///<<< END WRITING YOUR CODE
	}

	public void PlayAnimation(string animName)
	{
///<<< BEGIN WRITING YOUR CODE PlayAnimation

        var self = GetOwner();

        UnityClient.Services.Instance.ViewService.PlayAnimation(self, animName);
///<<< END WRITING YOUR CODE
	}

	public void StopMove()
	{
///<<< BEGIN WRITING YOUR CODE StopMove
        {
            var self = GetOwner();
            self.ReplaceMovement(Vector3.zero);
        }
///<<< END WRITING YOUR CODE
	}

	public void UseSkill(int SkillId)
	{
///<<< BEGIN WRITING YOUR CODE UseSkill
        GameEntity self = GetOwner();
        if (!CanUseSkill())
            return;
        UnityClient.SkillSystem.Instance.StartSkill(self, self, SkillId, -1, self.position.Value, self.rotation.Value);
///<<< END WRITING YOUR CODE
	}

///<<< BEGIN WRITING YOUR CODE CLASS_PART

        public void Init(uint id)
    {
        m_EntityId = id;
    }
    private GameEntity GetOwner()
    {
        return Contexts.sharedInstance.game.GetEntityWithId(m_EntityId);
    }
    private bool IsSkillActivite()
    {
        GameEntity self = GetOwner();
        return self.hasSkill && null != self.skill.Instance;
    }
    private bool IsBuffActivite()
    {
        GameEntity self = GetOwner();
        foreach(var pair in self.buff.InstanceInfos)
        {
            if (pair.Value.Count > 0)
                return true;
        }
        return false;
    }
    private uint m_EntityId;

///<<< END WRITING YOUR CODE

}

///<<< BEGIN WRITING YOUR CODE FILE_UNINIT

///<<< END WRITING YOUR CODE

