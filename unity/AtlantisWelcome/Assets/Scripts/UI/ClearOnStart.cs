using UnityEngine;
using TMPro;

public class ClearOnStart : MonoBehaviour
{
    [SerializeField]
    private TMP_Text clearThisText;

    private void Awake()
    {
        if (clearThisText == null)
        {
            clearThisText = GetComponent<TMP_Text>();
        }

        if (clearThisText != null)
        {
            clearThisText.text = string.Empty;
        }
    }
}