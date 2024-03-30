using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour, IHitable
{
    [SerializeField] int maxHealth;
    [SerializeField] protected Transform targetPos;

    [Header("Shooting Properties")]
    [SerializeField] IntervalRange interval = new IntervalRange(1.5f, 2.7f);
    [SerializeField] float shootAccuracy = 0.5f;
    [SerializeField] ParticleSystem shotFX;

    private int currentHealth;
    private Transform player;
    private bool isDead;
    protected NavMeshAgent agent;
    private ShootOutPoint shootOutPoint;
    private Animator anim;
    private Vector3 movementLocal;
   


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();

        agent.updatePosition = true;
       agent.updateRotation = true;
    }

    public void Init(ShootOutPoint point)
    {
        currentHealth = maxHealth;
        shootOutPoint = point;

        if (agent != null)
        {
            BehaviorSetup();
        }
    }

    protected virtual void BehaviorSetup()
    {
        agent.SetDestination(targetPos.position);
        StartCoroutine(Shoot());
        GameManager.Instance.RegisterEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && !isDead)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            transform.rotation = Quaternion.LookRotation(direction);
        }
        RunBlend();
    }

    void RunBlend()
    {
        if (anim == null || !anim.enabled)
        {
            return;
        }
       
        if (agent.remainingDistance > 0.1f)
        {

            movementLocal = Vector3.Lerp(movementLocal, transform.InverseTransformDirection(agent.velocity).normalized, 2f * Time.deltaTime);

            agent.nextPosition = transform.position;

          

        }
        else
        {
            movementLocal = Vector3.Lerp(movementLocal, Vector3.zero, 2f * Time.deltaTime);
           
        }
        anim.SetFloat("X Speed", movementLocal.x);
        anim.SetFloat("Z Speed", movementLocal.z);

    }

    public void Hit(RaycastHit hit, int damageValue)
    {
        if (isDead)
            return;

        currentHealth--;
        Debug.Log("Enemy Shot!");

        if (currentHealth <= 0)
        {
            isDead = true;
            agent.enabled = false;
            DeadBehaviour();
            anim.SetTrigger("Dead");
            anim.SetBool("Is Dead", true);
           
            Destroy(gameObject,4f);

        }


        else
        {
            anim.SetTrigger("Shot");
        }
        
    }

    protected virtual void DeadBehaviour()
    {
        shootOutPoint.EnemyKilled();
        StopShooting();
        GameManager.Instance.EnemyKilled();
    }


    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => { return agent.remainingDistance < 0.02f; });

        while (!isDead)
        {
            if (GameManager.Instance.PlayerDead)
            StopShooting();

            shotFX.transform.rotation = Quaternion.LookRotation(transform.forward + Random.insideUnitSphere * 0.1f);
            if(Random.Range(0f, 1f)< shootAccuracy)
            {
                shotFX.transform.rotation = Quaternion.LookRotation(player.position - shotFX.transform.position);

                GameManager.Instance.PlayerHit(1f);
                Debug.Log("Player has been hit");
            }
            shotFX.Play();
          yield return new WaitForSeconds(interval.GetValue);
        }
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }
}
[System.Serializable]
public struct IntervalRange {

    [SerializeField] float min, max;

    public IntervalRange(float min, float max)
    {
        this.min = min;
        this.max = max;


    }

    public float GetValue { get => Random.Range(min, max); }


        }