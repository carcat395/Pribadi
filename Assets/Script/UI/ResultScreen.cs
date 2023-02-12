using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    private ResultScreenView _view;
    private Dictionary<string, string> _resultContent = new Dictionary<string, string>();
    private string _filePath = "Result_Content";

    private void Start()
    {
        _view = GetComponent<ResultScreenView>();
        GameManager.instance.onShowResult += UpdateResult;
        _view.continueButton.onClick.AddListener(GameManager.instance.CloseResult);

        GameManager.instance._resultScreen = transform.GetChild(0).gameObject;
    }

    private Dictionary<string, string> GetTextContent(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(targetFile.text);
    }

    private void UpdateResult(int[] scores, Dictionary<string, int> itemResults)
    {
        Dictionary<string, string> content = GetTextContent(_filePath);

        for(int i = 1; i < _view.scoreSliders.Length; i++)
        {
            _view.scoreSliders[i].value = scores[i];
            _view.scoreSliders[i].GetComponentInChildren<TMP_Text>().text = new string(scores[i] + " / 20");

            if(scores[i] < 20 * 1/3)
            {
                _view.resultDescriptions[i].text = content[i.ToString()] + content[new string(i + "_Low")];
            }
            else if(scores[i] > 20 * 1/3 && scores[i] < 20 * 2/3)
            {
                _view.resultDescriptions[i].text = content[i.ToString()] + content[new string(i + "_Mid")];
            }
            else if(scores[i] > 20 * 2/3)
            {
                _view.resultDescriptions[i].text = content[i.ToString()] + content[new string(i + "_High")];
            }
        }

        
        _view.scoreSliders[0].value = scores[0];
        _view.scoreSliders[0].GetComponentInChildren<TMP_Text>().text = new string(scores[0] + " / 120");
        Debug.Log("score: " + scores[0]);
        Debug.Log("low threshold: " + GameManager.instance.globalExtraversionAverage * 0.75f);
        Debug.Log("high threshold: " + GameManager.instance.globalExtraversionAverage * 1.25f);
        if(scores[0] < GameManager.instance.globalExtraversionAverage * 0.75f)
        {
            _view.resultDescriptions[0].text = content["0"] + content[new string(0 + "_Low")];
        }
        else if(scores[0] > GameManager.instance.globalExtraversionAverage * 0.75f && scores[0] < GameManager.instance.globalExtraversionAverage * 1.25f)
        {
            _view.resultDescriptions[0].text = content["0"] + content[new string(0 + "_Mid")];
        }
        else if(scores[0] > GameManager.instance.globalExtraversionAverage * 1.25f)
        {
            _view.resultDescriptions[0].text = content["0"] + content[new string(0 + "_High")];
        }
    }
}
