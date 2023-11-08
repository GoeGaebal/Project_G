using UnityEngine;

namespace ObjectController.Character.Player
{
    public class PlayerLeg : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int DieAnimParam = Animator.StringToHash("die");
        private static readonly int WalkAnimParam = Animator.StringToHash("walk");
        private static readonly int HitAnimParam = Animator.StringToHash("hit");

        public void Init()
        {
            _animator = GetComponent<Animator>();
        }

        public void Walk(bool isWalk)
        {
            _animator.SetBool(WalkAnimParam, isWalk);
        }
        
        public void Die()
        {
            _animator.SetTrigger(DieAnimParam);
        }
    }
}
