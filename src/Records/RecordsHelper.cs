using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordConnector.Records
{
    /// <summary>
    /// These are categories used when keeping track of values in the record system. It is a simple system
    /// that currently only supports storing string:integer pairings underneath one of these categories.
    /// </summary>
    public static class Categories
    {
        public const string Death = "death";
        public const string Join = "join";
        public const string Leave = "leave";
        public const string Ping = "ping";
        public const string Shout = "shout";

        public readonly static string[] All = new string[] {
            Death,
            Join,
            Leave,
            Ping,
            Shout
        };
    }
    }
