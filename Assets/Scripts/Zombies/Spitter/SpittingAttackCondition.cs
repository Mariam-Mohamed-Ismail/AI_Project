using Pada1.BBCore;
using Zombies.Spitter;

namespace BBUnity.Conditions
{
    [Condition("Perception/IsSpitReady")]
    public class SpittingAttackCondition : GOCondition
    {

        [InParam("SpittingAttackTimer")]
        private SpittingAttackTimer spittingAttaxkTimer;

        public override bool Check()
        {
            return spittingAttaxkTimer.IsSpitReady();
        }
    }
}

