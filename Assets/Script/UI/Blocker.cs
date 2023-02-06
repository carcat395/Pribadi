using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

[RequireComponent(typeof(Image))]
public class Blocker : MonoBehaviour
{
    [Header("Duration of text on screen before continuing fade (in ms)")]
    [SerializeField] private int _inbetweenTextDuration;
    private Transform _blockerContent;

    private void Start()
    {
        _blockerContent = transform.GetChild(0);
        GameManager.instance.onStartTransition += Fade;
        GameManager.instance.onUpdateTransitionText += UpdateText;
        gameObject.SetActive(false);
    }

    private async void Fade(bool fadeIn, float _fadeDuration)
    {
        float timeElapsed = 0;
        Image image = GetComponent<Image>();

        if(fadeIn)
        {
            // while(image.color.a > 0)
            // {
            //     Debug.Log(image.color.a );
            //     var tempColor = image.color;
            //     tempColor.a -= incrementValue;
            //     image.color = tempColor;

            //     await UniTask.Delay(10);
            // }

            while (timeElapsed < _fadeDuration)
            {
                var tempColor = image.color;
                tempColor.a = Mathf.Lerp(image.color.a, 0, timeElapsed / _fadeDuration);
                image.color = tempColor;
                //_fadeVolume.weight = Mathf.Lerp(startWeight, endWeight, timeElapsed / _fadeDuration);
                timeElapsed += Time.deltaTime;

                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }

            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            // while(image.color.a < 1)
            // {
            //     var tempColor = image.color;
            //     tempColor.a += incrementValue;
            //     image.color = tempColor;
            //     Debug.Log(image.color.a );
                
            //     await UniTask.Delay(10);
            //     Debug.Log(":)");
            // }

            while (timeElapsed < _fadeDuration)
            {
                var tempColor = image.color;
                tempColor.a = Mathf.Lerp(image.color.a, 1, timeElapsed / _fadeDuration);
                image.color = tempColor;
                //_fadeVolume.weight = Mathf.Lerp(startWeight, endWeight, timeElapsed / _fadeDuration);
                timeElapsed += Time.deltaTime;

                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }
        }
        
        GameManager.instance.onTransitionFinish?.Invoke();
    }

    private async void UpdateText(string title, string content)
    {
        _blockerContent.gameObject.SetActive(true);
        TMP_Text[] texts = _blockerContent.GetComponentsInChildren<TMP_Text>(true);
        texts[0].text = title;
        texts[1].text = content;
        Debug.Log("transition delay: " + _inbetweenTextDuration);

        await UniTask.Delay(_inbetweenTextDuration);
        
        _blockerContent.gameObject.SetActive(false);
        GameManager.instance.onTransitionFinish?.Invoke();
    }
}
