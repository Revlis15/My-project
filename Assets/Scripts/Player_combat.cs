using UnityEngine;

public class Player_combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1;
    public LayerMask enemyLayer;
    public int attackDamage = 1;

    public Animator anim;
    public float attackCooldown = 2;
    private float attackCooldownTimer = 0;

    public void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (attackCooldownTimer <= 0)
        {
            anim.SetBool("isAttacking", true);


            attackCooldownTimer = attackCooldown;
        }
    }

    public void dealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_health>().changeHealth(-attackDamage);
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }

    public void FinishAttack()
    {
        anim.SetBool("isAttacking", false);
    }


}
