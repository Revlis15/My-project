using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 5;
    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // FixedUpdate is called 50x per frame
    void FixedUpdate()
    {
        if (isKnockback == false) 
        { 
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
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
