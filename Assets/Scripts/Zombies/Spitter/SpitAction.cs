using Assets.Scripts.Zombies.Spitter;
using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;



namespace BBCore.Actions
{
    [Action("Spitter/Spit")]
    public class SpitAction : GOAction
    {
        [InParam("Spitter Zombie")]
        private SpitterZombie spitterZombie;
        private bool _hasSpitted = false;
        public override void OnStart()
        {
            _hasSpitted = false;

        }

        public override TaskStatus OnUpdate()
        {
            if (spitterZombie == null)
                return TaskStatus.FAILED;

            if (_hasSpitted && spitterZombie.IsSpittingFinished)
                return TaskStatus.COMPLETED;

            if (!_hasSpitted)
            {
                spitterZombie.Spit();
                _hasSpitted = true;
            }
            return TaskStatus.RUNNING;
        }
        public override void OnAbort()
        {

            base.OnAbort();
        }
        public override void OnEnd()
        {

            base.OnEnd();
        }
    }
}
