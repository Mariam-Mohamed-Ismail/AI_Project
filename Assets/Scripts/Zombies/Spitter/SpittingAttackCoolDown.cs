using BBCore.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using System.Threading;
using UnityEngine;

namespace BBUnity.Conditions
{
    [Condition("Perception/IsSpitReady")]
    public class SpittingAttackCoolDown : GOCondition
    {

        [InParam("SpittingAttackTimer")]
        private SpittingAttackTimer spittingAttaxkTimer;

        public override bool Check()
        {
            return spittingAttaxkTimer.IsSpitReady();
        }

       

    }
}

