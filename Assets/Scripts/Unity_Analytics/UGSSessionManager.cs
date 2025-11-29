using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Analytics;
using UnityEngine.Analytics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UGSSessionManager : MonoBehaviour
{
    private string _sessionId;
    private float _sessionStartTime;
    // TCS is used to pause the async flow until the UI decision is made
    private TaskCompletionSource<bool> _consentTCS = new TaskCompletionSource<bool>();

    // 1. UGS Initialization and Authentication (The starting point)
    void Awake()
    {
        // === CRITICAL LINK: SUBSCRIBE TO THE UI EVENT ===
        // We subscribe immediately so we don't miss the button click signal
        ConsentUIManager.OnConsentDecisionMade += HandleConsentDecision;

        // Start the main asynchronous initialization sequence
        InitializeUGSAsync();
    }

    // Listener method that the UI script calls when a button is clicked
    private void HandleConsentDecision(bool isGranted)
    {
        // This releases the 'await _consentTCS.Task' line, resuming the async flow
        _consentTCS.SetResult(isGranted);

        // Clean up the subscription
        ConsentUIManager.OnConsentDecisionMade -= HandleConsentDecision;
    }

    private async void InitializeUGSAsync()
    {
        _sessionId = Guid.NewGuid().ToString();
        _sessionStartTime = Time.time;

        try
        {
            // Initializes UGS Core and Authentication (fast setup)
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"UGS Initialized. PlayerID: {AuthenticationService.Instance.PlayerId}");

            // === 2. PAUSE AND WAIT FOR UI DECISION (The Game Freeze) ===
            // Execution stops here. The UI is already visible from its own Start() method.
            bool consentGranted = await _consentTCS.Task;

            // === 3. ACT ON CONSENT (START/STOP DATA COLLECTION) ===
            SetDataCollectionStatus(consentGranted);

            // --- CONTINUE THE GAME LOGIC ---
            if (consentGranted)
            {
                // If accepted, fire the mandatory session events

                // Check if this is the first session to send device_spec
                bool isFirstSession = PlayerPrefs.GetInt("HasSentDeviceSpec", 0) == 0;
                if (isFirstSession)
                {
                    PlayerPrefs.SetInt("HasSentDeviceSpec", 1);
                    PlayerPrefs.Save();
                }

                Debug.Log("Game flow resumed. Analytics are ON.");
            }
            else
            {
                Debug.LogWarning("Consent denied by player. Analytics are OFF. Game flow resumed.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"UGS Initialization Failed: {e.Message}");
        }
    }

    // Helper method to execute the explicit Start/Stop commands
    private void SetDataCollectionStatus(bool granted)
    {
        if (granted)
        {
            // Player Accepted: Explicitly signals to the SDK to begin collection
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Analytics: Data collection ENABLED.");
        }
        else
        {
            // Player Denied: Explicitly signals to the SDK to stop collection [1]
            AnalyticsService.Instance.StopDataCollection();
            Debug.Log("Analytics: Data collection DISABLED.");
        }
    }

}