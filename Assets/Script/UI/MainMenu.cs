using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private MainMenuView _view;

    private void Awake()
    {
        GameManager gm = GameManager.instance;
        _view = GetComponent<MainMenuView>();

        _view.newGameButton.onClick.AddListener(() => 
        {
            gm.gameMode = GameManager.GameMode.defaultGame;
            gm.StartGame(true);
        });
        
        _view.startConvetionalTestButton.onClick.AddListener(() => 
        {
            gm.gameMode = GameManager.GameMode.conventionalTest;
            gm.StartGame(true);
        });
        _view.continueButton.onClick.AddListener(() => 
        {
            if(gm._minigameQueue.Count <= 0)
            {
                gm.onLoadProgress += (int minigameCount, int score) =>
                {
                    gm.onLoadProgress = null;
                    GameManager.instance.StartGame(false);
                };
                gm.LoadProgress();
                return;
            }
            GameManager.instance.StartGame(false);
        });
        _view.resultButton.onClick.AddListener(GameManager.instance.ShowResult);
        _view.resetButton.onClick.AddListener(GameManager.instance.ResetData);

        GameManager.instance.onLoadProgress += (int minigameCount, int FirstScore) =>
        {
            if(minigameCount > 0)
                _view.continueButton.gameObject.SetActive(true);

            if(FirstScore > 0)
                _view.resultButton.gameObject.SetActive(true);
        };
        GameManager.instance.onQuitCurrentGame += () => 
        {
            if(!_view.continueButton.gameObject.activeSelf)
                _view.continueButton.gameObject.SetActive(true);
        };
        GameManager.instance.onShowResult += (int[] result, Dictionary<string, int> itemResults) =>
        {
                _view.continueButton.gameObject.SetActive(false);
                _view.resultButton.gameObject.SetActive(true);
        };

        GameManager.instance._mainMenuScreen = transform.GetChild(0).gameObject;
    }
}
