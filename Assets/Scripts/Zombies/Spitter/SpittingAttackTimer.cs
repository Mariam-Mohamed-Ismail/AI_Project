using Pada1.BBCore;
using UnityEngine;


namespace Zombies.Spitter
{
    public class SpittingAttackTimer : MonoBehaviour
    {

        [SerializeField] private float coolDownTime;

        private float lastSpitTime = 0;


        public bool IsSpitReady()
        {
            return Time.time >= lastSpitTime + coolDownTime;
        }
        public void ResetCoolDown()
        {
            lastSpitTime = Time.time;
        }
    }
}
