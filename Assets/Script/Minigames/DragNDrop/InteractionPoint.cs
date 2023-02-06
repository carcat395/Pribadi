using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public Action<int> onEnter;
    public Action onStay;
    public Action onExit;
    [SerializeField] private int _point;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == StringRef.PLAYER_TAG)
        {
            Debug.LogFormat(format: "hit player", context: this);
            onEnter?.Invoke(_point);
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject.tag == StringRef.PLAYER_TAG)
        {
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag == StringRef.PLAYER_TAG)
        {
        }
    }
}
