using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

// Lưu ý: Cần có thư viện TelemetryTracker và TelemetryEvents đã được định nghĩa.

public class DataSimulator : MonoBehaviour
{
    private List<string> MAU_IDs = new List<string>();
    private const int TOTAL_MAU = 30000;

    // Sử dụng Seed Random chung cho các tham số ngẫu nhiên không quan trọng
    private System.Random globalRandom;
    private bool isInitialized = false;
    private int totalSessionsToSimulate = 0;



    // Định nghĩa các Archetype (tham số lấy từ báo cáo của bạn)
    private struct ArchetypeModel
    {
        public string Name;
        public int Count;
        public float DailyChance;
        public Action<string, System.Random> SimulateAction; // Nhận System.Random cục bộ
    }
    private ArchetypeModel[] Archetypes;

    void Awake()
    {
        globalRandom = new System.Random();

        // 1. Khởi tạo ID người chơi
        InitializeMAU();

        // 2. Định nghĩa Mô hình hành vi
        DefineArchetypes();

        // 3. Chờ UGS khởi tạo (Đảm bảo chạy sau UGSSessionManager)
        // Bạn có thể thêm logic kiểm tra UGS đã sẵn sàng tại đây.
        isInitialized = true;

        // Cần tính tổng số phiên cần mô phỏng (tổng DAU)
        totalSessionsToSimulate = Archetypes.Sum(a => Mathf.RoundToInt(a.Count * a.DailyChance));
        Debug.Log($"Simulator Ready. Total DAU Sessions to Simulate: {totalSessionsToSimulate}");
    }

    void Update()
    {
        // Kiểm tra phím P (Debug Key)
        if (isInitialized && Input.GetKeyDown(KeyCode.P))
        {
            // Kiểm tra xem DataSimulator đã sẵn sàng chưa
            // Thêm logic kiểm tra xem nó có đang chạy mô phỏng không, để tránh gọi đè
            if (gameObject.activeInHierarchy) // Kiểm tra nếu object đang hoạt động
            {
                Debug.Log("Key P Pressed. Initiating Data Simulation...");
                SimulateOneDayOfData();
            }
        }
    }

    private void InitializeMAU()
    {
        for (int i = 0; i < TOTAL_MAU; i++)
        {
            MAU_IDs.Add(Guid.NewGuid().ToString());
        }
    }

    private void DefineArchetypes()
    {
        Archetypes = new ArchetypeModel[]
        {
            new ArchetypeModel { Name = "Casual Dabbler", Count = 15000, DailyChance = 0.10f, SimulateAction = SimulateCasualDabbler },
            new ArchetypeModel { Name = "Engaged Newbie", Count = 6000, DailyChance = 0.33f, SimulateAction = SimulateEngagedNewbie },
            new ArchetypeModel { Name = "Typical Player", Count = 6000, DailyChance = 0.40f, SimulateAction = SimulateTypicalPlayer },
            new ArchetypeModel { Name = "End-Game Raider", Count = 1500, DailyChance = 0.50f, SimulateAction = SimulateEndGameRaider },
            new ArchetypeModel { Name = "Grinder / Crafter", Count = 1500, DailyChance = 0.67f, SimulateAction = SimulateGrinderCrafter }
        };
    }

    // GỌI HÀM NÀY ĐỂ BẮT ĐẦU TẠO DỮ LIỆU
    public async void SimulateOneDayOfData()
    {
        if (!isInitialized) return;

        int sessionCounter = 0;
        const int BATCH_SIZE = 100; // Số phiên chơi gửi trong 1 lô

        Debug.Log("--- STARTING HIGH-VOLUME SIMULATION ---");

        foreach (var archetype in Archetypes)
        {
            int sessionsToFire = Mathf.RoundToInt(archetype.Count * archetype.DailyChance);

            for (int i = 0; i < sessionsToFire; i++)
            {
                // 1. CHỌN PLAYER ID VÀ THIẾT LẬP HẠT GIỐNG NGẪU NHIÊN
                string playerId = MAU_IDs[globalRandom.Next(TOTAL_MAU)];
                // Dùng ID làm hạt giống (Seed) để hành vi có thể tái tạo
                System.Random sessionRandom = new System.Random(playerId.GetHashCode());

                // 2. THIẾT LẬP CONTEXT VÀ MÔ PHỎNG
                UnityServices.ExternalUserId = playerId;
                archetype.SimulateAction(playerId, sessionRandom);

                // 3. XỬ LÝ LÔ (BATCHING)
                sessionCounter++;
                if (sessionCounter % BATCH_SIZE == 0)
                {
                    AnalyticsService.Instance.Flush();
                    Debug.Log($"Flushed {sessionCounter}/{totalSessionsToSimulate} sessions.");
                    await Task.Delay(5*1000); // Giảm tải cho Editor
                }
            }
        }

        // Gửi lô cuối cùng
        AnalyticsService.Instance.Flush();
        Debug.Log("Simulation COMPLETE. Total Events Fired: ~267,000");
    }

    // =======================================================
    // II. CÁC HÀM HÀNH VI CỤ THỂ (SỬ DỤNG SEED-BASED RANDOMNESS)
    // =======================================================

    // Lớp mô phỏng cơ bản (Casual Dabbler: 19 Events/30 mins)
    private void SimulateCasualDabbler(string playerId, System.Random random)
    {
        const int PAUSE_POINTS = 3;

        // 1. Immediate Events
        TelemetryTracker.TrackLoadComplete("Scene_A", (float)random.NextDouble() * 5f + 2f);
        TelemetryTracker.TrackQuestEvent("Quest_01_Intro", "Completed", 1, 900f);

        // 2. Tích lũy (Batched Events)
        for (int i = 0; i < PAUSE_POINTS; i++)
        {
            EconomyAggregator.Instance.AddSource("Gold", random.Next(100, 500));
            EconomyAggregator.Instance.AddSink("HealthPotion", random.Next(0, 1)); // Chi tiêu ít
        }

        // Gửi lô và Reset
        EconomyAggregator.Instance.SendBatchedEventsAndFlush("Simulation_Casual_End");
    }

    // Lớp mô phỏng phức tạp (Engaged Newbie: 43 Events/1 hr)
    private void SimulateEngagedNewbie(string playerId, System.Random random)
    {
        string[] easyEnemies = { "Rat", "Goblin", "Spider" };
        const int PAUSE_POINTS = 6;

        // 1. Immediate Events (4 lần chết)
        for (int i = 0; i < 4; i++)
        {
            TelemetryTracker.TrackPlayerDeath(
                levelId: "Forest_Zone",
                enemyId: easyEnemies[random.Next(easyEnemies.Length)], 
                playerLevel: random.Next(1, 3), 
                position: new Vector3((float)random.NextDouble() * 500f, (float)random.NextDouble() * 500f, 0)
            );
        }
        
        // 2. Tích lũy (Batched Events)
        for (int i = 0; i < PAUSE_POINTS; i++)
        {
            EconomyAggregator.Instance.AddSource("Gold", random.Next(500, 1500));
            EconomyAggregator.Instance.AddSink("HealthPotion", random.Next(1, 5)); // Chi tiêu Potion
            
            Dictionary<string, int> gridCounts = new Dictionary<string, int> {
                {"G" + random.Next(1, 10), random.Next(60, 200)}
            };
            EconomyAggregator.Instance.UpdateGridTime("Forest_Zone", "G" + random.Next(1, 10), 120);
        }
        
        // Gửi lô và Reset
        EconomyAggregator.Instance.SendBatchedEventsAndFlush("Simulation_Newbie_End");  
    }

    private void SimulateTypicalPlayer(string playerId, System.Random random)
    {
        int playerLevel = random.Next(15, 30);
        const int PAUSE_POINTS = 6;

        // Immediate Events
        TelemetryTracker.TrackLoadComplete("MidGame_CityLoad", (float)random.NextDouble() * 8f + 3f);
        TelemetryTracker.TrackLevelUp(playerLevel + 1, (float)random.Next(3000, 5000));

        // Tích lũy (Batched Events)
        for (int i = 0; i < PAUSE_POINTS; i++)
        {
            EconomyAggregator.Instance.AddSource("Gold", random.Next(1000, 3000));
            EconomyAggregator.Instance.AddSink("Gold", random.Next(500, 1500));

            // Tích lũy Vị trí ngẫu nhiên
            EconomyAggregator.Instance.UpdateGridTime("MidGame_Map", "C" + random.Next(1, 5), random.Next(60, 300));
        }

        // Gửi lô và Reset
        EconomyAggregator.Instance.SendBatchedEventsAndFlush("Simulation_Typical_End");
    }

    private void SimulateEndGameRaider(string playerId, System.Random random)
    {
        const int BOSS_ATTEMPTS = 6;

        // Immediate Events
        TelemetryTracker.TrackLoadComplete("BossArena_Load", 5f);

        // Tích lũy (Chủ yếu là Sinks và Combat Summary)
        for (int i = 0; i < BOSS_ATTEMPTS; i++)
        {
            // Tích lũy chi tiêu Potion
            EconomyAggregator.Instance.AddSink("HighPotion", random.Next(5, 15));

            // Giả định Combat Summary được tính toán cho mỗi trận đấu
            Dictionary<string, int> abilitiesUsed = new Dictionary<string, int> { { "Spell_" + random.Next(1, 5), random.Next(10, 30) } };
            Dictionary<string, int> itemsUsed = new Dictionary<string, int> { { "HighPotion", random.Next(1, 5) } };

            // Gửi Combat Summary ngay
            TelemetryTracker.SendCombatSummary(
                random.Next(0, 100) > 10 ? "Victory" : "Defeat",
                (float)random.Next(120, 300),
                abilitiesUsed, itemsUsed
            );
        }

        // Gửi lô và Reset
        EconomyAggregator.Instance.SendBatchedEventsAndFlush("Simulation_Raider_End");
    }
    private void SimulateGrinderCrafter(string playerId, System.Random random)
    {
        const int PAUSE_POINTS = 6;

        // Immediate Events
        TelemetryTracker.TrackLoadComplete("Farming_Dungeon", 4f);

        // Tích lũy (Batched Events)
        for (int i = 0; i < PAUSE_POINTS; i++)
        {
            // Tích lũy nguồn thu cao
            EconomyAggregator.Instance.AddSource("IronOre", random.Next(100, 500));

            // Tích lũy Heatmap (Tập trung ở một khu vực nhỏ)
            EconomyAggregator.Instance.UpdateGridTime("Farming_Dungeon", "F1", 600); // 10 phút ở F1
        }

        // Gửi lô và Reset
        EconomyAggregator.Instance.SendBatchedEventsAndFlush("Simulation_Grinder_End");
    }
}