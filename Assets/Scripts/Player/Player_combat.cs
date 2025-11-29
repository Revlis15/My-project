using UnityEngine;

public class Player_combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public StatsUI statsUI;

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
        statsUI.UpdateDamage();

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            foreach (Collider2D enemyCollider in enemies)
            {          
                enemyCollider.GetComponent<Enemy_health>().changeHealth(-StatsManager.Instance.damage);
                enemyCollider.GetComponent<Enemy_knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.stunTime, StatsManager.Instance.knockbackTime);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }

    public void FinishAttack()
    {
        anim.SetBool("isAttacking", false);
    }


}
