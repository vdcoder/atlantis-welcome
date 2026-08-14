using System.Diagnostics;
using UnityEngine;

namespace AtlantisWelcome.Voice
{
    public sealed class VisitorTextToSpeech
        : MonoBehaviour
    {
        public void Speak(
            string text)
        {
            if (string.IsNullOrWhiteSpace(
                    text))
            {
                return;
            }

            var escapedText =
                text.Replace(
                    "'",
                    "''");

            var script =
                "Add-Type -AssemblyName System.Speech; " +
                "$speech = New-Object " +
                "System.Speech.Synthesis.SpeechSynthesizer; " +
                "$speech.Speak('" +
                escapedText +
                "');";

            var startInfo =
                new ProcessStartInfo
                {
                    FileName =
                        "powershell.exe",

                    Arguments =
                        "-NoProfile -WindowStyle Hidden " +
                        "-Command \"" +
                        script +
                        "\"",

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };

            Process.Start(
                startInfo);
        }
    }
}