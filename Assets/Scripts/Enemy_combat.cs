using UnityEngine;

public class Enemy_combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            foreach (Collider2D hit in hits)
            {
                hit.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hit.GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime);
            }
        }
    }
}
