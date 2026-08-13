using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using AtlantisWelcome.World;

public sealed class PositionObservationReporter
    : MonoBehaviour
{
    [SerializeField]
    private WorldSnapshotLoader worldLoader;

    [SerializeField]
    private string apiBaseUrl =
        "http://localhost:5130";

    [SerializeField]
    private string entityId =
        "visitor-default";

    [SerializeField]
    private float reportIntervalSeconds =
        0.1f;

    private float _elapsed;

    private void Update()
    {
        _elapsed +=
            Time.deltaTime;

        if (_elapsed <
            reportIntervalSeconds)
        {
            return;
        }

        _elapsed =
            0f;

        var entityView =
                worldLoader.FindEntityView(
                    entityId);

        if (entityView == null)
        {
            return;
        }

        StartCoroutine(
            ReportPositionAsync(
                entityView.transform.position));
    }

    private IEnumerator ReportPositionAsync(
        Vector3 position)
    {
        var request =
            new ReportPositionObservationRequest
            {
                actorId =
                    entityId,

                position =
                    new PositionDto
                    {
                        x =
                            position.x,

                        y =
                            position.y,

                        z =
                            position.z
                    }
            };

        var json =
            JsonUtility.ToJson(
                request);

        using var webRequest =
            new UnityWebRequest(
                $"{apiBaseUrl}/api/world/entities/{entityId}/report-position-observation",
                UnityWebRequest.kHttpVerbPOST);

        var body =
            System.Text.Encoding.UTF8
                .GetBytes(
                    json);

        webRequest.uploadHandler =
            new UploadHandlerRaw(
                body);

        webRequest.downloadHandler =
            new DownloadHandlerBuffer();

        webRequest.SetRequestHeader(
            "Content-Type",
            "application/json");

        yield return
            webRequest.SendWebRequest();

        if (webRequest.result !=
            UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(
                $"Position observation failed: " +
                $"{webRequest.responseCode} " +
                $"{webRequest.error}");
        }
    }

    [System.Serializable]
    private sealed class
        ReportPositionObservationRequest
    {
        public string actorId;
        public string observedEntityId;
        public PositionDto position;
    }

    [System.Serializable]
    private sealed class PositionDto
    {
        public float x;
        public float y;
        public float z;
    }
}