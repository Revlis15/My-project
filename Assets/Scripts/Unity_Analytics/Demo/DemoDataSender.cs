using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;
using Telemetry;
using System.Threading.Tasks;

// Lớp này dùng để tạo và gửi một payload telemetry đầy đủ
public class DemoDataSender : MonoBehaviour
{
    private bool _canSend = false;

    // Giả lập ID và vị trí
    const string TEST_LEVEL_ID = "Zone1_Forest";
    const string TEST_ENEMY_ID = "WolfPackLeader";
    const int TEST_LEVEL = 2;
    Vector3 TEST_POSITION = new Vector3(100.0f, 50.0f, -2.0f);

    // Đặt Public để có thể gọi từ nút UI hoặc hàm kiểm tra
    public void EnableSender()
    {
        // Kích hoạt flag sau khi UGS đã sẵn sàng
        _canSend = true;
        Debug.Log("Demo Data Sender is ENABLED. Press 'P' to send full payload.");
    }

    void Update()
    {
        // Kích hoạt bằng nút P (ví dụ)
        if (_canSend && Input.GetKeyDown(KeyCode.P))
        {
            SendFullTelemetryPayload();
            Debug.Log("Demo sent again !!");
        }
    }

    public void SendFullTelemetryPayload()
    {
        if (!_canSend)
        {
            Debug.LogError("UGS not initialized or consent not granted. Cannot send data.");
            return;
        }

        Debug.Log("--- Starting Full Telemetry Payload Simulation ---");

        // *********************************************************
        // I. SỰ KIỆN GỬI NGAY (IMMEDIATE EVENTS)
        // *********************************************************

        // 1. Session & UX Events
        TelemetryTracker.TrackLoadComplete("MainMenuToZone1", 15.5f);
        TelemetryTracker.TrackSettingChanged("Difficulty", "Hard");

        // 2. Progression Events
        TelemetryTracker.TrackQuestEvent("EGame_01_FindSword", "Started", 1, 0f);
        TelemetryTracker.TrackQuestEvent("EGame_01_FindSword", "Completed", 2, 900.0f);
        TelemetryTracker.TrackLevelUp(newlevel: TEST_LEVEL, timeToLevelSeconds: 900.0f);

        // 3. High-Volume Events (4 lần chết)
        for (int i = 0; i < 4; i++)
        {
            TelemetryTracker.TrackPlayerDeath(TEST_LEVEL_ID, TEST_ENEMY_ID, TEST_LEVEL, TEST_POSITION);
        }

        // 4. Critical Events
        TelemetryTracker.TrackGameError("Example: Asset load failed on monster spawn.", "StackTrace Test", TEST_LEVEL_ID);

        // 5. Narrative Events
        TelemetryTracker.TrackCutsceneInteraction("Intro_Cutscene", "Completed");

        // *********************************************************
        // II. SỰ KIỆN GỬI THEO LÔ (BATCHED SUMMARY EVENTS)
        // *********************************************************

        // Giả lập dữ liệu đã tích lũy (Aggregated Data)
        Dictionary<string, object> sourcesData = new Dictionary<string, object>
        {
            {"Gold", 5000},
            {"IronOre", 150}
        };
        Dictionary<string, int> sinksData = new Dictionary<string, int>
        {
            {"Gold", 1200},
            {"HealthPotion", 25}
        };
        Dictionary<string, int> gridCountsData = new Dictionary<string, int>
        {
            {"A1", 350},
            {"B2", 120},
        };
        Dictionary<string, int> abilitiesUsed = new Dictionary<string, int>
        {
            {"Fireball", 15},
            {"IceWall", 5},
        };

        // 6. Gửi sự kiện theo lô (Batched)
        TelemetryTracker.SendEconomySourceSummary(sourcesData);
        TelemetryTracker.SendEconomySinkSummary(sinksData);
        TelemetryTracker.SendPositionHeatmapSummary(gridCountsData, TEST_LEVEL_ID);
        TelemetryTracker.SendCombatSummary("Victory", 125.5f, abilitiesUsed, sinksData);

        // *********************************************************
        // III. GỬI TẤT CẢ LÊN SERVER (FINAL FLUSH)
        // *********************************************************

        // Gọi Flush để gửi tất cả Immediate Events (đã cached) và Batched Events lên server
        AnalyticsService.Instance.Flush();

        Debug.Log("--- All Events have been generated and FLUSHED for Snowflake testing! ---");
    }
}