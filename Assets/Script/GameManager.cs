using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string userID{get; set;}

    public FirestoreManager firestoreManager {get; private set;}

    //Startup processes
    private int _startupProcessCount = 2;
    private int _finishedStartupProcess;
    public Action onFinishStartupProcess {get; private set;} 
    public Action onFinishStartup;
    public Action onResetData;

    public enum GameMode
    {
        defaultGame,
        conventionalTest
    }
    //Minigames
    public GameMode gameMode;
    public List<GameObject> activeMinigames {get; private set;}
    public List<GameObject> minigames;
    public List<GameObject> conventionalTestItems;
    public Queue<GameObject> _minigameQueue{get; private set;}
    private BaseMinigame _currMinigame;

    //Scores
    private int[] _scores = new int[7];
    private Dictionary<string, int> _itemResults = new Dictionary<string, int>();

    public float globalExtraversionAverage {get;set;}
    public int globalAverageCount {get;set;}

    //UI reference
    [Header("UI Reference")]
    [SerializeField] private Transform _mainGameTransform;
    public GameObject _mainMenuScreen {get; set;}
    public GameObject _pauseScreen {get; set;}
    public GameObject _resultScreen {get; set;}
    
    public Action<int, int> onLoadProgress;
    public Action onFinishLoad;
    public Action onStartGame;
    public Action onQuitCurrentGame;
    public Action onFinishUpload;
    public Action<int[], Dictionary<string, int>> onShowResult;

    //transition
    public Action<bool,float> onStartTransition;
    public Action onTransitionFinish;
    public Action<string, string> onUpdateTransitionText;
    public float fadeDuration = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        firestoreManager = GetComponent<FirestoreManager>();
        onFinishStartupProcess += () =>
        {
            _finishedStartupProcess++;
            if(_finishedStartupProcess >= _startupProcessCount)
            {
                _finishedStartupProcess = 0;
                onFinishStartup?.Invoke();
            }
        };
        onFinishUpload += resetIndividualResult;
        LoadProgress();
    }

    public void StartGame(bool isNewGame)
    {
        Debug.Log(gameMode);
        if(gameMode == GameMode.defaultGame)
        {
            activeMinigames = new List<GameObject>(minigames);
        }
        else
        {
            activeMinigames = new List<GameObject>(conventionalTestItems);
        }

        if(isNewGame)
        {
            _minigameQueue = RandomizeMinigames();
            ResetScore();
        }

        //hapus listener di onTransitionFinish dan isi function yang baru
        onTransitionFinish += () =>
        {
            onTransitionFinish = null;
            _mainMenuScreen.SetActive(false);
            onStartGame?.Invoke();
            LoadMinigame();
        };
        onStartTransition.Invoke(false, fadeDuration);
    }

    private Queue<GameObject> RandomizeMinigames()
    {
        List<GameObject> tempGames = new List<GameObject>(activeMinigames);
        Queue<GameObject> newOrder = new Queue<GameObject>();

        foreach(GameObject minigame in activeMinigames)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempGames.Count);
            newOrder.Enqueue(tempGames[randomIndex]);
            tempGames.RemoveAt(randomIndex);
        }

        return newOrder;
    }
    
    private void ResetScore()
    {
        _itemResults.Clear();
        _scores[0] = 0;
        _scores[1] = 0;
        _scores[2] = 0;
        _scores[3] = 0;
        _scores[4] = 0;
        _scores[5] = 0;
        _scores[6] = 0;
    }

    private GameObject FindMinigame(string name)
    {
        GameObject foundMinigame = null;
        foreach(GameObject minigame in activeMinigames)
        {
            if(string.Equals(minigame.name, name))
            {
                foundMinigame = minigame;
            }
        }

        return foundMinigame;
    }

    private void LoadMinigame()
    {
        GameObject minigame = _minigameQueue.Dequeue();
        _currMinigame = minigame.GetComponent<BaseMinigame>();

        onTransitionFinish += () =>
        {
            onTransitionFinish = null;
            _currMinigame = Instantiate(minigame, _mainGameTransform).GetComponent<BaseMinigame>();
            _currMinigame.name = minigame.name;

            _currMinigame.onFinishMinigame += FinishMinigame;
            
            onTransitionFinish += () =>
            {
                onTransitionFinish = null;
                _currMinigame.onStartMinigame?.Invoke();
            };
            onStartTransition.Invoke(true, fadeDuration);
        };
        onUpdateTransitionText.Invoke(_currMinigame.minigameStyle, _currMinigame.minigameDescription);
    }

    private void FinishMinigame(BaseMinigame miniGame)
    {
        _itemResults.Add(miniGame.gameObject.name, miniGame.point);
        switch(miniGame.type)
        {
            case BaseMinigame.MinigameType.Friendliness:
                _scores[1] += miniGame.point;
                break;
            case BaseMinigame.MinigameType.Gregariousness:
                _scores[2] += miniGame.point;
                break;
            case BaseMinigame.MinigameType.Assertiveness:
                _scores[3] += miniGame.point;
                break;
            case BaseMinigame.MinigameType.ActivityLevel:
                _scores[4] += miniGame.point;
                break;
            case BaseMinigame.MinigameType.ExcitementSeeking:
                _scores[5] += miniGame.point;
                break;
            case BaseMinigame.MinigameType.Cheerfulness:
                _scores[6] += miniGame.point;
                break;
        }

        //hapus listener di onTransitionFinish dan isi function yang baru
        onTransitionFinish += () =>
        {
            onTransitionFinish = null;
            GameObject.Destroy(miniGame.gameObject);
            if(_minigameQueue.Count == 0)
            {
                _currMinigame = null;
                SaveProgress();
                ShowResult();
                return;
            }

            LoadMinigame();
        };
        onStartTransition.Invoke(false, fadeDuration);
    }

    public void ShowResult()
    {
        string result = string.Format("Results:\nFriendliness: {0}\nGregariousness: {1}\nAssertiveness: {2}\nActivity Level: {3}\nExcitement Seeking: {4}\nCheerfulness: {5}\n\nExtraversion: {6}", 
            _scores[1], _scores[2], _scores[3], _scores[4], _scores[5], _scores[6], 
            (_scores[1] + _scores[2] + _scores[3] + _scores[4] + _scores[5] + _scores[6]));
        _scores[0] = _scores[1] + _scores[2] + _scores[3] + _scores[4] + _scores[5] + _scores[6];
        _resultScreen.SetActive(true);
        onShowResult.Invoke(_scores, _itemResults);

        onStartTransition.Invoke(true, fadeDuration);
    }

    public void CloseResult()
    {
        _resultScreen.SetActive(false);
        _mainMenuScreen.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        _pauseScreen.SetActive(false);
    }

    public void QuitCurrentGame()
    {
        Time.timeScale = 1;
        SaveProgress();
        ClearMinigame();
        _pauseScreen.SetActive(false);
        _mainMenuScreen.SetActive(true);

        onQuitCurrentGame?.Invoke();
    }

    private void ClearMinigame()
    {
        foreach(Transform minigame in _mainGameTransform)
        {
            GameObject.Destroy(minigame.gameObject);
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetString(StringRef.USER_ID, userID);
        int index = 0;
        if(_currMinigame != null)
        {
            index++;
            PlayerPrefs.SetString(index.ToString(), _currMinigame.gameObject.name);
        }

        while(_minigameQueue.Count > 0)
        {
            index++;
            BaseMinigame minigame = _minigameQueue.Dequeue().GetComponent<BaseMinigame>();
            PlayerPrefs.SetString(index.ToString(), minigame.gameObject.name);
        }

        PlayerPrefs.SetInt(StringRef.MINIGAMES_LEFT, index);
        PlayerPrefs.SetString(StringRef.GAME_MODE, gameMode.ToString());

        PlayerPrefs.SetInt(StringRef.FRIENDLINESS, _scores[1]);
        PlayerPrefs.SetInt(StringRef.GREGARIOUSNESS, _scores[2]);
        PlayerPrefs.SetInt(StringRef.ASSERTIVENESS, _scores[3]);
        PlayerPrefs.SetInt(StringRef.ACTIVITY_LEVEL, _scores[4]);
        PlayerPrefs.SetInt(StringRef.EXCITEMENT_SEEKING, _scores[5]);
        PlayerPrefs.SetInt(StringRef.CHEERFULNESS, _scores[6]);

        foreach(KeyValuePair<string, int> itemResult in _itemResults)
        {
            Debug.Log(itemResult.Key);
            if(PlayerPrefs.HasKey(itemResult.Key))
                PlayerPrefs.DeleteKey(itemResult.Key);
            PlayerPrefs.SetInt(itemResult.Key, itemResult.Value);
        }

        PlayerPrefs.Save();
        DebugProgress();
    }

    public void LoadProgress()
    {
        userID = PlayerPrefs.HasKey(StringRef.USER_ID)? PlayerPrefs.GetString(StringRef.USER_ID) : "";

        if(PlayerPrefs.HasKey(StringRef.MINIGAMES_LEFT) == false)
        {
            onFinishStartupProcess?.Invoke();
            onLoadProgress?.Invoke(0, 0);
            return;
        } 

        _scores[1] = PlayerPrefs.GetInt(StringRef.FRIENDLINESS, 0);
        _scores[2] = PlayerPrefs.GetInt(StringRef.GREGARIOUSNESS, 0);
        _scores[3] = PlayerPrefs.GetInt(StringRef.ASSERTIVENESS, 0);
        _scores[4] = PlayerPrefs.GetInt(StringRef.ACTIVITY_LEVEL, 0);
        _scores[5] = PlayerPrefs.GetInt(StringRef.EXCITEMENT_SEEKING, 0);
        _scores[6] = PlayerPrefs.GetInt(StringRef.CHEERFULNESS, 0);

        if(PlayerPrefs.GetString(StringRef.GAME_MODE) == GameMode.defaultGame.ToString())
        {
            gameMode = GameMode.defaultGame;
            activeMinigames = new List<GameObject>(minigames);
        }
        else
        {
            gameMode = GameMode.conventionalTest;
            activeMinigames = new List<GameObject>(conventionalTestItems);
        }

        _itemResults.Clear();
        foreach(GameObject minigame in activeMinigames)
        {
            if(PlayerPrefs.HasKey(minigame.name))
            {
                _itemResults.Add(minigame.name, PlayerPrefs.GetInt(minigame.name));
            }
        }

        _minigameQueue = new Queue<GameObject>();
        for(int i = 1; i <= PlayerPrefs.GetInt(StringRef.MINIGAMES_LEFT); i++)
        {
            GameObject minigameToQueue = FindMinigame(PlayerPrefs.GetString(i.ToString()));

            if(minigameToQueue == null)
            {
                return;
            }
            _minigameQueue.Enqueue(minigameToQueue);
        }

        onLoadProgress?.Invoke(PlayerPrefs.GetInt(StringRef.MINIGAMES_LEFT), _scores[1]);
        onFinishStartupProcess?.Invoke();
        DebugProgress();
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        userID = null;

        onResetData?.Invoke();
    }
    
    public void resetIndividualResult()
    {
        foreach(KeyValuePair<string, int> keyValuePair in _itemResults)
        {
            PlayerPrefs.DeleteKey(keyValuePair.Key);
        }

        _itemResults.Clear();
    }

    public void DebugProgress()
    {
        string result = string.Format("Minigame Progress {0}\n\nResults\nFriendliness: {1}\nGregariousness: {2}\nAssertiveness: {3}\nActivity Level: {4}\nExcitement Seeking: {5}\nCheerfulness: {6}\n\nExtraversion: {7}", 
            PlayerPrefs.GetInt(StringRef.MINIGAMES_LEFT),
            _scores[1], _scores[2], _scores[3], _scores[4], _scores[5], _scores[6], 
            (_scores[1] + _scores[2] + _scores[3] + _scores[4] + _scores[5] + _scores[6]));

        foreach(KeyValuePair<string, int> itemResult in _itemResults)
        {
            Debug.Log(itemResult.Key + ": " + itemResult.Value);
        }

        Debug.Log(result);
    }
}
