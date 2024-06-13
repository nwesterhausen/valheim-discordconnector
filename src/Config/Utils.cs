
using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiscordConnector.Config;

internal class Utils
{
  public static void WriteConfig(string yamlFilePath, Model.Config config)
  {
    var serializer = new SerializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();

    try
    {
      string yaml = serializer.Serialize(config);
      File.WriteAllText(yamlFilePath, yaml);

      Plugin.StaticLogger.LogInfo($"Exported config to {yamlFilePath}");
    }
    catch (YamlException e)
    {
      Plugin.StaticLogger.LogError($"YAML Exception: {e}");
    }
    catch (Exception e)
    {
      Plugin.StaticLogger.LogError($"Error writing config to YAML: {e.Message}");
    }
  }

  public static Model.Config ParseConfig(string yamlFilePath)
  {
    var deserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();
    try
    {
      string yaml = File.ReadAllText(yamlFilePath);
      Model.Config config = deserializer.Deserialize<Model.Config>(yaml);

      Plugin.StaticLogger.LogInfo($"Parsed config from {yamlFilePath}");

      return config;
    }
    catch (YamlException e)
    {
      Plugin.StaticLogger.LogError($"YAML Error parsing config: {e}");
      return new Model.Config();
    }
    catch (Exception e)
    {
      Plugin.StaticLogger.LogError($"Error parsing config from YAML: {e.Message}");
      return new Model.Config();
    }
  }
}
