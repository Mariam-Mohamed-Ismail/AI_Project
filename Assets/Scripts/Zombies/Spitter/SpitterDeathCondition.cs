using Assets.Scripts.Zombies.Spitter;
using Pada1.BBCore;


namespace BBUnity.Conditions
{
    [Condition("Spitter/IsSpitterDead")]
    public class SpitterDeathCondition : GOCondition
    {
        [InParam("SpitterZombieHealth")]
        private SpitterZombie spitterZombie;
        public override bool Check()
        {
            return spitterZombie.IsDead;
        }
    }
}
