using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBar : MonoBehaviour {
    public void OnSkillButtonClick()
    {
        UnityDelegate.GfxMoudle.Instance.PublishLogicEvent<int>("player_use_skill", "skill_system", 1);
    }
}
