using Assets.Scripts.Zombies.Spitter;
using Pada1.BBCore;

namespace BBUnity.Conditions
{
    [Condition("Spitter/IsSpitReady")]
    public class SpittingAttackCondition : GOCondition
    {

        [InParam("SpitterZombie")]
        private SpitterZombie spitterZombie;

        public override bool Check()
        {
            return spitterZombie.IsSpitReady();
        }
    }
}

