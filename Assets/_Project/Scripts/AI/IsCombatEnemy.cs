using CombatSystem;
using UnityEngine;
using EventBus;

//Just put this component on any enemy that participates in combat
public class IsCombatEnemy : MonoBehaviour
{
    private void OnDisable()
    {
        EventBus<EnemyExitCombat>.Raise(0, new EnemyExitCombat(gameObject.GetInstanceID()));
    }
}
