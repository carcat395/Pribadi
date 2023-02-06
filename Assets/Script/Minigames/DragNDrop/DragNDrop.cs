using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDrop : BaseMinigame
{
    [SerializeField] private InteractionPoint[] _interactionPoints;
    
    private void Start()
    {
        _onTimerFinished += () =>
        {
            FinishMinigame();
        };

        foreach(InteractionPoint interactionPoint in _interactionPoints)
        {
            interactionPoint.onEnter += (value) =>
            {
                point = value;
                if(point == 1)
                    StartEndingCutscene(0);
                else
                    StartEndingCutscene(1);
            };
        }
    }
}
