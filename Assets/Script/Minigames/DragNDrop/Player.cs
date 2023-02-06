using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    enum PlayerIdleAnimation
    {
        PlayerStand,
        PlayerSit,
        PlayerAvoidContactIdle,
        TalkToALotOfPeopleIdle,
        LikeToTakeItEasyIdle,
        TakesChargeIdle
    }

    [SerializeField] private PlayerIdleAnimation idleAnim;
    private Animator anim;
    [SerializeField] private BaseMinigame _baseMinigame;
    private Collider2D _collider;

    [SerializeField] private float _touchSensitivity = 1;
    [SerializeField] private bool _lockHorizontalMove;
    [SerializeField] private bool _lockVerticalMove;
    
    public Action<Vector2, Vector2> OnDragPerformed;
    public Action<Vector2> OnPointerUpPerformed;
    public Action<Vector2> OnPointerDownPerformed;
    public Action<Vector2> OnTapPerformed;
    private bool _isDragging = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool(idleAnim.ToString(), true);
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            if(_collider = Physics2D.OverlapPoint(mousePosition))
            {
                Move(mousePosition);
            }
        }
    }

    private void Move(Vector2 delta)
    {
        if(_baseMinigame.inCutscene) return;

        float movementX = transform.position.x;
        float movementY = transform.position.y;

        if(!_lockHorizontalMove)
            movementX = delta.x;
            //movementX = transform.position.x + (delta.x * _touchSensitivity);
            
        if(!_lockVerticalMove)
            movementY = delta.y;
            //movementY = transform.position.y + (delta.y * _touchSensitivity);

        transform.position = new Vector3(movementX, movementY);
    }
    
    #region interface

    public void OnDrag(PointerEventData eventData)
    {
        if(_baseMinigame.inCutscene) return;

        Move(eventData.delta);
        _isDragging = true;
        OnDragPerformed?.Invoke(eventData.position, eventData.delta);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_baseMinigame.inCutscene) return;

        OnPointerUpPerformed?.Invoke(eventData.position);
        if (_isDragging)
        {
            _isDragging = false;
        }
        else
        {
            OnTapPerformed?.Invoke(eventData.position);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_baseMinigame.inCutscene) return;

        OnPointerDownPerformed?.Invoke(eventData.position);
    }

    #endregion
}
