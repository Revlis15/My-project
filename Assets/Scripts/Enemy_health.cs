using UnityEngine;

public class Enemy_health : MonoBehaviour
{
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
            curHealth = 0;
            Destroy(gameObject);
        }
    }
}
