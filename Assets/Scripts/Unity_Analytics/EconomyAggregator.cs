using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;
using Telemetry;
using System;

// Lớp này quản lý việc tích lũy dữ liệu cho các sự kiện theo lô (Batched Events)
public class EconomyAggregator : MonoBehaviour
{
    // Singleton Instance để dễ dàng truy cập
    public static EconomyAggregator Instance { get; private set; }

    // Dữ liệu tích lũy cục bộ (Local Aggregators)
    // Lưu ý: Key là tên tài nguyên/vật phẩm, Value là số lượng tích lũy
    private Dictionary<string, object> currentSources = new Dictionary<string, object>();
    private Dictionary<string, int> currentSinks = new Dictionary<string, int>();
    private Dictionary<string, int> currentGridCounts = new Dictionary<string, int>();

    // Bộ đếm thời gian để kích hoạt flush tự động
    private float batchTimer = 0f;
    private const float BATCH_INTERVAL = 600f; // 10 phút = 600 giây

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Kích hoạt Flush theo Timer (Fall-back mechanism)
        batchTimer += Time.deltaTime;
        if (batchTimer >= BATCH_INTERVAL)
        {
            SendBatchedEventsAndFlush("10_minute_timer");
            batchTimer = 0f;
        }
    }

    void OnApplicationQuit()
    {
        // Gửi lô cuối cùng trước khi đóng game để tránh mất dữ liệu
        SendBatchedEventsAndFlush("application_quit");
    }

    // =======================================================
    // I. HÀM GỌI TỪ GAME LOGIC (TÍCH LŨY DỮ LIỆU)
    // =======================================================

    // Hàm gọi khi người chơi thu thập tài nguyên (Source)
    public void AddSource(string resourceName, int amount)
    {
        if (currentSources.ContainsKey(resourceName))
            currentSources[resourceName] = (int)currentSources[resourceName] + amount;
        else
            currentSources.Add(resourceName, amount);
    }

    // Hàm gọi khi người chơi tiêu hao tài nguyên (Sink)
    public void AddSink(string resourceName, int amount)
    {
        if (currentSinks.ContainsKey(resourceName))
            currentSinks[resourceName] = (int)currentSinks[resourceName] + amount;
        else
            currentSinks.Add(resourceName, amount);
    }

    // Hàm gọi để cập nhật vị trí cho Heatmap
    public void UpdateGridTime(string mapId, string gridId, int seconds)
    {
        // Lưu trữ thời gian cho từng ô lưới
        // (Logic game sẽ gọi hàm này định kỳ, ví dụ: mỗi giây)
        if (currentGridCounts.ContainsKey(gridId))
            currentGridCounts[gridId] += seconds;
        else
            currentGridCounts.Add(gridId, seconds);
    }


    // =======================================================
    // II. HÀM GỬI DỮ LIỆU TẠI PAUSE POINT
    // =======================================================

    // Hàm QUAN TRỌNG: Gửi tất cả các sự kiện theo lô và Flush toàn bộ cache
    public void SendBatchedEventsAndFlush(string triggerReason)
    {
        Debug.Log($"Flushing batched data due to: {triggerReason}");

        // 1. Gửi các sự kiện theo lô (sử dụng dữ liệu đã tích lũy)
        TelemetryTracker.SendEconomySourceSummary(currentSources);
        TelemetryTracker.SendEconomySinkSummary(currentSinks);
        TelemetryTracker.SendPositionHeatmapSummary(currentGridCounts, "World_Main");
        // TODO: Cần gọi logic gửi Combat Summary tại đây khi nó được tích lũy.

        // 2. Gửi TẤT CẢ dữ liệu (Immediate và Batched) đã được cache lên server
        AnalyticsService.Instance.Flush();

        // 3. ĐẶT LẠI (RESET) các bộ đếm cục bộ cho lô tiếp theo
        currentSources.Clear();
        currentSinks.Clear();
        currentGridCounts.Clear();
    }
}