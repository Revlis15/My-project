using Telemetry;
using Unity.Services.Analytics;
using Newtonsoft.Json;
using System.Collections.Generic;

// --- Sự kiện Gửi Ngay (Immediate Events) ---

// Kế thừa lớp Event cơ bản nếu SDK cung cấp, hoặc chỉ là một POCO (Plain Old C# Object)
// Tên lớp phải ánh xạ tới tên Event trong Event Manager của bạn
public class PlayerDeathEvent : Event
{
    // === 1. Hàm tạo (Constructor) ===
    // Đặt tên sự kiện
    public PlayerDeathEvent() : base(TelemetryConstants.EVT_PLAYER_DEATH) { }

    // === 2. Các Thuộc tính (Parameters) ===

    // level_id (string)
    public string map_level_id
    {
        set => SetParameter(TelemetryConstants.PARAM_MAP_ID, value);
    }

    // enemy_id (string)
    public string enemy_id
    {
        set => SetParameter(TelemetryConstants.PARAM_ENEMY_ID, value);
    }

    // player_level (int)
    public int player_level
    {
        set => SetParameter(TelemetryConstants.PARAM_PLAYER_LEVEL, value);
    }

    // position_x (float)
    public float position_x
    {
        set => SetParameter(TelemetryConstants.PARAM_POS_X, value);
    }

    // position_y (float)
    public float position_y
    {
        set => SetParameter(TelemetryConstants.PARAM_POS_Y, value);
    }

    // position_z (float)
    public float position_z
    {
        set => SetParameter(TelemetryConstants.PARAM_POS_Z, value);
    }
}

public class TrackLevelUp : Event
{
    public TrackLevelUp() : base(TelemetryConstants.EVT_LEVEL_UP) { }

    public int newlevel
    {
        set => SetParameter(TelemetryConstants.PARAM_NEW_LEVEL, value);
    }

    public float timeToLevelSeconds
    {
        set => SetParameter(TelemetryConstants.PARAM_TIME_TO_LEVEL_SEC, value);
    }
}

public class QuestEvent : Event
{
    public QuestEvent() : base(TelemetryConstants.EVT_QUEST) { }

    public string quest_id { set => SetParameter(TelemetryConstants.PARAM_QUEST_ID, value); }
    public string status { set => SetParameter(TelemetryConstants.PARAM_QUEST_STATUS, value); } 
    public int player_level { set => SetParameter(TelemetryConstants.PARAM_PLAYER_LEVEL, value); }
    public float time_to_complete_seconds { set => SetParameter(TelemetryConstants.PARAM_TIME_TO_COMPLETE_QUEST_SEC, value); }
}

public class GameErrorEvent : Event
{
    public GameErrorEvent() : base(TelemetryConstants.EVT_GAME_ERROR) { }

    public string error_message { set => SetParameter(TelemetryConstants.PARAM_ERROR_MES, value); }
    public string stack_trace { set => SetParameter(TelemetryConstants.PARAM_STACK_TRACE, value); }
    public string level_id { set => SetParameter(TelemetryConstants.PARAM_MAP_ID, value); }
}

public class CutsceneInteractionEvent : Event
{
    // Tên sự kiện: cutscene_interaction
    public CutsceneInteractionEvent() : base(TelemetryConstants.EVT_CUTSCENE) { }

    public string cutscene_id { set => SetParameter(TelemetryConstants.PARAM_CUTSCENE_ID, value); }
    public string status { set => SetParameter(TelemetryConstants.PARAM_STATUS_CUTSCENE, value); } 
}

public class SettingChangedEvent : Event
{
    // Tên sự kiện: setting_changed
    public SettingChangedEvent() : base(TelemetryConstants.EVT_SETTING_CHANGED) { }

    public string setting_name { set => SetParameter(TelemetryConstants.PARAM_SETTING_NAME, value); }
    public string new_value { set => SetParameter(TelemetryConstants.PARAM_SETTING_NEW_VALUE, value); } 
}

public class LoadScreenCompleteEvent : Event
{
    // Tên sự kiện: load_screen_complete
    public LoadScreenCompleteEvent() : base(TelemetryConstants.EVT_LOAD_COMPLETE) { }

    // Tham số 1: Tên màn hình tải
    public string load_screen_id { set => SetParameter(TelemetryConstants.PARAM_LOAD_SCREEN_ID, value); }

    // Tham số 2: Thời gian tải
    public float load_duration_seconds { set => SetParameter(TelemetryConstants.PARAM_LOAD_SCREEN_DUR_SEC, value); }
}

public class BossEvent : Event
{
    public BossEvent() : base(TelemetryConstants.EVT_BOSS_EVENT) { }

    // boss_id (string): ID của boss (ví dụ: TheLichKing)
    public string boss_id { set => SetParameter("boss_id", value); }

    // status (string): Started, Defeated, hoặc Wiped (Thua/Thất bại)
    public string status { set => SetParameter("status_boss_event", value); }

    // fight_duration_seconds (float): Thời gian chiến đấu
    public float fight_duration_seconds { set => SetParameter("fight_duration_seconds", value); }

    // player_level (int): Cấp độ của người chơi
    public int player_level { set => SetParameter(TelemetryConstants.PARAM_PLAYER_LEVEL, value); }
}

// --- Sự kiện Gửi Theo Lô (Batched/Summary Events) ---

// Tên lớp phải là tên sự kiện (economy_source_summary)
public class EconomySourceSummaryEvent : Event
{
    public EconomySourceSummaryEvent() : base(TelemetryConstants.EVT_ECONOMY_SOURCE) { }

    public string sources_json { set => SetParameter(TelemetryConstants.PARAM_SOURCES_JSON, value); }
}

public class EconomySinkSummaryEvent : Event
{
    public EconomySinkSummaryEvent() : base(TelemetryConstants.EVT_ECONOMY_SINK) { }

    public string sinks_json { set => SetParameter(TelemetryConstants.PARAM_SINKS_JSON, value); }
}

public class PositionHeatmapSummaryEvent : Event
{
    public PositionHeatmapSummaryEvent() : base(TelemetryConstants.EVT_HEATMAP) { }

    public string map_id { get; set; }  
                                       
    public string grid_counts { set => SetParameter(TelemetryConstants.PARAM_GRID_COUNTS, value); }
}

public class CombatSummaryEvent : Event
{
    public CombatSummaryEvent() : base(TelemetryConstants.EVT_COMBAT_SUMMARY) { }

    public string result { set => SetParameter(TelemetryConstants.PARAM_COMBAT_RESULT, value); }
    public float duration_seconds { set => SetParameter(TelemetryConstants.PARAM_COMBAT_DUR_SEC, value); }
    public string abilities_used_json { set => SetParameter(TelemetryConstants.PARAM_ABILITIES_JSON, value); }
    public string items_used_json { set => SetParameter(TelemetryConstants.PARAM_ITEMS_JSON, value); }
}

