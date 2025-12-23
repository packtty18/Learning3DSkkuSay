using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Start()
    {
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        AsyncOperation ao =  SceneManager.LoadSceneAsync(2);

        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            _progressBar.value = ao.progress;
            _progressText.text = $"{ao.progress * 100}%";

            if(ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
