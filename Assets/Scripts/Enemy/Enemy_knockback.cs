using System.Collections;
using UnityEngine;

public class Enemy_knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_movement enemy_Movement;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_movement>();
    }

    public void Knockback(Transform forceTransform, float force, float stunTime, float knockbackTime)
    {
        enemy_Movement.ChangeState(EnemyState.Knockback);
        StartCoroutine(KnockbackCounter(stunTime, knockbackTime));
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    IEnumerator KnockbackCounter(float stunTime, float knockbackTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
