using UnityEngine;
using UnityEngine.UI;
using System;

public class ConsentUIManager : MonoBehaviour
{
    // C# Action to signal the result back to the UGSSessionManager
    public static event Action<bool> OnConsentDecisionMade;

    [Header("UI Elements")]
    public Button acceptButton;
    public Button denyButton;

    void Start()
    {
        Time.timeScale = 0;
        // Add listeners for button clicks
        acceptButton.onClick.AddListener(() => SendDecision(true));
        denyButton.onClick.AddListener(() => SendDecision(false));

        // Ensure the panel is visible at start
        gameObject.SetActive(true);
        
    }

    private void SendDecision(bool granted)
    {
        // 1. Send the result through the static event
        OnConsentDecisionMade?.Invoke(granted);

        // 2. Hide the UI panel immediately so the game can continue
        gameObject.SetActive(false);

        Time.timeScale = 1;
    }
}