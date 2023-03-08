using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptions : BaseMinigame
{
    [SerializeField] private List<Button> _dialogueOptions;

    private void Start()
    {
        _onTimerFinished += () =>
        {
            UpdatePoint(1);
        };
        
        foreach(Button option in _dialogueOptions)
        {
            option.onClick.AddListener(() =>
            {
                _dialogueOptions[0].interactable = false;
                _dialogueOptions[1].interactable = false;
                if(_dialogueOptions.IndexOf(option) == 0)
                {
                    Debug.LogFormat(format:_dialogueOptions[1].transform.parent.gameObject.name ,context:_dialogueOptions[1].transform.parent);
                    Destroy(_dialogueOptions[1].transform.parent.gameObject);
                }
                else
                {
                    Debug.LogFormat(format:_dialogueOptions[0].transform.parent.gameObject.name,context:_dialogueOptions[0].transform.parent);
                    Destroy(_dialogueOptions[0].transform.parent.gameObject);
                }
                UpdatePoint(int.Parse(option.gameObject.name));
            });
        }
    }

    private void UpdatePoint(int value)
    {
        point = value;
        if(negativeKeyed)
        {
            switch(point){
                case 1:
                    point = 5;
                    break;
                case 2:
                    point = 4;
                    break;
                case 4:
                    point = 2;
                    break;
                case 5:
                    point = 1;
                    break;
            }
        }

        if(point == 1)
            StartEndingCutscene(0);
        else
            StartEndingCutscene(1);
    }
}
