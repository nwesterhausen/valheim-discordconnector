﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordConnector;

class ConfigWatcher
{
    /// <summary>
    /// Regex which matches only DiscordConnector config files; basically matches <code>discordconnector*.cfg</code> but restricted to 
    /// exactly how the files are named.
    /// </summary>
    private static Regex watchedConfigFilesRegex = new Regex(@"discordconnector?[\w\-]*\.cfg$");
    private static Regex configExtensionMatcherRegex = new Regex(@"discordconnector-(\w+)\.cfg$");
    /// <summary>
    /// Date when the last change to any config file was detected.
    /// </summary>
    private static DateTime lastChangeDetected;
    /// <summary>
    /// Period of time (in seconds) to ignore subsequent changes to config files.
    /// </summary>
    private static int DEBOUNCE_SECONDS = 10;
    /// <summary>
    /// A dictionary of 'filename' -> 'hash' to determine if config files were changed in a meaningful way.
    /// </summary>
    private static Dictionary<String, String> _fileHashDictionary;

    public ConfigWatcher()
    {
        var watcher = new FileSystemWatcher();

        watcher.NotifyFilter = NotifyFilters.Size
                           | NotifyFilters.LastWrite;

        watcher.Changed += OnChanged;
        watcher.Error += OnError;

        watcher.Path = Plugin.StaticConfig.configPath;

        watcher.Filter = "discordconnector*.cfg";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        Plugin.StaticLogger.LogInfo("File watcher loaded and watching for changes to configs.");

        // Create and populate the file hash dictionary (a collection of MD5 hashes of our configs, to be able
        // to determine if the files were properly changed or not).
        _fileHashDictionary = new Dictionary<string, string>();

        PopulateHashDictionary();

        // Set an initial value for last change detected.
        lastChangeDetected = DateTime.Now;
    }

    /// <summary>
    /// Offload population of hash dictionary to a separate thread if possible.
    /// </summary>
    private void PopulateHashDictionary()
    {
        Task.Run(() =>
        {
            // Get an iterable of files in the DiscordConnector config directory, where the file matches our config file regex
            var myConfigFiles = Directory.EnumerateFiles(Plugin.StaticConfig.configPath).Where(file => watchedConfigFilesRegex.IsMatch(file));
            foreach (String filename in myConfigFiles)
            {
                string extension = ConfigExtensionFromFilename(filename);
                // Put the filename str and the hash of the file into the dictionary
                _fileHashDictionary.Add(extension, DiscordConnector.Hashing.GetMD5Checksum(filename));
            }

            Plugin.StaticLogger.LogDebug($"Initialization of file hash dictionary completed.");
            Plugin.StaticLogger.LogDebug(string.Join(Environment.NewLine, _fileHashDictionary));
        });
    }

    /// <summary>
    /// Get the config file extension from the config file path
    /// </summary>
    /// <param name="filename">Filename or full file path to extract config file extension from</param>
    /// <returns>The extension slug for the config file</returns>
    private static string ConfigExtensionFromFilename(string filename)
    {
        // Determine config extension
        string extension = "main";
        var extensionMatch = configExtensionMatcherRegex.Match(filename);
        if (extensionMatch.Success && extensionMatch.Groups.Count > 1)
        {
            extension = extensionMatch.Groups[1].Value;
        }
        return extension;
    }

    /// <summary>
    /// Method for reacting to changes in the files (from the FileWatcher).
    /// </summary>
    /// <remarks>
    /// This method reacts to the change in config file by hashing the file again and on a different result, it tells the mod to reload that config.
    /// </remarks>
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        // Guard against other change types
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        String configExtension = ConfigExtensionFromFilename(e.FullPath);

        Plugin.StaticLogger.LogDebug($"Detected change of {configExtension} config file");

        // Hash the changed file
        String fileHash = DiscordConnector.Hashing.GetMD5Checksum(e.FullPath);

        // Create an entry if we haven't yet
        if (!_fileHashDictionary.ContainsKey(configExtension))
        {
            Plugin.StaticLogger.LogWarning("Unexpectedly encountered unhashed config file!");
            Plugin.StaticLogger.LogDebug($"Added {configExtension} config to config hash dictionary.");
            _fileHashDictionary.Add(configExtension, fileHash);
            return;
        }

        // Check if current hash differs from stored hash.
        if (String.Equals(_fileHashDictionary[configExtension], fileHash))
        {
            Plugin.StaticLogger.LogDebug("Changes to file were determined to be inconsequential.");
            return;
        }

        // Check if we are within a very short amount of time from last change. If so, ignore the change.
        if (lastChangeDetected.AddSeconds(DEBOUNCE_SECONDS) > DateTime.Now)
        {
            Plugin.StaticLogger.LogDebug("Skipping config reload, within DEBOUNCE timing.");
            return;
        }

        // Tell the plugin to reload the config file
        Plugin.StaticConfig.ReloadConfig(configExtension);
        lastChangeDetected = DateTime.Now; // Update last changed date
    }

    /// <summary>
    /// Error passthrough for the config watcher.
    /// </summary>
    private static void OnError(object sender, ErrorEventArgs e) =>
        Plugin.StaticLogger.LogError(e.GetException().ToString());

}
