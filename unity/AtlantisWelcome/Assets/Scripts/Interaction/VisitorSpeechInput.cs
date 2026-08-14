using System.Net.Http;
using System.Text;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class VisitorSpeechInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField = null!;

    [SerializeField]
    private Button speakButton = null!;

    [SerializeField]
    private string apiBaseUrl =
        "http://localhost:5130";

    private static readonly HttpClient
        HttpClient = new();

    private void Awake()
    {
        speakButton.onClick.AddListener(
            Submit);

        inputField.onSubmit.AddListener(
            _ =>
                Submit());
    }

    private async void Submit()
    {
        var text =
            inputField.text.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var payload =
            new
            {
                ActorId =
                    "visitor-default",

                EntityId =
                    "visitor-default",

                Text =
                    text
            };

        var json =
            JsonSerializer.Serialize(
                payload);

        using var content =
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

        var response =
            await HttpClient.PostAsync(
                $"{apiBaseUrl}/api/world/entities/visitor-default/say",
                content);

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError(
                $"Visitor speech failed: " +
                $"{(int)response.StatusCode} " +
                $"{response.ReasonPhrase}");

            return;
        }

        inputField.text =
            string.Empty;

        inputField.ActivateInputField();
    }
}