using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int curHealth;
    public int maxHealth;

    public TMP_Text healthText;

    public void Start()
    {
        healthText.text = "HP: " + curHealth + "/" + maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        curHealth += amount;
        healthText.text = "HP: " + curHealth + "/" + maxHealth;
        if (curHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
