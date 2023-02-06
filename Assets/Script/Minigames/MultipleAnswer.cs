using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleAnswer : BaseMinigame
{
    [SerializeField] private Button _submit;
    [SerializeField] private ToggleGroup _toggleGroup;

    private void Start()
    {
        _submit.onClick.AddListener(SubmitAnswer);
    }

    private void SubmitAnswer()
    {
        if(!_toggleGroup.AnyTogglesOn()) return;
        
        point = int.Parse(_toggleGroup.GetFirstActiveToggle().gameObject.name);
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
        onFinishMinigame.Invoke(this);
    }
}
