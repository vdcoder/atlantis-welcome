using AtlantisWelcome.Networking;
using AtlantisWelcome.World;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace AtlantisWelcome.Networking
{
    public sealed class AtlantisApiClient : MonoBehaviour
    {
        [SerializeField]
        private string baseUrl = "http://localhost:8080";

        public IEnumerator GetWorldSnapshot(
            Action<WorldSnapshotDto> onSuccess,
            Action<string> onFailure)
        {
            var url = $"{baseUrl.TrimEnd('/')}/api/world";

            using var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onFailure?.Invoke(
                    $"GET {url} failed: {request.responseCode} " +
                    request.error);

                yield break;
            }

            try
            {
                var json = request.downloadHandler.text;
                var snapshot =
                    JsonUtility.FromJson<WorldSnapshotDto>(json);

                if (snapshot == null)
                {
                    onFailure?.Invoke("The world endpoint returned an empty snapshot.");
                    yield break;
                }

                if (snapshot.world == null)
                {
                    onFailure?.Invoke(
                        $"The snapshot did not contain a world object. JSON: {json}");
                    yield break;
                }

                if (snapshot.world.entities == null)
                {
                    onFailure?.Invoke(
                        $"The world did not contain an entities array. JSON: {json}");
                    yield break;
                }

                onSuccess?.Invoke(snapshot);
            }
            catch (Exception exception)
            {
                onFailure?.Invoke(
                    $"Could not deserialize the world snapshot: " +
                    exception.Message);
            }
        }

        public IEnumerator MoveEntity(
            string actorId,
            string entityId,
            Vector3 destination,
            Action<MoveEntityResultDto>? onSuccess,
            Action<string>? onFailure)
        {
            var url =
                $"{baseUrl.TrimEnd('/')}/api/world/entities/" +
                $"{UnityWebRequest.EscapeURL(entityId)}/move";

            var requestDto = new MoveEntityRequestDto
            {
                actorId = actorId,
                entityId = entityId,
                destination = new PositionDto
                {
                    x = destination.x,
                    y = destination.y,
                    z = destination.z
                }
            };

            var json = JsonUtility.ToJson(requestDto);

            Debug.Log($"Move request JSON: {json}");

            var body = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest(
                url,
                UnityWebRequest.kHttpVerbPOST);

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onFailure?.Invoke(
                    $"POST {url} failed: " +
                    $"{request.responseCode} {request.error}\n" +
                    request.downloadHandler.text);

                yield break;
            }

            try
            {
                var result = JsonUtility.FromJson<MoveEntityResultDto>(
                    request.downloadHandler.text);

                if (result == null)
                {
                    onFailure?.Invoke(
                        "The movement endpoint returned an empty result.");

                    yield break;
                }

                onSuccess?.Invoke(result);
            }
            catch (Exception exception)
            {
                onFailure?.Invoke(
                    "Could not deserialize movement response: " +
                    exception.Message);
            }
        }
    }
}