using Detection;
using PlayerController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Weapons;
namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Tasks/SpotPlayer")]
    public class SpotPlayer : Leaf
    {
        [SerializeField] Transform self;
        [SerializeField] ScannerBotComponent scanner;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] DetectionSystem detectionSystem;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override NodeResult Execute()
        {
            var target = detectionSystem.ClosestTarget ?? null;
            if (target == null) return NodeResult.failure;
            var targetTransform = target.Transform;
            var targetPos = targetTransform.position;
            self.LookAt(new Vector3(targetPos.x, self.position.y, targetPos.z));
            //if (scanner != null)
            //    scanner.AngularSpeed = 0;
            //if(agent != null)
            //{
            //    agent.angularSpeed = 0;
            //    agent.speed = 0;
            //}
            var stealthComponent = targetTransform.GetComponent<PlayerStealthController>();
            if (stealthComponent != null)
            {
                StartCoroutine(stealthComponent.GetSpottedBy(self));
                return NodeResult.running;
            }
            return NodeResult.failure;
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
