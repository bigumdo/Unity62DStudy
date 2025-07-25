using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckCurrentHp", story: "[Boss] Health [ChekcHp] or less", category: "Conditions", id: "78a9e478cce4c4d90327913d36d42517")]
public partial class CheckCurrentHpCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<float> ChekcHp;

    public override bool IsTrue()
    {
        return !(Boss.Value.GetCompo<BossHealth>().GetCurrentHp() >= ChekcHp.Value);
    }
}
