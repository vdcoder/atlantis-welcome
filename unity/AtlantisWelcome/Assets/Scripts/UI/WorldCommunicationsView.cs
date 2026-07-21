using System.Collections;
using AtlantisWelcome.World;
using TMPro;
using UnityEngine;

namespace AtlantisWelcome.UI
{
    public sealed class WorldCommunicationsView :
        MonoBehaviour
    {
        [Header("Dependencies")]

        [SerializeField]
        private WorldSnapshotLoader worldLoader;

        [Header("Viewer")]

        [SerializeField]
        private string viewerEntityId =
            "visitor-default";

        [Header("Public Speech")]

        [SerializeField]
        private TMP_Text publicSpeechText;

        [SerializeField]
        private float publicSpeechDurationSeconds =
            5f;

        [Header("Private Touch")]

        [SerializeField]
        private TMP_Text privateTouchText;

        [SerializeField]
        private float privateTouchDurationSeconds =
            7f;

        private Coroutine _publicSpeechCoroutine;
        private Coroutine _privateTouchCoroutine;

        private void OnEnable()
        {
            worldLoader.UtteranceReceived +=
                HandleUtterance;

            worldLoader.PrivateMessageReceived +=
                HandlePrivateMessage;
        }

        private void OnDisable()
        {
            worldLoader.UtteranceReceived -=
                HandleUtterance;

            worldLoader.PrivateMessageReceived -=
                HandlePrivateMessage;
        }

        private void Start()
        {
            if (publicSpeechText != null)
            {
                publicSpeechText.text = string.Empty;
            }

            if (privateTouchText != null)
            {
                privateTouchText.text = string.Empty;
            }
        }

        private void HandleUtterance(
            EntityDto speaker,
            UtteranceDto utterance)
        {
            if (publicSpeechText == null)
            {
                return;
            }

            if (_publicSpeechCoroutine != null)
            {
                StopCoroutine(
                    _publicSpeechCoroutine);
            }

            publicSpeechText.text =
                $"{speaker.name}: “{utterance.text}”";

            _publicSpeechCoroutine =
                StartCoroutine(
                    ClearAfterDelay(
                        publicSpeechText,
                        publicSpeechDurationSeconds));
        }

        private void HandlePrivateMessage(
            EntityDto recipient,
            PrivateMessageDto message)
        {
            if (recipient.id != viewerEntityId)
            {
                return;
            }

            if (privateTouchText == null)
            {
                return;
            }

            if (_privateTouchCoroutine != null)
            {
                StopCoroutine(
                    _privateTouchCoroutine);
            }

            privateTouchText.text =
                $"Private touch from " +
                $"{message.senderId}: " +
                $"“{message.text}”";

            _privateTouchCoroutine =
                StartCoroutine(
                    ClearAfterDelay(
                        privateTouchText,
                        privateTouchDurationSeconds));
        }

        private static IEnumerator ClearAfterDelay(
            TMP_Text target,
            float durationSeconds)
        {
            yield return new WaitForSeconds(
                durationSeconds);

            if (target != null)
            {
                target.text = string.Empty;
            }
        }
    }
}