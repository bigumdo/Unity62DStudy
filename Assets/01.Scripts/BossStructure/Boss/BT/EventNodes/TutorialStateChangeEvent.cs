using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TutorialStateChangeEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TutorialStateChangeEvent", message: "[TutorialBossState]", category: "Events", id: "d4afb9ac518c7ad11468e7defbb14972")]
public sealed partial class TutorialStateChangeEvent : EventChannel<TutorialBossStateEnum> { }

