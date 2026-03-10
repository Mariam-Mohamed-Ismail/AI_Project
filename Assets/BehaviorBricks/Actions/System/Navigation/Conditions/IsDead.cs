using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Framework;

[Condition("Zombie/IsDead")]
public class IsDead : ConditionBase
{
    [InParam("zombie")] public GameObject zombie;

    private ZombieHealth health;

    public override bool Check()
    {
        if (zombie == null)
            return false;

        if (health == null)
            health = zombie.GetComponent<ZombieHealth>();

        return health != null && health.IsDead;
    }
}