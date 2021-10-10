using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ProgressSceneLoader : MonoBehaviour
{
    [SerializeField] TMP_Text progressText;
    [SerializeField] Slider slider;

    AsyncOperation operation;
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>(true);
        DontDestroyOnLoad(gameObject);
    }
    public void LoadScene(string sceneName)
    {
        UpdateProgressUI(0);
        canvas.gameObject.SetActive(true);

        StartCoroutine(BeginLoad(sceneName));
    }

    IEnumerator BeginLoad(string sceneName)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);

        //while (!operation.isDone)
        //{
        //    UpdateProgressUI(operation.progress);
        //    yield return null;
        //}     

        while (!operation.isDone)
        {
            if (operation.progress < 0.5f)
            {
                UpdateProgressUI(operation.progress);
                yield return null;
            }
            else if(operation.progress < 0.85f)
            {
                // Do fake show
                float fakeProgress = 0.5f;
                float fakeWaitDuration = 3f;
                float progressRemaining = 1f - fakeProgress;
                int NStep = 10;
                float deltaTime = progressRemaining / fakeWaitDuration / ((float)NStep);
                while (fakeProgress < 1f)
                {
                    fakeProgress += deltaTime;
                    fakeProgress = Mathf.Clamp(fakeProgress, 0, 1);
                    UpdateProgressUI(fakeProgress);
                    yield return new WaitForSeconds(fakeWaitDuration / ((float)NStep));
                }
            }
            else
            {
                UpdateProgressUI(1f);
                yield return null;
            }
        }

    }

    void UpdateProgressUI(float progress)
    {
        slider.value = progress;
        progressText.text = string.Concat((int)(progress * 100f), "%");
    }
}
