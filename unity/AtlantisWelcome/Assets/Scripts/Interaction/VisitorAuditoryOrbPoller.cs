using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using TMPro;
using UnityEngine;
using AtlantisWelcome.Voice;

public sealed class VisitorAuditoryOrbPoller : MonoBehaviour
{
    [SerializeField]
    private VisitorTextToSpeech
        textToSpeech;

    [SerializeField]
    private TMP_Text outputText = null!;

    [SerializeField]
    private string apiBaseUrl =
        "http://localhost:5130";

    [SerializeField]
    private string entityId =
        "visitor-default";

    [SerializeField]
    private float pollIntervalSeconds =
        0.5f;

    private static readonly HttpClient
        HttpClient = new();

    private readonly HashSet<Guid>
        _seenOrbIds =
            new HashSet<Guid>();

    private bool _hasEstablishedBaseline;

    private Coroutine? _pollCoroutine;

    private void OnEnable()
    {
        _pollCoroutine =
            StartCoroutine(
                PollLoop());
    }

    private void OnDisable()
    {
        if (_pollCoroutine is not null)
        {
            StopCoroutine(
                _pollCoroutine);

            _pollCoroutine =
                null;
        }
    }

    private IEnumerator PollLoop()
    {
        while (true)
        {
            _ =
                PollOnceAsync();

            yield return new WaitForSeconds(
                pollIntervalSeconds);
        }
    }

    private async System.Threading.Tasks.Task
        PollOnceAsync()
    {
        try
        {
            var url =
                $"{apiBaseUrl}/api/world/entities/" +
                $"{entityId}/auditory-orbs";

            var response =
                await HttpClient.GetAsync(
                    url);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogWarning(
                    $"Auditory orb poll failed: " +
                    $"{(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}");

                return;
            }

            var json =
                await response.Content
                    .ReadAsStringAsync();

            var auditoryOrbs =
                JsonSerializer.Deserialize<
                    List<AuditoryOrbDto>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive =
                                true
                        });

            if (auditoryOrbs is null)
            {
                return;
            }

            if (!_hasEstablishedBaseline)
            {
                foreach (var orb in auditoryOrbs)
                {
                    _seenOrbIds.Add(
                        orb.OrbId);
                }

                _hasEstablishedBaseline =
                    true;

                return;
            }

            foreach (var orb in auditoryOrbs)
            {
                if (!_seenOrbIds.Add(
                        orb.OrbId))
                {
                    continue;
                }

                Append(
                    orb);

                textToSpeech.Speak(
                    orb.Content);
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(
                exception);
        }
    }

    private void Append(
        AuditoryOrbDto orb)
    {
        if (outputText.text.Length > 0)
        {
            outputText.text +=
                Environment.NewLine;
        }

        outputText.text +=
            $"{orb.Reference}: " +
            $"{orb.Content}";
    }

    [Serializable]
    private sealed class AuditoryOrbDto
    {
        public Guid OrbId { get; set; }

        public string Reference { get; set; } =
            string.Empty;

        public float DirectionX { get; set; }

        public float DirectionY { get; set; }

        public float DirectionZ { get; set; }

        public float Volume { get; set; }

        public string Content { get; set; } =
            string.Empty;
    }
}