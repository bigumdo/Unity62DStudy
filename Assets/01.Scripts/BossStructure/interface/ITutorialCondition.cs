using System;
using YUI.Agents.players;

public interface ITutorialCondition
{
    void Initialize(Action onMet, Player player);
    void Dispose(); // ���� ���� ���� ��
}
