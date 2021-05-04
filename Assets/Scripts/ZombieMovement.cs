using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class ZombieMovement : MonoBehaviour
{
    [Header("AI config")]
    public float moveRadius = 10;
    public float standbyRadius = 15;
    public float attackRadius = 3;
    public int viewAngle = 90;

    [Header("Gameplay config")]
    public float attackRate = 1f;
    public int health = 100;
    public int damage = 5;

    ZombieState activeState;

    Animator animator;
    AIPath aiPath;
    AIDestinationSetter aiDestinationSetter;


    float nextAttack;
    float distanceToPlayer;
    Player player;

    bool isDead = false;

    Transform startTransform;

    public Action HealthChanged = delegate { };


    enum ZombieState
    {
        STAND,
        RETURN,
        MOVE_TO_PLAYER,
        ATTACK
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    void Start()
    {
        player = Player.Instance;
        print(player);

        ChangeState(ZombieState.STAND);

        player.OnDeath += PlayerDied;

        GameObject startPosGO = new GameObject(name + "_startPosition");
        startPosGO.transform.position = transform.position;
        startTransform = startPosGO.transform;
    }

    private void PlayerDied()
    {
        ChangeState(ZombieState.RETURN);
    }

    //public void UpdateHealth(int amount)
    //{
    //    health += amount;
    //    if (health <= 0)
    //    {
    //        isDead = true;
    //        Destroy(gameObject);

    //        player.OnDeath -= PlayerDied;
    //    }
    //    HealthChanged();
    //}


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Bullet bullet = collision.GetComponent<Bullet>();
    //    UpdateHealth(-bullet.damage);
    //}
    //када игрок бьет зомби

   
    void Update()
    {
        if (isDead)
        {
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (activeState)
        {
            case ZombieState.STAND:
                DoStand();
                break;
            case ZombieState.RETURN:
                DoReturn();
                break;
            case ZombieState.MOVE_TO_PLAYER:
                DoMove();
                break;
            case ZombieState.ATTACK:
                DoAttack();
                break;
        }
    }

    private void ChangeState(ZombieState newState)
    {
        switch (newState)
        {
            case ZombieState.STAND:
                aiPath.enabled = false;
                break;
            case ZombieState.RETURN:
                aiDestinationSetter.target = startTransform;
                aiPath.enabled = true;
                break;
            case ZombieState.MOVE_TO_PLAYER:
                aiPath.enabled = true;
                aiDestinationSetter.target = player.transform;
                //Play move sound
                break;
            case ZombieState.ATTACK:
                aiPath.enabled = false;
                break;
        }
        activeState = newState;
    }

    private void DoStand()
    {
        if (!player.isDead)
        {
            CheckMoveToPlayer();
        }
    }

    private void DoReturn()
    {
        if (!player.isDead && CheckMoveToPlayer())
        {
            return;
        }

        float distanceToStart = Vector3.Distance(transform.position, startTransform.position);
        if (distanceToStart <= 0.05f)
        {
            ChangeState(ZombieState.STAND);
            return;
        }
    }

    private bool CheckMoveToPlayer()
    {
        //проверям радиус
        if (distanceToPlayer > moveRadius)
        {
            return false;
        }


        //проверям препятствия
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Debug.DrawRay(transform.position, directionToPlayer, Color.red);

        float angle = Vector3.Angle(transform.forward, directionToPlayer); //was changed
        if (angle > viewAngle / 2)
        {
            return false;
        }

        LayerMask layerMask = LayerMask.GetMask("Obstacles");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, directionToPlayer.magnitude, layerMask);
        if (hit.collider != null)
        {
            //есть коллайдер
            return false;
        }


        //бежать за игроком
        ChangeState(ZombieState.MOVE_TO_PLAYER);
        return true;
    }

    private void DoMove()
    {
        if (distanceToPlayer < attackRadius)
        {
            ChangeState(ZombieState.ATTACK);
            animator.SetFloat("Speed", 0);
            return;
        }
        if (distanceToPlayer > standbyRadius)
        {
            ChangeState(ZombieState.RETURN);
            animator.SetFloat("Speed", 0);
            return;
        }


        animator.SetFloat("Speed", 1);
    }
    private void DoAttack()
    {
        if (distanceToPlayer > attackRadius)
        {
            ChangeState(ZombieState.MOVE_TO_PLAYER);
            StopAllCoroutines();
            return;
        }

        nextAttack -= Time.deltaTime;
        if (nextAttack <= 0)
        {
            animator.SetTrigger("Attack");

            nextAttack = attackRate;
        }
    }

    public void DamageToPlayer()
    {
        if (distanceToPlayer > attackRadius)
        {
            return;
        }
        player.UpdateHealth(-damage);
    }

    //IEnumerator AttackCoroutine()
    //{
    //    while (true)
    //    {
    //        animator.SetTrigger("Shoot");
    //        player.UpdateHealth(-damage);
    //        yield return new WaitForSeconds(attackRate);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, moveRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, standbyRadius);


        Gizmos.color = Color.cyan;
        Vector3 lookDirection = transform.forward;//was chanched
        Vector3 leftViewVector = Quaternion.AngleAxis(viewAngle / 2, Vector3.up) * lookDirection;
        Vector3 rightViewVector = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up) * lookDirection;

        Gizmos.DrawRay(transform.position, leftViewVector * moveRadius);
        Gizmos.DrawRay(transform.position, rightViewVector * moveRadius);
    }
}
