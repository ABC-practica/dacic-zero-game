using CombatSystem;
using EventBus;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class CombatStateManager : MonoBehaviour
    {
        private HashSet<int> idsAggroedEnemies = new();
        private bool isPlayerInCombat = false;
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
            Debug.Log("reset combat state");
            idsAggroedEnemies.Clear();
        }

        private void AddEnemy(EnemyEnterCombat @event)
        {
            Debug.Log("enemy enters combat");
            idsAggroedEnemies.Add(@event.EnemyInstanceId);
        }

        private void RemoveEnemy(EnemyExitCombat @event)
        {
            Debug.Log("enemy exits combat");
            idsAggroedEnemies.Remove(@event.EnemyInstanceId);
        }

        private void Update()
        {
            if (idsAggroedEnemies.Count > 0 && !isPlayerInCombat)
            {
                isPlayerInCombat = true;
                EventBus<PlayerEnterCombat>.Raise(0, new PlayerEnterCombat());
                return;
            }
            if (idsAggroedEnemies.Count == 0 && isPlayerInCombat)
            {
                isPlayerInCombat = false;
                EventBus<PlayerExitCombat>.Raise(0, new PlayerExitCombat());
            }
        }
    }

}
