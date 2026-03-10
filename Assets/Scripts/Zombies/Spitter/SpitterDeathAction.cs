using Assets.Scripts.Zombies.Spitter;
using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;


namespace BBCore.Actions
{
    [Action("Spitter/SpitterDeath")]
    public class SpitterDeathAction : GOAction
    {
        [InParam("Spitter Zombie")]
        private SpitterZombie spitterZombie;


        public override void OnStart()
        {
            if (spitterZombie == null)
                return;
        }

        public override TaskStatus OnUpdate()
        {
            if (spitterZombie == null)
                return TaskStatus.FAILED;

            spitterZombie.Die();

            return TaskStatus.COMPLETED;
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
