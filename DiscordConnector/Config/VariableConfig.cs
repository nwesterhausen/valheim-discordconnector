using System;

using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class VariableConfig
{
    // config header strings
    private const string VariableSettings = "Variable Definition";
    private const string DynamicVariableConfig = "Variables.DynamicConfig";

    public const string ConfigExtension = "variables";
    private readonly ConfigEntry<string> _appendedPosFormat;
    private readonly ConfigEntry<string> _posVarFormat;

    // Variable Definition
    private readonly ConfigEntry<string> _userVar;
    private readonly ConfigEntry<string> _userVar1;
    private readonly ConfigEntry<string> _userVar2;
    private readonly ConfigEntry<string> _userVar3;
    private readonly ConfigEntry<string> _userVar4;
    private readonly ConfigEntry<string> _userVar5;
    private readonly ConfigEntry<string> _userVar6;
    private readonly ConfigEntry<string> _userVar7;
    private readonly ConfigEntry<string> _userVar8;
    private readonly ConfigEntry<string> _userVar9;

    public VariableConfig(ConfigFile configFile)
    {
        // User Variable Settings
        _userVar = configFile.Bind<string>(VariableSettings,
            "Defined Variable 1",
            "",
            "This variable can be reference in any of the message content settings with %VAR1%");
        _userVar1 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 2",
            "",
            "This variable can be reference in any of the message content settings with %VAR2%");
        _userVar2 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 3",
            "",
            "This variable can be reference in any of the message content settings with %VAR3%");
        _userVar3 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 4",
            "",
            "This variable can be reference in any of the message content settings with %VAR4%");
        _userVar4 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 5",
            "",
            "This variable can be reference in any of the message content settings with %VAR5%");
        _userVar5 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 6",
            "",
            "This variable can be reference in any of the message content settings with %VAR6%");
        _userVar6 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 7",
            "",
            "This variable can be reference in any of the message content settings with %VAR7%");
        _userVar7 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 8",
            "",
            "This variable can be reference in any of the message content settings with %VAR8%");
        _userVar8 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 9",
            "",
            "This variable can be reference in any of the message content settings with %VAR9%");
        _userVar9 = configFile.Bind<string>(VariableSettings,
            "Defined Variable 10",
            "",
            "This variable can be reference in any of the message content settings with %VAR10%");

        _posVarFormat = configFile.Bind<string>(DynamicVariableConfig,
            "POS Variable Formatting",
            "%X%, %Y%, %Z%",
            "Modify this to change how the %POS% variable gets displayed." + Environment.NewLine +
            "You can use %X%, %Y%, and %Z% in this value to customize how the %POS% gets sent.");
        _appendedPosFormat = configFile.Bind<string>(DynamicVariableConfig,
            "Auto-Appended POS Format",
            "Coords: (%POS%)",
            "This defines how the automatic inclusion of the position data is included. This gets appended to the messages sent." +
            Environment.NewLine +
            "If you prefer to embed the POS inside the message instead of embedding it, you can modify the messages in the message config " +
            Environment.NewLine +
            "to include the %POS% variable. This POS message only gets appended on the message if no %POS% is in the message getting sent " +
            Environment.NewLine +
            "but you have sent position data enabled for that message.");

        configFile.Save();
    }

    // Variables
    public string UserVariable => _userVar.Value;
    public string UserVariable1 => _userVar1.Value;
    public string UserVariable2 => _userVar2.Value;
    public string UserVariable3 => _userVar3.Value;
    public string UserVariable4 => _userVar4.Value;
    public string UserVariable5 => _userVar5.Value;
    public string UserVariable6 => _userVar6.Value;
    public string UserVariable7 => _userVar7.Value;
    public string UserVariable8 => _userVar8.Value;
    public string UserVariable9 => _userVar9.Value;
    public string PosVarFormat => _posVarFormat.Value;
    public string AppendedPosFormat => _appendedPosFormat.Value;

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
