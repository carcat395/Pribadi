using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Test : MonoBehaviour
{
    public static Test instance;
    public Action<int[], Dictionary<string, int>> onUpload;
    public int id;
    public bool conventionalTest;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        transform.GetComponentInChildren<Button>().onClick.AddListener(Upload);
    }

    private void Upload()
    {
        int[] result = new int[7];
        Dictionary<string, int> minigameResult = new Dictionary<string, int>();
        int i = 0;

        foreach(Transform child in GetComponent<ScrollRect>().content)
        {
            Debug.Log(i);
            if(i >= 24) continue;

            TMP_InputField inputField = child.GetComponentInChildren<TMP_InputField>();
            Debug.Log(minigameResult);
            Debug.Log(child.name);
            Debug.Log(child.GetComponentInChildren<TMP_InputField>());

            minigameResult.Add(child.name, int.Parse(inputField.text));

            if(i <= 3)
            {
                result[1] += int.Parse(inputField.text);
            }
            else if(i > 3 && i <= 7)
            {
                result[2] += int.Parse(inputField.text);
            }
            else if(i > 7 && i <= 11)
            {
                result[3] += int.Parse(inputField.text);
            }
            else if(i > 11 && i <= 15)
            {
                result[4] += int.Parse(inputField.text);
            }
            else if(i > 15 && i <= 19)
            {
                result[5] += int.Parse(inputField.text);
            }
            else if(i > 19 && i <= 23)
            {
                result[6] += int.Parse(inputField.text);
            }
            
            i++;
        }
        result[0] = result[1] + result[2] + result[3] + result[4] + result[5] + result[6];

        onUpload.Invoke(result, minigameResult);
    }
}

