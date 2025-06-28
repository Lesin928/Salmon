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

    private void FixedUpdate()
    { 
    }
    private void Update()
    { 
    }

    /// <summary>
    /// 키보드 방향키 입력을 처리하는 메서드
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {        
        salmonObject.MoveInput = context.ReadValue<Vector2>();
                
        salmonObject.IsAccelerating = salmonObject.MoveInput.y > 0.1f; //전진 
        salmonObject.IsBraking = salmonObject.MoveInput.y < -0.1f;     //브레이크 
        salmonObject.IsTurningRight = salmonObject.MoveInput.x > 0.1f; //오른쪽 회전 
        salmonObject.IsTurningLeft = salmonObject.MoveInput.x < -0.1f; //왼쪽 회전  
    }

    /// <summary>
    /// 키보드 Space 입력을 처리하는 메서드
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        { 
            Jump();
        }

    }

    public float jumpForce = 8f;  // 세게 튀는 힘

    private void Jump()
    {
        salmonObject.rb.AddForce(Vector3.up * jumpForce + salmonObject.rb.linearVelocity , ForceMode.Impulse);
    }

}