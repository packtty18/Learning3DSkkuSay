using Febucci.TextAnimatorForUnity.TextMeshPro;
using UnityEngine;


public class TextAnimatorBase : MonoBehaviour
{
    [SerializeField] private TextAnimator_TMP _textAnimator;

    public void ShowText(string text)
    {
        _textAnimator.gameObject.SetActive(true);
        _textAnimator.SetText(text);
    }

    public void HideText()
    {
        _textAnimator.SetText(string.Empty);
        _textAnimator.gameObject.SetActive(false);
    }
}
