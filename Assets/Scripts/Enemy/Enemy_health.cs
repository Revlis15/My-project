using UnityEngine;

public class Enemy_health : MonoBehaviour
{
    public int expReward = 3;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public int curHealth;
    public int maxHealth;

    public void Start()
    {
        curHealth = maxHealth;
    }

    public void changeHealth(int change)
    {
        curHealth += change;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
        else if (curHealth <= 0)
        {
            OnMonsterDefeated(expReward);
            Destroy(gameObject);
        }
    }
}
