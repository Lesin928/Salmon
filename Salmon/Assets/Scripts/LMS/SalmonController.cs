using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;

/// <summary>
/// 플레이어의 입력을 처리하는 클래스
/// </summary>
public class SalmonController : MonoBehaviour
{
    #region Components
    private SalmonStateMachine stateMachine;
    private SalmonAnimation salmonAnimation;
    private SalmonObject salmonObject;
    #endregion

    private void Awake()
    {
        salmonAnimation = GetComponentInParent<SalmonAnimation>();
        salmonObject = GetComponent<SalmonObject>();
    }

    private void FixedUpdate() //나중에 무브 스테이트로 이동
    { 
    }
    private void Update()
    { 
    }

    /// <summary>
    /// 키보드 좌우 방향키 입력을 처리하는 메서드
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    { 
        salmonObject.MoveInput = context.ReadValue<Vector2>();

        salmonObject.IsAccelerating = salmonObject.MoveInput.y > 0.1f;
        salmonObject.IsBraking = salmonObject.MoveInput.y < -0.1f;

    }

    /// <summary>
    /// 키보드 C 입력을 처리하는 메서드
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    { 
    }

    /// <summary>
    /// 키보드 Space 입력을 처리하는 메서드
    /// </summary>
    public void OnDash(InputAction.CallbackContext context)
    { 
    }
     
}