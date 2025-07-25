using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Animators;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAnimatorBoolAction", story: "Set [CurrentAnim] in [Animator] to [Value]", category: "Action", id: "832d8a2076fdaf2b486294ecb7d20320")]
public partial class SetAnimatorBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<string> CurrentAnim;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        if(CurrentAnim == null || Animator == null)
        {
            return Status.Failure;
        }
        Animator.Value.SetBool(CurrentAnim.Value, Value.Value);
        return Status.Success;
    }

}

