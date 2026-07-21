using TMPro;
using AtlantisWelcome.World;
using UnityEngine;

namespace AtlantisWelcome.UI
{
    public sealed class WorldStatusView : MonoBehaviour
    {
        [SerializeField]
        private WorldSnapshotLoader worldLoader;

        [SerializeField]
        private TMP_Text revisionText;

        private void Update()
        {
            revisionText.text =
                $"World revision: {worldLoader.CurrentRevision}";
        }
    }
}