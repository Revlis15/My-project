using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockback;
    public bool isShooting;
    public Player_combat player_Combat;

    private void Update()
    {
        if (Input.GetButtonDown("Slash") && player_Combat.enabled == true)
        {
            player_Combat.Attack();
        }
    }

    // FixedUpdate is called 50x per frame
    void FixedUpdate()
    {
        if (isShooting == true)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (isKnockback == false) 
        { 
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.linearVelocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }
}
