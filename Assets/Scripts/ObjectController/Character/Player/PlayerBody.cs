using System;
using UnityEngine;

namespace ObjectController.Character.Player
{
    public class PlayerBody : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int DieAnimParam = Animator.StringToHash("die");
        private static readonly int WalkAnimParam = Animator.StringToHash("walk");
        private static readonly int HitAnimParam = Animator.StringToHash("hit");

        public Action OnFinishDie;
        public Action OnFinishHit;

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
        
        public void Hit()
        {
            _animator.SetTrigger(HitAnimParam);
        }

        private void FinishDieAnimClip()
        {
            OnFinishDie?.Invoke();
        }
        
        private void FinishHitAnimClip()
        {
            OnFinishHit?.Invoke();
        }
    }
}
