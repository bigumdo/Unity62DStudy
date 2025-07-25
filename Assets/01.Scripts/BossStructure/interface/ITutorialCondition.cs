using System;
using YUI.Agents.players;

public interface ITutorialCondition
{
    void Initialize(Action onMet, Player player);
    void Dispose(); // 조건 구독 해제 등
}
