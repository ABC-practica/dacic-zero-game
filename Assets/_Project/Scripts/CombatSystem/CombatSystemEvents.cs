using EventBus;
using UnityEngine;


namespace CombatSystem
{
    public struct EnemyEnterCombat : IEvent
    {
        public int EnemyInstanceId { get; }
        public EnemyEnterCombat(int instanceId)
        {
            EnemyInstanceId = instanceId;
        }
    }

    public struct EnemyExitCombat : IEvent
    {
        public int EnemyInstanceId { get; }
        public EnemyExitCombat(int instanceId)
        {
            EnemyInstanceId = instanceId;
        }
    }

    public struct PlayerEnterCombat : IEvent
    {

    }

    public struct PlayerExitCombat : IEvent
    {

    }
}