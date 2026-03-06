using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;


namespace BBCore.Actions
{
    [Action("Zombies/Spit")]
    public class SpitAction : GOAction
    {
        [InParam("Bullet Prefab")]
        private GameObject bulletPrefab;
        [InParam("Fire Point")]
        private Transform firePoint;
        [InParam("Bullet Speed")]
        private float bulletSpeed = 10f;
        [InParam("Spitting Attack CoolDown")]
        private SpittingAttackTimer spittingAttackTimer;
        private Animator _animator;
        public override void OnStart()
        {
            _animator = gameObject.GetComponent<Animator>();
            if (_animator != null)
                _animator.SetInteger("state", (int)ZombieState.Attacking);
        }

        public override TaskStatus OnUpdate()
        {
            if (bulletPrefab == null)
                return TaskStatus.FAILED;
            if (firePoint == null)
                return TaskStatus.FAILED;

            Spit();

            return TaskStatus.COMPLETED;
        }
        private void Spit()
        {
            GameObject bullet = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            else
                bullet.AddComponent<Rigidbody>().linearVelocity = firePoint.forward * bulletSpeed;

            spittingAttackTimer.ResetCoolDown();
        }

        public override void OnAbort()
        {
            base.OnAbort();
        }

        public override void OnEnd()
        {
            if (_animator != null)
                _animator.SetInteger("state", (int)ZombieState.Idle);
            base.OnEnd();
        }
    }
}
