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
        //AudioManager.Instance.PlaySFX("Salmon_SlowSwim");
    }
    private void Update()
    { 
    }

    /// <summary>
    /// 키보드 방향키 입력을 처리하는 메서드
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        salmonObject.MoveInput = input;


        if (input.magnitude > 0.1f)
        {
            AudioManager.Instance.StopSFX("Salmon_SlowSwim");
            AudioManager.Instance.PlaySFX("Salmon_Swim");
            
        }
        else
        {
            AudioManager.Instance.StopSFX("Salmon_Swim");
            AudioManager.Instance.PlaySFX("Salmon_SlowSwim");
        }
        /*
        //레거시 코드: SalmonObject의 MoveInput을 설정하는 부분
        salmonObject.MoveInput = context.ReadValue<Vector2>();
                
        salmonObject.IsAccelerating = salmonObject.MoveInput.y > 0.1f; //전진 
        salmonObject.IsBraking = salmonObject.MoveInput.y < -0.1f;     //브레이크 
        salmonObject.IsTurningRight = salmonObject.MoveInput.x > 0.1f; //오른쪽 회전 
        salmonObject.IsTurningLeft = salmonObject.MoveInput.x < -0.1f; //왼쪽 회전 
        */
    }

    /// <summary>
    /// 키보드 Space 입력을 처리하는 메서드
    /// </summary> 


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        if (salmonObject.IsWaterDetected())
        { 
            salmonObject.TakePush(Vector3.up, salmonObject.JumpForce);
            //salmonObject.PositionPush(Vector3.up, salmonObject.headCheck, salmonObject.JumpForce);
            AudioManager.Instance.PlaySFX("Salmon_Jump");
        }            
        else if (salmonObject.IsGroundDetected()) // 땅에 닿아있을 때는 강하게 대각선 점프
        { 
            salmonObject.TakePush(Vector3.up, salmonObject.JumpForce*0.7f);
            salmonObject.PositionPush(salmonObject.desiredDirection, salmonObject.tailCheck, salmonObject.JumpForce); 
        }   
    }
}