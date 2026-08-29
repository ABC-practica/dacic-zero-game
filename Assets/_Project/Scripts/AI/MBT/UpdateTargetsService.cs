using AI;
using CombatSystem;
using Detection;
using EventBus;
using UnityEngine;
namespace MBT {
    [AddComponentMenu("")]
    [MBTNode("Service/Update Targets Service")]
    public class UpdateTargetsService : Service {
        [SerializeField] protected DetectionSystem detectionSystem;
        [SerializeField] protected Vector3Reference position;
        [SerializeField] protected BoolReference hasPosition;
        [SerializeField] protected FloatReference targetAwareness;
        [SerializeField] protected TacticalBrain tacticalBrain;
        [SerializeField] protected bool lookAtTarget = true, lookAtSound = true;
        public override void Task() {
            if (detectionSystem) {
                bool pastHasPosition = hasPosition.Value;
                if (detectionSystem.ClosestTarget != null) {
                    //targets have priority over sounds; we will prioritize going
                    //after them over investigating noises
                    position.Value = detectionSystem.ClosestTarget.LastKnownPosition;
                    targetAwareness.Value = detectionSystem.ClosestTarget.Awareness;
                    hasPosition.Value = true;
                }
                else
                {
                    //reset target awareness if we do not have a target
                    targetAwareness.Value = 0;
                    if (tacticalBrain.RequestedPosition != null) {
                        position.Value = tacticalBrain.RequestedPosition.Value;
                        hasPosition.Value = true;
                    }
                    else if (detectionSystem.ClosestSound != null) {
                        position.Value = detectionSystem.ClosestSound.Value.Position;
                        detectionSystem.Eye.LookAt(position.Value);
                        hasPosition.Value = true;
                    }
                    else
                    {
                        hasPosition.Value = false;
                    }
                }

                // part of combat state system
                if (!detectionSystem.gameObject.GetComponent<IsCombatEnemy>())
                    return;

                if (!pastHasPosition && hasPosition.Value)
                {
                    EventBus<EnemyEnterCombat>.Raise(0, new EnemyEnterCombat(transform.parent.gameObject.GetInstanceID()));
                    return;
                }
                if (pastHasPosition && !hasPosition.Value)
                {
                    EventBus<EnemyExitCombat>.Raise(0, new EnemyExitCombat(transform.parent.gameObject.GetInstanceID()));
                    return;
                }
            }

            detectionSystem = GetComponentInParent<DetectionSystem>();
            if (detectionSystem == null)
            {
                Debug.LogError("BT (MBT.UpdateTargetsService) has no detection system set.");
            }
        }
    }
}
