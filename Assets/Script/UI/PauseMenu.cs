using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    private PauseView _view;

    private void Start()
    {
        _view = GetComponent<PauseView>();
        _view.quitButton.onClick.AddListener(GameManager.instance.QuitCurrentGame);
        _view.pauseButton.onClick.AddListener(() =>
        {
            _view.pauseButton.gameObject.SetActive(false);
            GameManager.instance.PauseGame();
        });
        _view.resumeButton.onClick.AddListener(() =>
        {
            _view.pauseButton.gameObject.SetActive(true);
            GameManager.instance.ResumeGame();
        });

        GameManager.instance.onStartGame += () =>
        {
            _view.pauseButton.gameObject.SetActive(true);
        };
        GameManager.instance.onShowResult += (int[] result, Dictionary<string, int> itemResults) =>
        {
            _view.pauseButton.gameObject.SetActive(false);
        };
        GameManager.instance._pauseScreen = transform.GetChild(0).GetComponentInChildren<VerticalLayoutGroup>(true).gameObject;
    }
}
