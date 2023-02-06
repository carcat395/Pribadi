using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class MultiPick : BaseMinigame
{
    [SerializeField] private Transform _toggleParent;

    private void Start()
    {
        _onTimerFinished += () =>
        {
            WaitForPoint();
        };
    }

    private async void WaitForPoint()
    {
        await UniTask.WaitUntil(CalculatePoint);
        if(point == 1)
            StartEndingCutscene(0);
        else
            StartEndingCutscene(1);
    }

    private bool CalculatePoint()
    {
        List<Toggle> interactableOptions = new List<Toggle>();
        float pickedOptions = 0;
        foreach(Toggle option in _toggleParent.GetComponentsInChildren<Toggle>(true))
        {
            if(option.isOn)
                pickedOptions++;
            interactableOptions.Add(option);
        }

        if(pickedOptions <= (float)interactableOptions.Count * 1/5)
        {
            if(negativeKeyed)
                point = 5;
            else
                point = 1;
        }
        else if(pickedOptions > (float)interactableOptions.Count * 1/5 && pickedOptions < (float)interactableOptions.Count * 2/5)
        {
            if(negativeKeyed)
                point = 4;
            else
                point = 2;
        }
        else if(pickedOptions > (float)interactableOptions.Count * 2/5 && pickedOptions < (float)interactableOptions.Count * 3/5)
        {
            point = 3;
        }
        else if(pickedOptions > (float)interactableOptions.Count * 3/5 && pickedOptions < (float)interactableOptions.Count * 4/5)
        {
            if(negativeKeyed)
                point = 2;
            else
                point = 4;
        }
        else if(pickedOptions >= (float)interactableOptions.Count * 4/5)
        {
            if(negativeKeyed)
                point = 1;
            else
                point = 5;
        }

        return true;
    }
}
