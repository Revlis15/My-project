using UnityEngine;

public class Enemy_movement : MonoBehaviour
{
    public float speed;
    private int facingDir = 1;

    public float attackRange = 2;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private EnemyState enemyState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.Chasing && player != null)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attack)
        {

        }
    }

    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        const float turnThreshold = 0.01f;

        if ((direction.x > turnThreshold && facingDir == -1) ||
            (direction.x < -turnThreshold && facingDir == 1))
        {
            Flip();
        }

        // If close enough, switch to attack (flip already handled above)
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        rb.linearVelocity = direction * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(player == null)
            {
                player = collision.transform;
            }
            ChangeState(EnemyState.Chasing);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = null;
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    void Flip()
    {
        facingDir *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", false);
        }
        else if (enemyState == EnemyState.Attack)
        {
            anim.SetBool("isAttacking", false);
        }

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", true);
        }
        else if (enemyState == EnemyState.Attack)
        {
            anim.SetBool("isAttacking", true);
        }
    }
}


public enum EnemyState
{
    Idle,
    Chasing,
    Attack,
}