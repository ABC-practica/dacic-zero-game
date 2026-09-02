using CombatSystem;
using EventBus;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class CombatStateManager : MonoBehaviour
    {
        private HashSet<int> idsAggroedEnemies = new();
        private void OnEnable()
        {
            EventBus<EnemyEnterCombat>.AddActions(0, AddEnemy);
            EventBus<EnemyExitCombat>.AddActions(0, RemoveEnemy);
            EventBus<CheckPointLoaded>.AddActions(0, actionNoArgs: ResetCombatState);
        }

        private void OnDisable()
        {
            EventBus<EnemyEnterCombat>.RemoveActions(0, AddEnemy);
            EventBus<EnemyExitCombat>.RemoveActions(0, RemoveEnemy);
            EventBus<CheckPointLoaded>.RemoveActions(0, actionNoArgs: ResetCombatState);
        }

        private void ResetCombatState()
        {
            if (idsAggroedEnemies.Count > 0)
            {
                EventBus<PlayerExitCombat>.Raise(0, new PlayerExitCombat());
                idsAggroedEnemies.Clear();
            }  
        }

        private void AddEnemy(EnemyEnterCombat @event)
        {
            if (idsAggroedEnemies.Count == 0)
            {
                EventBus<PlayerEnterCombat>.Raise(0, new PlayerEnterCombat());
            }

            idsAggroedEnemies.Add(@event.EnemyInstanceId);
        }

        private void RemoveEnemy(EnemyExitCombat @event)
        {
            idsAggroedEnemies.Remove(@event.EnemyInstanceId);
            if (idsAggroedEnemies.Count == 0)
            {
                EventBus<PlayerExitCombat>.Raise(0, new PlayerExitCombat());
            }
        }
    }

}
