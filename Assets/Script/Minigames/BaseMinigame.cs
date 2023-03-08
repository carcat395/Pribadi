using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

public class BaseMinigame : MonoBehaviour
{
    public enum MinigameType
    {
        Friendliness,
        Gregariousness,
        Assertiveness,
        ActivityLevel,
        ExcitementSeeking,
        Cheerfulness
    }
    
    public MinigameType type;
    public string minigameStyle;
    public string minigameDescription;

    public bool negativeKeyed;
    [SerializeField] private float _duration;
    [SerializeField] private int _timeOutPoint; 
    [SerializeField] private Slider _timerBar;
    private float _timer;
    private bool _isTimerOn;
    protected Action _onTimerFinished;
    public int point {get; set;}
    public Action onStartMinigame;
    public Action<BaseMinigame> onFinishMinigame;
    [SerializeField] private PlayableDirector _timelineStart = null;
    [SerializeField] private PlayableDirector[] _timelineEnds;
    [SerializeField] private Animator _minigameObjectsAnim = null;

    public bool inCutscene {get; set;}

    private void Awake()
    {
        onStartMinigame += () =>
        {
            StartOpeningCutscene();
        };
    }

    protected async void StartOpeningCutscene()
    {
        inCutscene = true;
        Debug.Log(_timelineStart.playableAsset.duration);
        float delay = _timelineStart? (Mathf.Round((float)_timelineStart.playableAsset.duration * 100f) / 100f) * 1000: 0;
        if(_timelineStart != null)
            //_timelineStart.Play();

        await UniTask.Delay((int)delay);

        inCutscene = false;

        //_minigameObjectsAnim.enabled = true;
        // foreach(Transform minigameObject in _minigameObjects.transform)
        // {
        //     Animator anim = _minigameObjects.GetComponent<Animator>();
        //     if(anim == null) return;
        // }
        StartTimer();
    }

    protected async void StartEndingCutscene(int index)
    {
        if(_minigameObjectsAnim)
            _minigameObjectsAnim.enabled = false;
        inCutscene = true;
        if(_timelineEnds[index] != null)
            _timelineEnds[index].Play();
        double delay = _timelineEnds[index]? ((Mathf.Round(((float)_timelineEnds[index].playableAsset.duration * 3/4) * 100f) / 100f) * 1000) : 0;

        await UniTask.Delay((int)delay);

        inCutscene = false;
        FinishMinigame();
    }

    private void FixedUpdate()
    {
        if(!_isTimerOn) return;

        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            if(_timerBar)
                _timerBar.value = Mathf.Clamp(_timer/_duration, 0, 1);
        }
        else
        {
            _isTimerOn = false;
            point = _timeOutPoint;
            _onTimerFinished?.Invoke();
        }
    }

    protected void StartTimer()
    {
        if(_duration == 0) return;

        _timer = _duration;
        _isTimerOn = true;
    }

    protected void FinishMinigame()
    {
        onFinishMinigame.Invoke(this);
    }
}
