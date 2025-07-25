using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/PhaseChangeEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "PhaseChangeEvent", message: "Boss [ChangeState]", category: "Events", id: "e86183d09e074697604b79b5d1e98423")]
public partial class PhaseChangeEvent : EventChannelBase
{
    public delegate void PhaseChangeEventEventHandler(BossStateEnum ChangeState);
    public event PhaseChangeEventEventHandler Event; 

    public void SendEventMessage(BossStateEnum ChangeState)
    {
        Event?.Invoke(ChangeState);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<BossStateEnum> ChangeStateBlackboardVariable = messageData[0] as BlackboardVariable<BossStateEnum>;
        var ChangeState = ChangeStateBlackboardVariable != null ? ChangeStateBlackboardVariable.Value : default(BossStateEnum);

        Event?.Invoke(ChangeState);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        PhaseChangeEventEventHandler del = (ChangeState) =>
        {
            BlackboardVariable<BossStateEnum> var0 = vars[0] as BlackboardVariable<BossStateEnum>;
            if(var0 != null)
                var0.Value = ChangeState;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as PhaseChangeEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as PhaseChangeEventEventHandler;
    }
}

