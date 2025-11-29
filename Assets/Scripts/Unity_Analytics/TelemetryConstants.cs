namespace Telemetry
{
    public static class TelemetryConstants
    {
        // =======================================================
        // I. TÊN SỰ KIỆN (EVENT NAMES)
        // =======================================================

        // A. Immediate (Signal) Events
        public const string EVT_SESSION_START = "session_start";
        public const string EVT_DEVICE_SPEC = "device_spec";
        public const string EVT_SESSION_END = "session_end";

        // Progression, Combat & Technical
        public const string EVT_LEVEL_UP = "level_up";
        public const string EVT_QUEST = "quest_event";
        public const string EVT_PLAYER_DEATH = "player_death";
        public const string EVT_BOSS_EVENT = "boss_event";             
        public const string EVT_GAME_ERROR = "game_error";
        public const string EVT_LOAD_COMPLETE = "load_screen_complete"; 
        public const string EVT_SETTING_CHANGED = "setting_changed";   
        public const string EVT_CUTSCENE = "cutscene_interaction";      


        // B. Batched (Noise/Summary) Events
        public const string EVT_ECONOMY_SOURCE = "economy_source_summary";
        public const string EVT_ECONOMY_SINK = "economy_sink_summary";
        public const string EVT_COMBAT_SUMMARY = "combat_summary";      
        public const string EVT_HEATMAP = "position_heatmap_summary";

        // =======================================================
        // II. TÊN THAM SỐ (PARAMETER NAMES)
        // =======================================================

        public const string PARAM_SESSION_ID = "session_id";
        public const string PARAM_PLAYER_LEVEL = "userLevel"; 
        public const string PARAM_MAP_ID = "map_level_id";

        // Parameters cho Player Death
        public const string PARAM_ENEMY_ID = "enemy_id";
        public const string PARAM_POS_X = "position_x";
        public const string PARAM_POS_Y = "position_y";
        public const string PARAM_POS_Z = "position_z";

        // Parameters cho Level Up
        public const string PARAM_NEW_LEVEL = "new_level";
        public const string PARAM_TIME_TO_LEVEL_SEC = "time_to_level_seconds";

        // Parameters cho QuestEvent
        public const string PARAM_QUEST_ID = "quest_id";
        public const string PARAM_QUEST_STATUS = "status";
        public const string PARAM_TIME_TO_COMPLETE_QUEST_SEC = "time_to_complete_seconds";

        // Parameters cho GameErrorEvent
        public const string PARAM_ERROR_MES = "error_message";
        public const string PARAM_STACK_TRACE = "stackTrace";

        // Parameters cho CutsceneInteractionEvent
        public const string PARAM_CUTSCENE_ID = "cutscene_id";
        public const string PARAM_STATUS_CUTSCENE = "status_cutscene";

        // Parameters cho SettingChangedEvent
        public const string PARAM_SETTING_NAME = "setting_name";
        public const string PARAM_SETTING_NEW_VALUE = "new_value";

        // Parameters cho oadScreenCompleteEvent
        public const string PARAM_LOAD_SCREEN_ID = "load_screen_id";
        public const string PARAM_LOAD_SCREEN_DUR_SEC = "load_duration_seconds";


        // Parameters cho Batched Events
        public const string PARAM_SOURCES_JSON = "sources_json";
        public const string PARAM_SINKS_JSON = "sinks_json";
        public const string PARAM_GRID_COUNTS = "grid_counts";

        // Parameters cho Combat Summary
        public const string PARAM_ABILITIES_JSON = "abilities_used_json";
        public const string PARAM_ITEMS_JSON = "items_used_json";
        public const string PARAM_COMBAT_RESULT = "result";
        public const string PARAM_COMBAT_DUR_SEC = "duration_seconds";
    }
}