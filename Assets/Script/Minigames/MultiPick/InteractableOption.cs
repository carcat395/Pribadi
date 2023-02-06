using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableOption : MonoBehaviour
{
    public bool isPicked {get; set;}

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(UpdateStatus);
    }

    private void UpdateStatus()
    {
        isPicked = !isPicked;
    }
}
