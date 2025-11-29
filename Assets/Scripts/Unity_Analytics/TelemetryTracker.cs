using UnityEngine;
using Unity.Services.Analytics;
using Telemetry;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class TelemetryTracker
{
    // === 1. SỰ KIỆN GỬI NGAY (IMMEDIATE EVENTS) ===

    public static void TrackPlayerDeath(string levelId, string enemyId, int playerLevel, Vector3 position)
    {
        var myEvent = new PlayerDeathEvent
        {
            map_level_id = levelId,
            enemy_id = enemyId,
            player_level = playerLevel,
            position_x = position.x,
            position_y = position.y,
            position_z = position.z
        };

        // SỬ DỤNG PHƯƠNG THỨC MỚI: RecordEvent(myEvent)
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public static void TrackLevelUp(int newlevel, float timeToLevelSeconds)
    {
        var myEvent = new TrackLevelUp
        {
            newlevel = newlevel,
            timeToLevelSeconds = timeToLevelSeconds
        };

        // SỬ DỤNG PHƯƠNG THỨC MỚI: RecordEvent(myEvent)
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public static void TrackQuestEvent(string questId, string status, int playerLevel, float timeToComplete)
    {
        var myEvent = new QuestEvent
        {
            quest_id = questId,
            status = status,
            player_level = playerLevel,
            time_to_complete_seconds = timeToComplete
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public static void TrackGameError(string message, string stackTrace, string levelId)
    {
        var myEvent = new GameErrorEvent
        {
            error_message = message,
            stack_trace = stackTrace,
            level_id = levelId
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        // Flush ngay lập tức để giảm thiểu rủi ro mất dữ liệu trước khi game crash
        AnalyticsService.Instance.Flush();
    }

    public static void TrackCutsceneInteraction(string cutsceneId, string status)
    {
        var myEvent = new CutsceneInteractionEvent // Giả định lớp này được định nghĩa trong TelemetryEvents.cs
        {
            cutscene_id = cutsceneId,
            status = status // "Started", "Skipped", "Completed"
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public static void TrackSettingChanged(string settingName, string newValue)
    {
        var myEvent = new SettingChangedEvent // Giả định lớp này được định nghĩa trong TelemetryEvents.cs
        {
            setting_name = settingName,
            new_value = newValue
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public static void TrackLoadComplete(string loadScreenId, float loadDurationSeconds)
    {
        var myEvent = new LoadScreenCompleteEvent
        {
            load_screen_id = loadScreenId,
            load_duration_seconds = loadDurationSeconds
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        // Flush để gửi ngay lập tức, vì thời gian tải là dữ liệu UX quan trọng
        AnalyticsService.Instance.Flush();
    }

    public static void TrackBossEvent(string bossId, string status, float fightDurationSeconds, int playerLevel)
    {
        var myEvent = new BossEvent
        {
            boss_id = bossId,
            status = status,
            // Chỉ gửi thời gian nếu trận đấu đã kết thúc (Defeated/Wiped)
            fight_duration_seconds = status != "Started" ? fightDurationSeconds : 0f,
            player_level = playerLevel
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        // Flush để gửi ngay lập tức vì đây là sự kiện tiến trình quan trọng
        AnalyticsService.Instance.Flush();
    }

    // === 2. SỰ KIỆN GỬI THEO LÔ (BATCHED/SUMMARY EVENTS) ===

    public static void SendEconomySourceSummary(Dictionary<string, object> aggregatedSources)
    {
        // 1. Chuyển đối tượng C# đã tổng hợp thành Chuỗi JSON
        string sourcesJson = JsonConvert.SerializeObject(aggregatedSources);

        var myEvent = new EconomySourceSummaryEvent
        {
            // Gán chuỗi JSON vào thuộc tính sources_json
            sources_json = sourcesJson
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        // KHÔNG GỌI FLUSH Ở ĐÂY. Lớp EconomyAggregator sẽ gọi Flush chung tại Pause Point.
    }

    public static void SendEconomySinkSummary(Dictionary<string, int> aggregatedSinks)
    {
        // Cần Serialize đối tượng C# đã tổng hợp thành Chuỗi JSON
        string sinksJson = JsonConvert.SerializeObject(aggregatedSinks);

        var myEvent = new EconomySinkSummaryEvent // Sử dụng lớp sự kiện
        {
            sinks_json = sinksJson
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        // KHÔNG FLUSH
    }

    public static void SendPositionHeatmapSummary(Dictionary<string, int> gridCounts, string mapId)
    {
        string heatmapJson = JsonConvert.SerializeObject(gridCounts);

        var myEvent = new PositionHeatmapSummaryEvent
        {
            map_id = mapId,
            grid_counts = heatmapJson
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    public static void SendCombatSummary(string result, float durationSeconds, Dictionary<string, int> abilitiesUsed, Dictionary<string, int> itemsUsed)
    {
        // Serialize các đối tượng phức tạp thành chuỗi JSON
        string abilitiesJson = JsonConvert.SerializeObject(abilitiesUsed);
        string itemsJson = JsonConvert.SerializeObject(itemsUsed);

        var myEvent = new CombatSummaryEvent // Sử dụng lớp sự kiện
        {
            result = result, // "Victory" hoặc "Defeat"
            duration_seconds = durationSeconds,
            abilities_used_json = abilitiesJson,
            items_used_json = itemsJson
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        // KHÔNG FLUSH
    }

}