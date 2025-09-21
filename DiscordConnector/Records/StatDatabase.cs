using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using BepInEx;

using DiscordConnector.Records.Models;

using Newtonsoft.Json;

namespace DiscordConnector.Records;

internal static class StatDatabase
{
    private static readonly string
        PlayerDataPath = Path.Combine(Paths.ConfigPath, DiscordConnectorPlugin.LegacyConfigPath, "stats");

    private static readonly Dictionary<string, PlayerStats> playerCache = new();
    private static readonly object cacheLock = new(); // Object for locking the cache

    static StatDatabase()
    {
        if (!Directory.Exists(PlayerDataPath))
        {
            Directory.CreateDirectory(PlayerDataPath);
        }
    }

    internal static async Task<PlayerStats> GetPlayerStatsAsync(string networkId, bool isPlayFab = true)
    {
        lock (cacheLock)
        {
            if (playerCache.TryGetValue(networkId, out PlayerStats stats))
            {
                return stats;
            }
        }

        string filePath = Path.Combine(PlayerDataPath, $"{networkId}.json");

        return await Task.Run(() =>
        {
            if (!File.Exists(filePath))
            {
                return CreateAndCacheNewRecord(networkId, isPlayFab);
            }

            try
            {
                string json = File.ReadAllText(filePath);
                PlayerStats? loadedStats = JsonConvert.DeserializeObject<PlayerStats>(json);

                if (loadedStats == null)
                {
                    DiscordConnectorPlugin.StaticLogger.LogError(
                        $"Failed to load player stats for {networkId}. Making new record.");
                    return CreateAndCacheNewRecord(networkId, isPlayFab);
                }

                lock (cacheLock) { playerCache[networkId] = loadedStats; }

                return loadedStats;
            }
            catch (JsonException ex)
            {
                DiscordConnectorPlugin.StaticLogger.LogError(
                    $"Failed to parse player stats for {networkId}. Creating new.\n{ex}");
                return CreateAndCacheNewRecord(networkId, isPlayFab);
            }
        }).ConfigureAwait(false);
    }

    private static PlayerStats CreateAndCacheNewRecord(string networkId, bool isPlayFab = true)
    {
        string filePath = Path.Combine(PlayerDataPath, $"{networkId}.json");
        PlayerStats newStats = new(networkId, isPlayFab);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(newStats));
        lock (cacheLock) { playerCache[networkId] = newStats; }

        return newStats;
    }

    /// <summary>
    ///     Save a player stats record to disk.
    /// </summary>
    /// <param name="stats">stats to save to disk</param>
    internal static async Task SavePlayerStatsAsync(PlayerStats? stats)
    {
        if (stats == null || string.IsNullOrEmpty(stats.Id))
        {
            return;
        }

        // Update the cache first under a lock.
        lock (cacheLock)
        {
            playerCache[stats.Id] = stats;
        }

        string networkId = stats.Id;
        string filePath = Path.Combine(PlayerDataPath, $"{networkId}.json");

        await Task.Run(() =>
        {
            try
            {
                string json = JsonConvert.SerializeObject(stats, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (JsonException ex)
            {
                DiscordConnectorPlugin.StaticLogger.LogError($"Failed to serialize player stats for {networkId}\n{ex}");
            }
            catch (IOException ex)
            {
                DiscordConnectorPlugin.StaticLogger.LogError($"Failed to persist stats for {networkId}\n{ex}");
            }
        }).ConfigureAwait(false);
    }
}
