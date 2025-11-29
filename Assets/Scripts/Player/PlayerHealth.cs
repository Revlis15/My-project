using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;

    public void Start()
    {
        healthText.text = "HP: " + StatsManager.Instance.curHealth + "/" + StatsManager.Instance.maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.curHealth += amount;
        healthText.text = "HP: " + StatsManager.Instance.curHealth + "/" + StatsManager.Instance.maxHealth;
        if (StatsManager.Instance.curHealth <= 0)
        {
            TelemetryTracker.TrackPlayerDeath("Current_Level_ID", "Unknown_Enemy", ExpManager.Instance.level, transform.position);
            Debug.Log("Player death log sent");
            //gameObject.SetActive(false);
            
        }
    }
}
