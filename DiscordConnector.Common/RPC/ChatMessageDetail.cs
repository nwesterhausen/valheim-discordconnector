using System;
using System.Globalization;

using UnityEngine;

namespace DiscordConnector.RPC;

public class ChatMessageDetail(Vector3 pos, Talker.Type type, string text)
{
    private static readonly char Separator = '|';
    private static readonly int ExpectedParts = 5;
    public Vector3 Pos { get; } = pos;
    public Talker.Type TalkerType { get; } = type;
    public string Text { get; } = text;
    public static string EmptyTextMessage { get; } = "#empty#";

    private string EncodeSelf()
    {
        // Invariant culture: "1.5" everywhere. A client using e.g. German settings used to send "1,5", which
        // a server with English settings read as 15.
        string pos = string.Join(Separator.ToString(),
            Pos.x.ToString("R", CultureInfo.InvariantCulture),
            Pos.y.ToString("R", CultureInfo.InvariantCulture),
            Pos.z.ToString("R", CultureInfo.InvariantCulture));
        string talkerType = ((int)TalkerType).ToString(CultureInfo.InvariantCulture);
        return $"{pos}{Separator}{talkerType}{Separator}{Text}";
    }

    internal static ChatMessageDetail DecodeSelf(string encoded)
    {
        // The text is last and may itself contain '|', so split into at most 5 parts.
        string[] parts = encoded.Split(new[] { Separator }, ExpectedParts);
        if (parts.Length != ExpectedParts)
        {
            throw new ArgumentException(
                $"Invalid number of parts in encoded string ({parts.Length} instead of {ExpectedParts})");
        }

        float x = ParseFloat(parts[0], "X");
        float y = ParseFloat(parts[1], "Y");
        float z = ParseFloat(parts[2], "Z");
        Vector3 pos = new(x, y, z);

        if (!int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int talkerTypeInt))
        {
            throw new ArgumentException(
                $"Failed to parse Talker.Type component: {parts[3]}");
        }

        if (!Enum.IsDefined(typeof(Talker.Type), talkerTypeInt))
        {
            throw new ArgumentException(
                $"Invalid Talker.Type value: {talkerTypeInt}");
        }

        Talker.Type type = (Talker.Type)talkerTypeInt;

        return new ChatMessageDetail(pos, type, parts[4]);
    }

    private static float ParseFloat(string value, string component)
    {
        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            return result;
        }

        // Older client plugins wrote numbers in the player's own format (e.g. "1,5").
        if (float.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
            float.TryParse(value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        throw new ArgumentException($"Failed to parse Pos.{component} component: {value}");
    }

    public ZPackage ToZPackage()
    {
        string encoded = EncodeSelf();

        try
        {
            ZPackage pkg = new();
            pkg.Write(encoded);
            return pkg;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to encode ZPackage with {encoded}", e);
        }
    }

    public static ChatMessageDetail FromZPackage(ZPackage? pkg)
    {
        if (pkg == null || pkg.Size() == 0)
        {
            throw new ArgumentException("ZPackage is null or empty");
        }

        try
        {
            string encoded = pkg.ReadString();
            return DecodeSelf(encoded);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to decode ZPackage", e);
        }
    }

    public override string ToString()
    {
        return
            $"ChatMessageDetail: Pos={Pos}, Type={TalkerType}, Text={Text}";
    }
}
