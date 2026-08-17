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
        [SerializeField] DetectionSystem detectionSystem;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override NodeResult Execute()
        {
            var target = detectionSystem.ClosestTarget.Transform;
            var targetPos = target.position;
            self.LookAt(new Vector3(targetPos.x, self.position.y, targetPos.z));
            scanner.AngularSpeed = 0;
            var stealthComponent = target.GetComponent<PlayerStealthController>();
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
