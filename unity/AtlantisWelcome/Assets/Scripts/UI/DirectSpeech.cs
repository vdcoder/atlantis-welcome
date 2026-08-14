using UnityEngine;
using System.Text;
using TMPro;

public sealed class DirectSpeech : MonoBehaviour
{
    [SerializeField]
    private TMP_Text previewText;

    private readonly StringBuilder _buffer =
        new StringBuilder();

    private void Update()
    {
        HandleEditingKeys();
        HandlePrintableCharacters();
        RefreshPreview();
    }

    private void HandleEditingKeys()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) &&
            _buffer.Length > 0)
        {
            _buffer.Length--;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Submit();
        }
    }

    private void HandlePrintableCharacters()
    {
        foreach (var character in Input.inputString)
        {
            if (character == '\b' ||
                character == '\n' ||
                character == '\r')
            {
                continue;
            }

            _buffer.Append(character);
        }
    }

    private void RefreshPreview()
    {
        previewText.text =
            _buffer.Length == 0
                ? string.Empty
                : "> " + _buffer;
    }

    private async void Submit()
    {
        var text =
            _buffer.ToString().Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        _buffer.Clear();
        RefreshPreview();

        // Call visitor Say here.
    }
}