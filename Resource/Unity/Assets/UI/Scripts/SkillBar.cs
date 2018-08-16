using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

public class SkillBar : MonoBehaviour {
    public void OnSkillButtonClick(int code)
    {
        UnityDelegate.GfxMoudle.Instance.PublishLogicEvent<Keyboard.Code>("player_use_skill", "skill_system", (Keyboard.Code)code);
    }
}
