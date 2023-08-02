using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;

[RequireComponent(typeof(Image))]
public class Blocker : MonoBehaviour
{
    [Header("Duration of text on screen before continuing fade (in ms)")]
    [SerializeField] private int _inbetweenTextDuration;
    [SerializeField] private Button _closeBlockerButton;
    [SerializeField] private TMP_Text _minigameDescription;
    private Transform _blockerContent;

    
    private Dictionary<string, string> _minigameDescriptions = new Dictionary<string, string>();
    private string _filePath = "Minigame_Descriptions";


    private void Start()
    {
        _blockerContent = transform.GetChild(0);
        GameManager.instance.onStartTransition += Fade;
        GameManager.instance.onUpdateTransitionText += UpdateText;
        gameObject.SetActive(false);
        _minigameDescriptions = GetTextContent(_filePath);
        _closeBlockerButton.onClick.AddListener(() => 
        {
            _closeBlockerButton.interactable = false;
            GameManager.instance.onStartTransition.Invoke(true, GameManager.instance.fadeDuration);
        });
        GameManager.instance.onLoadedMinigame += (string minigameName) =>
        {
            UpdateMinigameDescription(minigameName);
            _closeBlockerButton.interactable = true;
        };
    }

    private async void Fade(bool fadeIn, float _fadeDuration)
    {
        float timeElapsed = 0;
        Image image = GetComponent<Image>();

        if(fadeIn)
        {
        Debug.Log("fade");
            // while(image.color.a > 0)
            // {
            //     Debug.Log(image.color.a );
            //     var tempColor = image.color;
            //     tempColor.a -= incrementValue;
            //     image.color = tempColor;

            //     await UniTask.Delay(10);
            // }
            transform.GetChild(0).gameObject.SetActive(false);

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
        Debug.Log("fade");
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
        
        //transform.GetChild(0).gameObject.SetActive(true);
        GameManager.instance.onTransitionFinish?.Invoke();
    }

    private async void UpdateText(string title, string content)
    {
        _blockerContent.gameObject.SetActive(true);
        TMP_Text[] texts = _blockerContent.GetComponentsInChildren<TMP_Text>(true);
        texts[0].text = title;
        texts[1].text = content;
        Debug.Log("transition delay: " + _inbetweenTextDuration);

        await UniTask.Delay(0);
        
        //_blockerContent.gameObject.SetActive(false);
        GameManager.instance.onTransitionFinish?.Invoke();
    }

    private Dictionary<string, string> GetTextContent(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(targetFile.text);
    }

    private void UpdateMinigameDescription(string minigameName)
    {
        Debug.Log(minigameName);
        _minigameDescription.text = _minigameDescriptions[minigameName];
    }
}
