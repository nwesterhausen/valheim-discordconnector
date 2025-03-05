using System;
using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class VariableConfig
{
    // config header strings
    private const string VARIABLE_SETTINGS = "Variable Definition";
    private const string DYNAMIC_VARIABLE_CONFIG = "Variables.DynamicConfig";
    private static ConfigFile config;

    public static string ConfigExtension = "variables";
    private ConfigEntry<string> appendedPosFormat;
    private ConfigEntry<string> posVarFormat;

    // Variable Definition
    private ConfigEntry<string> userVar;
    private ConfigEntry<string> userVar1;
    private ConfigEntry<string> userVar2;
    private ConfigEntry<string> userVar3;
    private ConfigEntry<string> userVar4;
    private ConfigEntry<string> userVar5;
    private ConfigEntry<string> userVar6;
    private ConfigEntry<string> userVar7;
    private ConfigEntry<string> userVar8;
    private ConfigEntry<string> userVar9;

    public VariableConfig(ConfigFile configFile)
    {
        config = configFile;
        
        // User Variable Settings
        userVar = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 1",
            "",
            "This variable can be reference in any of the message content settings with %VAR1%");
        userVar1 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 2",
            "",
            "This variable can be reference in any of the message content settings with %VAR2%");
        userVar2 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 3",
            "",
            "This variable can be reference in any of the message content settings with %VAR3%");
        userVar3 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 4",
            "",
            "This variable can be reference in any of the message content settings with %VAR4%");
        userVar4 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 5",
            "",
            "This variable can be reference in any of the message content settings with %VAR5%");
        userVar5 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 6",
            "",
            "This variable can be reference in any of the message content settings with %VAR6%");
        userVar6 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 7",
            "",
            "This variable can be reference in any of the message content settings with %VAR7%");
        userVar7 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 8",
            "",
            "This variable can be reference in any of the message content settings with %VAR8%");
        userVar8 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 9",
            "",
            "This variable can be reference in any of the message content settings with %VAR9%");
        userVar9 = config.Bind<string>(VARIABLE_SETTINGS,
            "Defined Variable 10",
            "",
            "This variable can be reference in any of the message content settings with %VAR10%");

        posVarFormat = config.Bind<string>(DYNAMIC_VARIABLE_CONFIG,
            "POS Variable Formatting",
            "%X%, %Y%, %Z%",
            "Modify this to change how the %POS% variable gets displayed." + Environment.NewLine +
            "You can use %X%, %Y%, and %Z% in this value to customize how the %POS% gets sent.");
        appendedPosFormat = config.Bind<string>(DYNAMIC_VARIABLE_CONFIG,
            "Auto-Appended POS Format",
            "Coords: (%POS%)",
            "This defines how the automatic inclusion of the position data is included. This gets appended to the messages sent." +
            Environment.NewLine +
            "If you prefer to embed the POS inside the message instead of embedding it, you can modify the messages in the message config " +
            Environment.NewLine +
            "to include the %POS% variable. This POS message only gets appended on the message if no %POS% is in the message getting sent " +
            Environment.NewLine +
            "but you have sent position data enabled for that message.");

        config.Save();
    }

    // Variables
    public string UserVariable => userVar.Value;
    public string UserVariable1 => userVar1.Value;
    public string UserVariable2 => userVar2.Value;
    public string UserVariable3 => userVar3.Value;
    public string UserVariable4 => userVar4.Value;
    public string UserVariable5 => userVar5.Value;
    public string UserVariable6 => userVar6.Value;
    public string UserVariable7 => userVar7.Value;
    public string UserVariable8 => userVar8.Value;
    public string UserVariable9 => userVar9.Value;
    public string PosVarFormat => posVarFormat.Value;
    public string AppendedPosFormat => appendedPosFormat.Value;

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += "\"User-Defined\":{";
        jsonString += $"\"userVar\":\"{UserVariable.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar1\":\"{UserVariable1.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar2\":\"{UserVariable2.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar3\":\"{UserVariable3.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar4\":\"{UserVariable4.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar5\":\"{UserVariable5.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar6\":\"{UserVariable6.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar7\":\"{UserVariable7.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar8\":\"{UserVariable8.Replace("\"", "\\\"")}\",";
        jsonString += $"\"userVar9\":\"{UserVariable9.Replace("\"", "\\\"")}\"";
        jsonString += "},";
        jsonString += "\"Dynamic-Configured\":{";
        jsonString += $"\"posVarFormat\":\"{PosVarFormat.Replace("\"", "\\\"")}\",";
        jsonString += $"\"appendedPosFormat\":\"{AppendedPosFormat.Replace("\"", "\\\"")}\"";
        jsonString += "}";
        jsonString += "}";
        return jsonString;
    }
}
