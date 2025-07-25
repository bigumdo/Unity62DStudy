using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/BossHit")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "BossHit", message: "[Boss] [BossHealth]", category: "Events", id: "2872b0ed9460c8425716077de42a7ff7")]
public partial class BossHit : EventChannelBase
{
    public delegate void BossHitEventHandler(Boss Boss, BossHealth BossHealth);
    public event BossHitEventHandler Event; 

    public void SendEventMessage(Boss Boss, BossHealth BossHealth)
    {
        Event?.Invoke(Boss, BossHealth);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<Boss> BossBlackboardVariable = messageData[0] as BlackboardVariable<Boss>;
        var Boss = BossBlackboardVariable != null ? BossBlackboardVariable.Value : default(Boss);

        BlackboardVariable<BossHealth> BossHealthBlackboardVariable = messageData[1] as BlackboardVariable<BossHealth>;
        var BossHealth = BossHealthBlackboardVariable != null ? BossHealthBlackboardVariable.Value : default(BossHealth);

        Event?.Invoke(Boss, BossHealth);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        BossHitEventHandler del = (Boss, BossHealth) =>
        {
            BlackboardVariable<Boss> var0 = vars[0] as BlackboardVariable<Boss>;
            if(var0 != null)
                var0.Value = Boss;

            BlackboardVariable<BossHealth> var1 = vars[1] as BlackboardVariable<BossHealth>;
            if(var1 != null)
                var1.Value = BossHealth;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as BossHitEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as BossHitEventHandler;
    }
}

