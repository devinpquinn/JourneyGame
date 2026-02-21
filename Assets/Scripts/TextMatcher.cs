using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextMatcher : MonoBehaviour
{
    [SerializeField] private TMP_Text target;

    private TMP_Text selfText;

    private void Awake()
    {
        selfText = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        if (selfText == null || target == null)
        {
            return;
        }

        if (selfText.text != target.text)
        {
            selfText.text = target.text;
        }
    }
}