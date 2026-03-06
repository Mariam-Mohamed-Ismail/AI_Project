using UnityEngine;
using UnityEngine.AI;

public class ZombieBrain : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public Health health;

    [Header("Ranges")]
    public float detectRange = 12f;
    public float attackRange = 2.2f;

    public bool IsDead => health != null && health.IsDead
        ;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponent<Animator>();
        if (!health) health = GetComponent<Health>();
    }

    public float DistanceToPlayer
    {
        get
        {
            if (!player) return Mathf.Infinity;
            return Vector3.Distance(transform.position, player.position);
        }
    }
}