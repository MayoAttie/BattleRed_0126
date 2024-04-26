using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchPadController;
using static HandlePauseTool;
using static UI_UseToolClass;
using static UseTool;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class CharacterControlMng : Subject, Observer
{
    #region 변수
    CharacterController controller;      // 캐릭터 컨트롤러
    Transform groundCheck;               // 지면 체크를 위한 위치 정보를 저장하는 변수
    [SerializeField] LayerMask groundMask;                // 지면을 나타내는 레이어 정보를 저장하는 변수
    TouchPadController TouchController;
    Transform originParents;                        // 기존의 부모객체 저장 변수

    Vector3 velocity;
    public bool isBattle;                              // 전투중인지 체크
    bool isReverseGround;                       // 반중력 체크
    bool isEndReverseAnimation;                 // 반중력 회전 애니메이션 종료
    bool isGrounded;                            // 지면인지 체크
    [SerializeField]bool isJump;                                // 점프중인지 체크
    bool isBlinking;                            // 회피중인지 체크
    bool isBlinkCoolTimeFleg;                   // 쿨타임 코루틴함수 제어 플래그
    bool isBlinkStart;                          // 블링크 애니메이션이 시작되었는지 체크
    

    float groundDistance = 0f;                // 지면과의 거리
    float zPos;                                 // 제어용 좌표 값
    float xPos;                                 // 제어용 좌표 값
    float runX;                                 // 달리기 제어용 변수
    float runZ;                                 // 달리기 제어용 변수
    float rotationSpeed = 100f;                 // 캐릭터 회전 속도
    float gravity = -9.81f;                     // 중력
    float fBliknkCoolTime = 3.0f;               // 회피기 충전 주기
    float jumpSpeed = 10f;                      // 점프력 변수
    float verticalSpeed = 0f;                   // 수직 속력 변수
    float yDegreeSave;                          // y축 각도를 저장하기 위한 변수(달리기에 사용)
    
    int nBlinkNumber = 2;                                       // 회피기 숫자
    Coroutine blinkCoolTimeCoroutine;                           // Coroutine 객체를 저장할 변수
    Coroutine jumpFinishCoroutine;                              // 점프 애니메이션 관리용 코루틴
    CancellationTokenSource moveSoundControlTokenSource;        // 무브 사운드용 유니테스크 인스턴스
    CharacterManager characMng;                                 // 캐릭터 매니저 싱글턴
    e_BlinkPos blinkpos;
    #endregion

    public bool debugging= false;

    #region 구조체
    public enum e_BlinkPos
    {
        None,
        Front,
        Back,
        Right,
        Left,
        Max
    }
    #endregion


    private void Awake()
    {
        isReverseGround = false;
        isEndReverseAnimation = false;
        Physics.gravity = new Vector3(0,gravity,0);
        controller = gameObject.GetComponent<CharacterController>();
        groundCheck = gameObject.transform;
        isBlinkCoolTimeFleg = false;
        isJump       = false;
        isBlinking   = false;
        isBattle     = false;
        blinkpos = e_BlinkPos.None;
    }

    void Start()
    {
        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        if (body != null)
            Destroy(body);

        characMng = CharacterManager.Instance;
        TouchController = GameObject.FindGameObjectWithTag("TouchPad").GetComponent<TouchPadController>();

        InvokeRepeating("RemoveRigidBody", 0f, 2.5f);
    }

    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!characMng.IsControl)
            return;

        ///Debug.Log("zpos: " + zPos + " xpos: " + xPos);

        isBattle = characMng.GetIsBattle();
        // 코루틴이 실행 중이지 않은 경우에만 코루틴을 시작.
        if (blinkCoolTimeCoroutine == null && nBlinkNumber<=0)
        {
            blinkCoolTimeCoroutine = StartCoroutine(BlinkCoolTimeReset());
        }
        GravityFunc();                      // 중력 함수
        ControllerGetInputData();           // 컨트롤러 값 호출 함수
        if (!isBlinking)
        {
            if (isBattle)
            {
                groundDistance = isReverseGround ? 3.6f : 3f;
                RunCharacterFunction();     // 달리기 함수
            }
            else
            {
                groundDistance = isReverseGround ? 2.2f : 0.3f;
                MoveCharacterFunction();    // 걷기 함수
            }
        }
        RotateCharacter();                  // 캐릭터 회전 함수
        JumpCharacterFunction();            // 점프 함수

    }

    // 조이스틱 컨트롤러, xy좌표 값 Get 함수
    void ControllerGetInputData()
    {
        zPos = JoyStickController.Instance.GetVerticalValue(); // 수직 입력값 (전진/후진)
        xPos = JoyStickController.Instance.GetHorizontalValue(); // 수평 입력값 (좌우 이동)

        //Debug.Log(nameof(zPos)+":" +zPos);
        //Debug.Log(nameof(xPos) + ":" + xPos);

        // 회피 중이 아닐 때, 걷기 혹은 달리기 상태로 캐릭터의 상태를 지정함
        if (!isBlinking && !isJump)
        {
            if (!isBattle)
                characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_WALK);
            else
            {
                if (zPos > F_Epsilon || xPos > F_Epsilon)
                    characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_RUN);
            }

        }
    }

    //중력함수
    void GravityFunc()
    {
        if (isJump)
            return;

        // isReverseGround가 true이면 y 축의 위쪽 방향으로 Ray를 쏘도록 설정
        Vector3 rayDirection = isReverseGround ? Vector3.up : Vector3.down;
        // Physics.Raycast를 사용하여 Ray를 발사하여 지면과의 충돌을 체크
        RaycastHit hitInfo;
        bool isRaycastHit = Physics.Raycast(groundCheck.position, rayDirection, out hitInfo, groundDistance, groundMask);


        // 레이캐스트 방향을 시각적으로 확인하기 위해 DrawRay 함수 사용
        Color rayColor = isRaycastHit ? Color.green : Color.red; // 충돌 여부에 따라 색상 지정
        DrawRay(groundCheck.position, rayDirection * groundDistance, rayColor, 0.1f); // 충돌 여부에 따라 색상이 다른 레이 그리기


        // isRaycastHit으로 지면과의 충돌 여부를 확인하여 isGrounded를 업데이트
        isGrounded = isRaycastHit;

        // isGrounded가 true이고 velocity.y가 음수일 때 지면에 닿았음을 확인하여 y 속도를 초기화
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = isReverseGround ? 4f : -4f;
        }



        // 중력 적용
        velocity.y += (isReverseGround ? -gravity : gravity) * Time.deltaTime;

        // controller.Move로 이동 처리
        controller.Move(velocity * Time.deltaTime);
    }

    #region 걷기

    // 워크 애니메이션 함수
    void MoveCharacterFunction()
    {
        if (isBattle)
            return;

        // 애니메이션을 실행할 때 필요한 파라미터 설정
        characMng.AnimatorFloatValueSetter(zPos, xPos);

        GravityFunc();

        // 객체 이동
        Vector3 move = transform.right * xPos + transform.forward * zPos;
        if (isEndReverseAnimation)
            controller.Move((move * 5 - velocity) * Time.deltaTime); // 중력이 적용된 이동
        else
            controller.Move((move * 5 + velocity) * Time.deltaTime); // 중력이 적용된 이동

        // x,y 값이 0에 가까우면, 이동을 멈추고 iDle상태로 바꿈
        if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f) && !isJump)
        {
            yDegreeSave = 0;
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_Idle);
            controller.Move(Vector3.zero);
            CancelMoveSoundControl();
        }
        else
            StartMoveSoundControl(1.4f);
    }

    #endregion

    #region 사운드처리
    private void StartMoveSoundControl(float time)
    {
        if (moveSoundControlTokenSource == null)
        {
            moveSoundControlTokenSource = new CancellationTokenSource();
            MoveSoundControl(time, moveSoundControlTokenSource.Token).Forget();
        }
    }

    private void CancelMoveSoundControl()
    {
        if (moveSoundControlTokenSource != null)
        {
            moveSoundControlTokenSource.Cancel();
            moveSoundControlTokenSource.Dispose();
            moveSoundControlTokenSource = null;
        }
    }

    private async UniTaskVoid MoveSoundControl(float time, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!Mathf.Approximately(zPos, 0f) || !Mathf.Approximately(xPos, 0f))
            {
                SoundManager.Instance.PlayEffect(gameObject, SoundManager.eTYPE_EFFECT.walk, Mathf.Infinity, time, false).Forget();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: token);
        }
    }
    #endregion

    #region 점프

    // 점프버튼 함수
    public void JumpCommand()
    {
        if (isGrounded)
        {
            isJump = true;
            verticalSpeed = jumpSpeed;
        }
    }

    void DrawRay(Vector3 origin, Vector3 direction, Color color, float duration)
    {
        Debug.DrawRay(origin, direction, color, duration);
    }
    // 점프 애니메이션 함수
    void JumpCharacterFunction()
    {
        // isReverseGround가 true이면 y 축의 위쪽 방향으로 Ray를 쏘도록 설정
        Vector3 rayDirection = isReverseGround ? Vector3.up : Vector3.down;
        // Physics.Raycast를 사용하여 Ray를 발사하여 지면과의 충돌을 체크
        RaycastHit hitInfo;
        bool isRaycastHit = Physics.Raycast(groundCheck.position, rayDirection, out hitInfo, groundDistance, groundMask);

        isGrounded = isRaycastHit;

        if (isGrounded && isJump)
        {
            var mng = characMng.AttackMng;
            mng.FlagValueReset();  
            characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_JUMP);

            verticalSpeed -= (isReverseGround == true ? gravity : -gravity) * Time.deltaTime;

            Vector3 verticalMove = new Vector3(0f, verticalSpeed, 0f);
            controller.Move(verticalMove * Time.deltaTime);

            if (jumpFinishCoroutine == null)
                jumpFinishCoroutine = StartCoroutine(JumFinish());
        }

        if(!isJump)
        {
            if (isReverseGround)
                velocity.y -= gravity * Time.deltaTime;
            else
                velocity.y += gravity * Time.deltaTime;
        }

    }
    IEnumerator JumFinish()
    {
        float time = GetAnimationLength(characMng.GetAnimator(), "Jump");
        yield return new WaitForSeconds(time);
        isJump = false;
        jumpFinishCoroutine = null;
        yDegreeSave = 0;
        yield break;
    }
    #endregion

    #region 회전

    // 캐릭터 회전 함수
    private void RotateCharacter()
    {
        e_TouchSlideDic touchDic;
        touchDic = TouchController.GetDirectionHorizontal();

        Quaternion newRotation = transform.localRotation;  // 현재 로컬 회전값을 저장

        // 만약 수평 방향 입력이 오른쪽(Right)으로 감지된 경우
        if (touchDic == e_TouchSlideDic.Right)
        {
            // 캐릭터를 오른쪽으로 회전시킵니다.
            newRotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);  // y축 회전만 적용
            yDegreeSave = 0;
            CharacterRotate_NotifyForCamera();
        }
        // 만약 수평 방향 입력이 왼쪽(Left)으로 감지된 경우
        else if (touchDic == e_TouchSlideDic.Left)
        {
            // 캐릭터를 왼쪽으로 회전시킵니다.
            newRotation *= Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0);  // y축 회전만 적용
            yDegreeSave = 0;
            CharacterRotate_NotifyForCamera();
        }

        // 회전 중이 아닌 경우에도 x축 회전값을 유지하기 위해 이전 회전값을 복원
        if (isEndReverseAnimation)
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 180);
        else
            newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, newRotation.eulerAngles.z);

        // 새로운 회전값을 적용
        transform.localRotation = newRotation;
    }
    #endregion

    #region 달리기

    //달리기 애니메이션 함수
    private void RunCharacterFunction()
    {
        characMng.AnimatorFloatValueSetter(zPos, xPos);
        GravityFunc();

        if (characMng.GetCharacterClass().GetState() != CharacterClass.eCharactgerState.e_RUN && Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f))
            return;

        if (zPos > 0.3f)
        {
            if (xPos>0.7f)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, GameManager.Instance.MainCamera.transform.eulerAngles.y + 20f, 0f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else if (xPos<-0.7f)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, GameManager.Instance.MainCamera.transform.eulerAngles.y - 20f, 0f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                float degree = GameManager.Instance.MainCamera.transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, degree, transform.eulerAngles.z);
            }

            Vector3 move = transform.right * xPos + transform.forward * zPos;
            // 객체 이동
            if (isEndReverseAnimation)
                controller.Move((move * 12 - velocity) * Time.deltaTime); // 중력이 적용된 이동
            else
                controller.Move((move * 12 + velocity) * Time.deltaTime); // 중력이 적용된 이동

            yDegreeSave = 0;

            // 이동이 멈추면 ATTACK 상태로 전환
            if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f) && !isJump)
            {
                var instance = gameObject.GetComponent<CharacterAttackMng>();
                controller.Move(Vector3.zero);
                instance.OffBattleMode();
                characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
                CancelMoveSoundControl();
            }
            else
                StartMoveSoundControl(0.4f);
        }
        else
        {
            // 좌우 이동값에 따라 회전 각도 설정
            if (xPos < -0.8f) // 좌
            {
                if (yDegreeSave == 0)
                    yDegreeSave = GameManager.Instance.MainCamera.transform.eulerAngles.y - 90f;
            }
            if (xPos > 0.8f) // 우
            {
                if (yDegreeSave == 0)
                    yDegreeSave = GameManager.Instance.MainCamera.transform.eulerAngles.y + 90f;
            }
            if (zPos < -0.8f) // 하
            {
                if (yDegreeSave == 0)
                    yDegreeSave = GameManager.Instance.MainCamera.transform.eulerAngles.y - 180f;
            }
            else
            {
                // left down
                if (xPos<0)
                {
                    if (yDegreeSave == 0)
                        yDegreeSave = GameManager.Instance.MainCamera.transform.eulerAngles.y - 135f;
                }
                // right down
                else
                {
                    if (yDegreeSave == 0)
                        yDegreeSave = GameManager.Instance.MainCamera.transform.eulerAngles.y + 135f;
                }
            }

            // 회전 각도 적용
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yDegreeSave, transform.eulerAngles.z);


            // 이동이 멈추면 ATTACK 상태로 전환
            if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f) && !isJump)
            {
                yDegreeSave = 0;
                var instance = gameObject.GetComponent<CharacterAttackMng>();
                controller.Move(Vector3.zero);
                instance.OffBattleMode();

                float dgree = GameManager.Instance.MainCamera.transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, dgree, transform.eulerAngles.z);
                characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
                CancelMoveSoundControl();
            }
            else
            {
                // 이동 처리
                Vector3 move = transform.forward;
                if (isEndReverseAnimation)
                    controller.Move((move * 12 - velocity) * Time.deltaTime); // 중력이 적용된 이동
                else
                    controller.Move((move * 12 + velocity) * Time.deltaTime); // 중력이 적용된 이동
                StartMoveSoundControl(0.4f);
            }
        }

    }
    //레거시 달리기 애니메이션 함수
    //private void RunCharacterFunction()
    //{

    //    characMng.AnimatorFloatValueSetter(zPos, xPos);
    //    GravityFunc();

    //    if (Mathf.Abs(xPos - 1f) < 0.1f)
    //    {
    //        // 오른쪽으로 방향 전환 (90도 회전)
    //        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + 150f, 0f);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //    else if (Mathf.Abs(xPos + 1f) < 0.1f)
    //    {
    //        // 왼쪽으로 방향 전환 (90도 회전)
    //        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y - 150f, 0f);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //    else if (Mathf.Abs(zPos + 1f) < 0.06f)
    //    {
    //        // 이동 멈추기
    //        xPos = 0f;
    //        zPos = 0f;

    //        var instance = gameObject.GetComponent<CharacterAttackMng>();
    //        instance.ShildAct();
    //        characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
    //    }


    //    Vector3 move = transform.right * xPos + transform.forward * zPos;
    //    // 객체 이동
    //    if (isEndReverseAnimation)
    //        controller.Move((move * 12 - velocity) * Time.deltaTime); // 중력이 적용된 이동
    //    else
    //        controller.Move((move * 12 + velocity) * Time.deltaTime); // 중력이 적용된 이동



    //    // x,y 값이 0에 가까우면, 이동을 멈추고 ATTACK 상태로 바꿈
    //    if (Mathf.Approximately(zPos, 0f) && Mathf.Approximately(xPos, 0f) && !isJump)
    //    {
    //        // 대기 모드 전환을 위한 함수 호출
    //        var instance = gameObject.GetComponent<CharacterAttackMng>();
    //        controller.Move(Vector3.zero);
    //        instance.OffBattleMode();
    //        characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_ATTACK);
    //    }
    //}

    #endregion

    #region 회피기

    private void ActTumblin()
    {
        if (isBlinkStart)
            return;


        // 앞점멸
        if (Mathf.Abs(zPos - 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Front;
            // zPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.forward * 1f;
            controller.Move(moveDirection);
        }
        // 뒷점멸
        else if (Mathf.Abs(zPos + 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Back;
            // zPos 방향으로 -1만큼 이동
            Vector3 moveDirection = -transform.forward * 1f;
            controller.Move(moveDirection);
        }
        // 우점멸
        else if (Mathf.Abs(xPos - 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Right;
            // xPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.right * 1f;
            controller.Move(moveDirection);
        }
        // 좌점멸
        else if (Mathf.Abs(xPos + 1f) < 0.05f)
        {
            blinkpos = e_BlinkPos.Left;
            // xPos 방향으로 -1만큼 이동
            Vector3 moveDirection = -transform.right * 1f;
            controller.Move(moveDirection);
        }
        // 디폴트는 앞점멸
        else
        {
            blinkpos = e_BlinkPos.Front;
            // zPos 방향으로 1만큼 이동
            Vector3 moveDirection = transform.forward * 1f;
            controller.Move(moveDirection);
        }
        NotifyBlinkValue(blinkpos);
        StartCoroutine(BlinkFinish());

        characMng.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_AVOID);
        // 옵저버에게 블링크 값 넘기기
    }

    // 블링크 쿨타임 초기화
    IEnumerator BlinkCoolTimeReset()
    {
        if (!isBlinkCoolTimeFleg)
        {
            isBlinkCoolTimeFleg = true;
            yield return new WaitForSeconds(fBliknkCoolTime);
            nBlinkNumber = 2;

            // 코루틴이 끝났으므로, Coroutine 변수를 null로 초기.
            blinkCoolTimeCoroutine = null;
            isBlinkCoolTimeFleg = false;
        }
    }

    // 블링크 버튼 클릭 이벤트 함수
    public void BlinkClickEvent()
    {
        if (isBlinkStart)
            return;
        if (nBlinkNumber > 0 && !isBlinking)
        {
            isBlinking = true;
            nBlinkNumber--;
            ActTumblin();
        }
    }

    IEnumerator BlinkFinish()
    {
        float time;
        string[] names = { "Tumbling", "L-Tumbling", "R-Tumbling", "B-Tumbling" };
        string name = "";
        switch(blinkpos)
        {
            case e_BlinkPos.Front:
                name = names[0];
                break;
            case e_BlinkPos.Back:
                name = names[3];
                break;
            case e_BlinkPos.Right:
                name = names[2];
                break;
            case e_BlinkPos.Left:
                name = names[1];
                break;
        }
        time = GetAnimationLength(characMng.GetAnimator(), name);
        yield return new WaitForSeconds(time);

        blinkpos = e_BlinkPos.None;
        isBlinking = false;
        isBlinkStart = false;
        yDegreeSave = 0;
        characMng.AttackMng.CallCurtainOff(0.11f);
    }

    public void GetBlinkStartNotify()
    {
        isBlinkStart = true;
    }

    #endregion

    #region 기타이동

    void RemoveRigidBody()
    {
        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        if(body != null) Destroy(body); 
    }

    public void Move_aPoint_to_bPoint(Vector3 aPoint, Vector3 bPoint, float time)
    {
        controller.Move(Vector3.zero);
        controller.velocity.Set(0,0,0);
        controller.enabled = false;
        characMng.IsControl = false;
        StartCoroutine(MoveAtoB_Smoothly(aPoint, bPoint, time,0));
    }
    public void Move_aPoint_to_bPoint(Vector3 aPoint, Vector3 bPoint, float time, float frontWaitTime)
    {
        controller.enabled = false;
        characMng.IsControl = false;
        StartCoroutine(MoveAtoB_Smoothly(aPoint, bPoint, time, frontWaitTime));
    }
    IEnumerator MoveAtoB_Smoothly(Vector3 aPoint, Vector3 bPoint, float time, float frontWaitTime)
    {
        yield return new WaitForSeconds(frontWaitTime);

        float elapsedTime = 0f; // 경과 시간

        Vector3 direction = bPoint - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        while (elapsedTime < time)
        {
            // 경과 시간에 따른 이동 보간
            float t = elapsedTime / time;
            Vector3 newPosition = Vector3.Lerp(aPoint, bPoint, t);

            // 이동
            transform.position = newPosition;

            Quaternion newRotation = transform.localRotation;  // 현재 로컬 회전값을 저장
            // 회전 중이 아닌 경우에도 x축 회전값을 유지하기 위해 이전 회전값을 복원
            if (isEndReverseAnimation)
                newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 180);
            else
                newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
            transform.localRotation = newRotation;

            characMng.IsControl = false;
            controller.enabled = false;
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.useGravity = false;
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            Destroy(body);
        }

        // 종료 시 캐릭터 컨트롤 활성화
        transform.position = bPoint;

        controller.enabled = true;
        controller.Move(Vector3.zero);
        characMng.IsControl = true;

        StartCoroutine(PositionSet(bPoint, 0.3f));

        // 코루틴 종료
        yield break;
    }
    IEnumerator PositionSet(Vector3 pos, float time)
    {
        yield return new WaitForSeconds(time);
        characMng.IsControl = false;
        controller.enabled = false;
        transform.position = pos;
        characMng.IsControl = true;
        controller.enabled = true;
        controller.Move(Vector3.zero);
    }
    public void Move_aPoint_to_bPoint(Vector3 targetPoint)
    {
        CharacterManager.Instance.IsControl = false;
        CharacterManager.Instance.ControlMng.MyController.enabled = false;

        gameObject.transform.position = targetPoint;
        Vector3 direction = targetPoint - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        CharacterManager.Instance.ControlMng.MyController.enabled = true;
        CharacterManager.Instance.IsControl = true;
    }
    public void Move_aPoint_to_bPoint(Vector3 targetPoint, float time)
    {
        StartCoroutine(Move_aPoint_to_bPointFunc(targetPoint, time));
    }

    IEnumerator Move_aPoint_to_bPointFunc(Vector3 targetPoint, float time)
    {
        yield return new WaitForSeconds(time);

        CharacterManager.Instance.IsControl = false;
        CharacterManager.Instance.ControlMng.MyController.enabled = false;

        gameObject.transform.position = targetPoint;
        Vector3 direction = targetPoint - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        CharacterManager.Instance.ControlMng.MyController.enabled = true;
        CharacterManager.Instance.IsControl = true;
        yield break;
    }

    // 부모 객체 set관련
    public void ParentsSet(Transform setParetns)
    {
        originParents = transform.parent;
        transform.parent = setParetns;
    }
    public void ReturnOriginParents()
    {
        transform.parent = originParents;
    }
    // X축 회전.(역중력 관련)

    IEnumerator RotateX_Dgree(float targetRotation)
    {
        float duration = 2f;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion target = Quaternion.Euler(targetRotation, 0, 0);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, target, elapsed / duration);

            characMng.IsControl = false;
            controller.enabled = false;
            yield return null;
        }
        transform.rotation = target;
        controller.enabled = true;
        isEndReverseAnimation = !isEndReverseAnimation;
        yield break;
    }
    #endregion


    // 씬 전환 간, 컨트롤러 버튼 함수 초기화
    public void CharacterControlMng_ControllerSet()
    {
        var parents = GameObject.FindGameObjectWithTag("Controller").transform;
        if (parents != null)
        {
            ButtonClass JumpBtn = parents.GetChild(3).GetComponent<ButtonClass>();
            JumpBtn.IsSoundPlay = false;
            var jumpBtnObj = JumpBtn.GetButton();
            jumpBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(JumpBtn);
            jumpBtnObj.onClick.AddListener(() => JumpCommand());

            ButtonClass blinkBtn = parents.GetChild(4).GetComponent<ButtonClass>();
            blinkBtn.IsSoundPlay = false;
            var blinkBtnObj = blinkBtn.GetButton();
            blinkBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(blinkBtn);
            blinkBtnObj.onClick.AddListener(() => BlinkClickEvent());
        }


    }


    #region 게터세터
    public CharacterController MyController
    {
        get { return controller; }
    }
    public void SetControllerFloat(float x, float z)
    {
        zPos = z;
        xPos = x;
        runX = x;
        runZ = z;
    }
    public bool IsReverseGround
    {
        get { return isReverseGround; }
        set 
        {
            if (isReverseGround != value)
            {
                isReverseGround = value;
                if (isReverseGround)
                    Physics.gravity = new Vector3(0, -gravity, 0);
                else
                    Physics.gravity = new Vector3(0, gravity, 0);
                StartCoroutine(RotateX_Dgree(value ? 180 : 0));
            }
        }
    }
    public bool IsJump
    {
        get { return isJump; }
        set { isJump = value; }
    }

    #endregion

    #region 옵저버 패턴

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level){}

    public void BlinkValueNotify(e_BlinkPos value){}




    public void GetEnemyFindNotify(List<Transform> findList)
    {
    }

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    public void CheckPoint_PlayerPassNotify(int num){}


    #endregion

}
