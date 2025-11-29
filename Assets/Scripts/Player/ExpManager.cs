using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance;
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float expGrowthMultipler = 1.2f;
    public Slider expSlider;
    public TMP_Text currentLevelText;

    public static event Action<int> OnLevelUp;
    private float lastLevelUpTime;

    private void Start()
    {
        UpdateUI();

        lastLevelUpTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            GainExperience(2);        
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void OnEnable()
    {
        Enemy_health.OnMonsterDefeated += GainExperience;
    }
    private void OnDisable()
    {
        Enemy_health.OnMonsterDefeated -= GainExperience;
    }

    private void LevelUp()
    {

        float timeSinceLastLevel = Time.time - lastLevelUpTime;
        lastLevelUpTime = Time.time;

        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultipler);

        TelemetryTracker.TrackLevelUp(
            newlevel: level,
            timeToLevelSeconds: timeSinceLastLevel
        );
        Debug.Log("level_up");

        OnLevelUp?.Invoke(1);
    }

    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        currentLevelText.text = "Level: " + level;
        
    }
}
