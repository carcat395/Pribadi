using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenView : MonoBehaviour
{   
    private void Start()
    {
        GameManager.instance.onFinishStartup += () =>
        {
            gameObject.SetActive(false);
        };
    }
}
