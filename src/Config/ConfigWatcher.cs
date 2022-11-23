using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscordConnector
{
    class ConfigWatcher
    {
        private static Regex watchedConfigFilesRegex = new Regex(@"games.nwest.valheim.discordconnector-?\w*\.cfg$");
        private static DateTime lastChangeDetected;
        private static int DEBOUNCE_SECONDS = 10;
        private static Dictionary<String, String> _fileHashDictionary;
        public ConfigWatcher()
        {
            var watcher = new FileSystemWatcher();

            watcher.NotifyFilter = NotifyFilters.Size
                               | NotifyFilters.LastWrite;

            watcher.Changed += OnChanged;
            watcher.Error += OnError;

            watcher.Path = BepInEx.Paths.ConfigPath;

            watcher.Filter = "games.nwest.valheim.discordconnector*.cfg";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Plugin.StaticLogger.LogInfo("File watcher loaded and watching for changes to configs.");

            // Create and populate the file hash dictionary (a collection of MD5 hashes of our configs, to be able
            // to determine if the files were properly changed or not).
            _fileHashDictionary = new Dictionary<string, string>();

            var myConfigFiles = Directory.EnumerateFiles(BepInEx.Paths.ConfigPath).Where(file => watchedConfigFilesRegex.IsMatch(file));
            foreach (String filename in myConfigFiles)
            {
                String fullPath = $"{filename}";
                _fileHashDictionary.Add(fullPath, DiscordConnector.Hashing.GetMD5Checksum(filename));
            }

            Plugin.StaticLogger.LogDebug($"Initialization of file hash dictionary completed.");
            Plugin.StaticLogger.LogDebug(string.Join(Environment.NewLine, _fileHashDictionary));

            // Set an initial value for last change detected.
            lastChangeDetected = DateTime.Now;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Plugin.StaticLogger.LogInfo($"Changed: {e.FullPath}");

            // Hash the changed file
            String fileHash = DiscordConnector.Hashing.GetMD5Checksum(e.FullPath);

            // Create an entry if we haven't yet
            if (!_fileHashDictionary.ContainsKey(e.FullPath))
            {
                Plugin.StaticLogger.LogWarning("Unexpectedly encountered unhashed config file!");
                _fileHashDictionary.Add(e.FullPath, fileHash);
                return;
            }

            // Check if current hash differs from stored hash.
            if (String.Equals(_fileHashDictionary[e.FullPath], fileHash))
            {
                Plugin.StaticLogger.LogDebug("Changes to file were determined to be inconsequential.");
            }
            else if (lastChangeDetected.AddSeconds(DEBOUNCE_SECONDS) > DateTime.Now)
            {
                Plugin.StaticLogger.LogDebug("Skipping config reload, within DEBOUNCE timing.");
            }
            else
            {
                Plugin.StaticConfig.ReloadConfig(e.FullPath);
                lastChangeDetected = DateTime.Now;
            }

        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            Plugin.StaticLogger.LogError(e.GetException());

    }
}
