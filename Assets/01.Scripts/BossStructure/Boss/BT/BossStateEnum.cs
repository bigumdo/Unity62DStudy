using System;
using Unity.Behavior;

[BlackboardEnum]
public enum BossStateEnum
{
    Phase1,
	Phase2,
	FinalPhase,
    Transition
}

[BlackboardEnum]
public enum TutorialBossStateEnum
{
    None,
    Attack1,
    Attack2,
    Counter,
    Dead
}
