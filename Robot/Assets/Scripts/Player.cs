using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MapObject
{
    #region Objects
    public float maxSpeed;
    public AnimationCurve jumpCurve;

    private Animator _animator;
    private CharacterController _controller;

    private PlayerState _state;
    private PlayerState _prevState;
    private float _runAceleration;
    private float _speed;
    private Coroutine _moveCorrutine;
    private bool isOnAir;

    #endregion

    #region Properties
    public PlayerState State
    {
        get => _state;
        private set
        {
            _prevState = _state;
            _state = value;
        }
    }
    #endregion

    void Start()
    {
        location = Location.Center;
        _animator = GetComponent<Animator>();
        _controller = this.GetComponent<CharacterController>();
    }

    void Update()
    {
        OnState_Update();
        Move();
    }

    #region Character Actions
    public void Run()
    {
        _animator.SetTrigger("Normal");
        this.State = PlayerState.Running;
    }
    public void Move_Right()
    {
        if (this.State != PlayerState.Running)
            return;

        if (base.location == Location.Right)
            return;

        _animator.SetTrigger("Jump");
        this.State = PlayerState.MovingRight;
        _moveCorrutine = StartCoroutine(MovePlayerToRight());
    }
    public void Move_Left()
    {
        if (this.State != PlayerState.Running)
            return;

        if (base.location == Location.Left)
            return;

        _animator.SetTrigger("Jump");
        this.State = PlayerState.MovingLeft;
        _moveCorrutine = StartCoroutine(MovePlayerToLeft());
    }
    public void Jump()
    {
        _animator.SetTrigger("Jump Over");
        this.State = PlayerState.Jumping;
    }
    private void Die()
    {
        _speed = 0;
        _runAceleration = 0;
        _animator.SetTrigger("Hit");
        this.State = PlayerState.Dying;
    }
    #endregion

    #region On State
    private void OnState_Update()
    {
        switch (this.State)
        {
            case PlayerState.Idle:
                OnState_Idle();
                break;
            case PlayerState.Walking:
                OnState_Walk();
                break;
            case PlayerState.Running:
                OnState_Run();
                break;
        }
    }
    private void OnState_Idle()
    {
        if (_runAceleration == 0)
            return;

        _runAceleration = Mathf.Max(_runAceleration - Time.deltaTime, 0);
        _animator.SetFloat("Blend", _runAceleration);
    }
    private void OnState_Walk()
    {
        if (_runAceleration == 0.25f)
            return;

        _runAceleration = this._prevState == PlayerState.Idle ? Mathf.Min(_runAceleration + Time.deltaTime, 0.25f) : Mathf.Max(_runAceleration - Time.deltaTime, 0.25f);
        _animator.SetFloat("Blend", _runAceleration);
    }
    private void OnState_Run()
    {
        if (_runAceleration > maxSpeed)
            return;

        _runAceleration = Mathf.Min(_runAceleration + Time.deltaTime, 1);
        _animator.SetFloat("Blend", _runAceleration);
    }
    #endregion

    #region Animation Events
    public void OnSlide_Completed()
    {
        this.State = _prevState;
    }
    public void OnJump_Air(int isOnAir)
    {
        this.isOnAir = isOnAir == 1;
    }
    public void OnJump_Completed()
    {
        if (this.State != PlayerState.Dying)
            Run();
    }
    #endregion

    #region Private Methods
    private void Move()
    {
        _speed += _runAceleration;
        if (_speed > maxSpeed)
            _speed = maxSpeed;

        _controller.Move(Vector3.forward * _speed * Time.deltaTime);
    }
    private IEnumerator MovePlayerToRight()
    {
        Location _destity = base.location == Location.Left ? Location.Center : Location.Right;

        float startPosition = this.transform.position.x;
        float endPosition = _destity == Location.Center ? 0 : 2f;
        float curveFactor = 0;

        while (this.transform.position.x < endPosition)
        {
            curveFactor = Mathf.Min(curveFactor + Time.deltaTime, 1);
            float movement = (endPosition - startPosition) * jumpCurve.Evaluate(curveFactor) * Time.deltaTime * 30;
            _controller.Move(Vector3.right * movement);
            yield return null;
        }

        location = _destity;
    }
    private IEnumerator MovePlayerToLeft()
    {
        Location _destity = base.location == Location.Right ? Location.Center : Location.Left;

        float startPosition = this.transform.position.x;
        float endPosition = _destity == Location.Center ? 0 : -2f;
        float curveFactor = 0;

        while (this.transform.position.x > endPosition)
        {
            curveFactor = Mathf.Min(curveFactor + Time.deltaTime, 1);

            float movement = (endPosition - startPosition) * jumpCurve.Evaluate(curveFactor) * Time.deltaTime * 30;
            _controller.Move(Vector3.right * movement);
            yield return null;
        }

        location = _destity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.State == PlayerState.Jumping && this.isOnAir)
            return;

        if (other.gameObject.CompareTag("Obstacle"))
        {
            var obstacle = other.gameObject.GetComponent<MapObject>();
            if (obstacle.location == base.location)
                this.Die();
        }
    }
    #endregion
}
public enum PlayerState
{
    Idle,
    Walking,
    Running,
    MovingRight,
    MovingLeft,
    Dying,
    Jumping
}
