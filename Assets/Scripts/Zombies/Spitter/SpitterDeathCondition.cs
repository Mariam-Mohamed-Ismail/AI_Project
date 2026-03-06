using Pada1.BBCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zombies.Spitter;

namespace BBUnity.Conditions
{
    [Condition("Perception/IsSpitterDead")]
    internal class SpitterDeathCondition : GOCondition
    {
        [InParam("SpitterZombieHealth")]
        private SpitterZombieHealth spitterZombieHealth;
        public override bool Check()
        {
            return spitterZombieHealth.IsDead;
        }
    }
}
