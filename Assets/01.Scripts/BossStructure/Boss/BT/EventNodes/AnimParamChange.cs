using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimParamChange")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimParamChange", message: "Set to [Param]", category: "Events", id: "e128fe27d56c43e9dfafe20da94adb5b")]
public partial class AnimParamChange : EventChannelBase
{
    public delegate void AnimParamChangeEventHandler(string Param);
    public event AnimParamChangeEventHandler Event; 

    public void SendEventMessage(string Param)
    {
        Event?.Invoke(Param);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<string> ParamBlackboardVariable = messageData[0] as BlackboardVariable<string>;
        var Param = ParamBlackboardVariable != null ? ParamBlackboardVariable.Value : default(string);

        Event?.Invoke(Param);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AnimParamChangeEventHandler del = (Param) =>
        {
            BlackboardVariable<string> var0 = vars[0] as BlackboardVariable<string>;
            if(var0 != null)
                var0.Value = Param;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AnimParamChangeEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AnimParamChangeEventHandler;
    }
}

