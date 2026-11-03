using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BepInEx;

using UnityEngine;

namespace DiscordConnector.Tests;

/// <summary>
///     Runs DiscordConnector tests inside a real dedicated server.
///     Does nothing unless the environment variable DCTEST_RUN=1 is set.
///     Results go to the BepInEx log (lines start with "DCTEST") and to the JSON file named by DCTEST_RESULTS.
///     With DCTEST_QUIT=1 the server shuts down when the tests finish.
/// </summary>
[BepInPlugin("dctest.DiscordConnector.Tests", "DiscordConnector.Tests", "0.1.0")]
[BepInDependency("nwesterhausen.DiscordConnector")]
public class TestPlugin : BaseUnityPlugin
{
    internal static TestPlugin Instance;

    private void Awake()
    {
        Instance = this;
        if (Environment.GetEnvironmentVariable("DCTEST_RUN") != "1")
        {
            Logger.LogInfo("DCTEST_RUN is not 1; tests will not run.");
            return;
        }

        StartCoroutine(RunAll());
    }

    internal void Log(string message)
    {
        Logger.LogMessage("DCTEST " + message);
    }

    private IEnumerator RunAll()
    {
        Log("waiting for world and database");
        float deadline = Time.realtimeSinceStartup + 300;
        while (ZNet.instance == null || ZNet.m_world == null || !DiscordConnectorPlugin.StaticDatabase.IsReady)
        {
            if (Time.realtimeSinceStartup > deadline)
            {
                Log("TIMEOUT waiting for world/database");
                Finish(new List<TestResult> { TestResult.Fail("startup", "world or database never became ready", 0) });
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Let startup webhooks (launch/start messages) go out before the tests begin.
        yield return new WaitForSeconds(3f);

        List<TestResult> results = new();
        foreach (TestCase test in Suite.All())
        {
            if (test.Slow && Environment.GetEnvironmentVariable("DCTEST_SLOW") != "1")
            {
                results.Add(TestResult.Skip(test.Name, "slow test; set DCTEST_SLOW=1"));
                Log($"SKIP {test.Name}");
                continue;
            }

            Log($"RUN  {test.Name}");
            float start = Time.realtimeSinceStartup;
            Exception failure = null;
            IEnumerator body = test.Body();
            while (true)
            {
                object current;
                try
                {
                    if (!body.MoveNext())
                    {
                        break;
                    }

                    current = body.Current;
                }
                catch (Exception e)
                {
                    failure = e;
                    break;
                }

                yield return current;
            }

            float seconds = Time.realtimeSinceStartup - start;
            if (failure == null)
            {
                results.Add(TestResult.Pass(test.Name, seconds));
                Log($"PASS {test.Name} ({seconds:0.0}s)");
            }
            else
            {
                string message = failure is AssertionException ? failure.Message : failure.ToString();
                results.Add(TestResult.Fail(test.Name, message, seconds));
                Log($"FAIL {test.Name}: {message}");
            }
        }

        Finish(results);
    }

    private void Finish(List<TestResult> results)
    {
        int passed = 0, failed = 0, skipped = 0;
        StringBuilder json = new("[\n");
        for (int i = 0; i < results.Count; i++)
        {
            TestResult r = results[i];
            if (r.Status == "pass") passed++;
            else if (r.Status == "fail") failed++;
            else skipped++;
            json.Append(r.ToJson()).Append(i < results.Count - 1 ? ",\n" : "\n");
        }

        json.Append("]\n");

        string path = Environment.GetEnvironmentVariable("DCTEST_RESULTS");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json.ToString());
        }

        Log($"DONE passed={passed} failed={failed} skipped={skipped}");

        if (Environment.GetEnvironmentVariable("DCTEST_QUIT") == "1")
        {
            StartCoroutine(QuitSoon());
        }
    }

    private static IEnumerator QuitSoon()
    {
        // Give any last webhook sends a moment, then shut down like a normal server stop.
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}

internal class TestCase
{
    public string Name;
    public Func<IEnumerator> Body;
    public bool Slow;
}

internal class TestResult
{
    public string Name;
    public string Status;
    public string Message;
    public float Seconds;

    public static TestResult Pass(string name, float s) => new() { Name = name, Status = "pass", Seconds = s };

    public static TestResult Fail(string name, string message, float s) =>
        new() { Name = name, Status = "fail", Message = message, Seconds = s };

    public static TestResult Skip(string name, string message) =>
        new() { Name = name, Status = "skip", Message = message };

    public string ToJson()
    {
        return $"  {{\"name\": {Q(Name)}, \"status\": {Q(Status)}, \"seconds\": {Seconds:0.00}, \"message\": {Q(Message ?? "")}}}";
    }

    private static string Q(string s)
    {
        return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n") + "\"";
    }
}

internal class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}
