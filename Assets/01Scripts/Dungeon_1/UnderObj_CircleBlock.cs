using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderObj_CircleBlock : Subject, IObjectTriggerCheckFunc
{
    float nowRotateY;
    [SerializeField]
    bool isRotatePossible;      // 회전 가능 객체
    [SerializeField]
    bool isTopObject;           // 회전 상태 객체. 천장에 부착.
    bool isHitted;              // 공격 당함(컨트롤 포트 상태 체크)
    bool isMovePossible;        // 이동 가능 위치임.
    ObjectTriggerEnterCheck circle;
    private void Awake()
    {
        circle = null;
        isHitted = false;
        isMovePossible = false;
        if(!isTopObject)
            nowRotateY = transform.rotation.eulerAngles.y;
        else
            nowRotateY = transform.rotation.eulerAngles.y;
    }

    // 써클 진입 시 호출 함수.
    public void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other)
    {
        if (circle == null)
            circle = other;
        CallUndergroundObjectNorify(this);
    }

    public void RotateSelf()
    {   // 회전
        if(isRotatePossible && !isHitted && !isMovePossible)
        {
            isHitted = true;
            if(!isTopObject)
            {
                StartCoroutine(RotateSmoothly(this.gameObject));
            }
            else
            {
                StartCoroutine(RotateSmoothlyReverse(this.gameObject));
            }
        }
    }

    // 회전 함수
    IEnumerator RotateSmoothlyReverse(GameObject obj)
    {
        // 초기 각도 설정
        float currentAngle = nowRotateY % 360;

        float X = transform.rotation.eulerAngles.z;
        float targetY = (currentAngle + 90) % 360;
        float epsilon = 0.01f; // 근사치 보정에 사용할 값

        CharacterManager chMng = CharacterManager.Instance;

        if (targetY != 0)
        {
            while (currentAngle < targetY - epsilon)
            {
                // 부드러운 회전 계산
                Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0, targetY, X), 30f * Time.deltaTime);
                obj.transform.rotation = newRotation;
                // 현재 각도 업데이트
                currentAngle = obj.transform.rotation.eulerAngles.y;


                yield return null;
            }
        }
        else
        {
            while (currentAngle < 359.9 && currentAngle != 0)        // 디버깅으로 확인 후, 회전 예외를 조건문으로 설정.
            {
                Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0, targetY, X), 30f * Time.deltaTime);
                obj.transform.rotation = newRotation;

                currentAngle = obj.transform.rotation.eulerAngles.y;


                yield return null;
            }
        }

        // 정확한 각도로 맞추기
        obj.transform.rotation = Quaternion.Euler(0, targetY, X);
        nowRotateY = targetY;
        // 총 관리 클래스에 옵저버 패턴으로 알림
        CallUndergroundObjectNorify(this);
        isHitted = false;
        yield break;
    }

    IEnumerator RotateSmoothly(GameObject obj)
    {
        CharacterManager.Instance.ControlMng.ParentsSet(this.transform);
        // 초기 각도 설정
        float currentAngle = nowRotateY % 360;

        float X = transform.rotation.eulerAngles.x;
        float targetY = (currentAngle + 90) % 360;
        float epsilon = 0.01f; // 근사치 보정에 사용할 값

        if (targetY != 0)
        {
            while (currentAngle < targetY - epsilon)
            {
                // 부드러운 회전 계산
                Quaternion newRotation;
                if (!isTopObject)
                    newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(X, targetY, 0f), 30f * Time.deltaTime);
                else
                    newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0f, targetY, 180f), 30f * Time.deltaTime);

                // 부드러운 회전 적용
                obj.transform.rotation = newRotation;
                // 현재 각도 업데이트
                currentAngle = obj.transform.rotation.eulerAngles.y;

                yield return null;
            }
        }
        else
        {
            while (currentAngle < 359.9 && currentAngle != 0)        // 디버깅으로 확인 후, 회전 예외를 조건문으로 설정.
            {
                Quaternion newRotation;
                if (!isTopObject)
                    newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(X, targetY, 0f), 30f * Time.deltaTime);
                else
                    newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0f, targetY, 180f), 30f * Time.deltaTime);

                obj.transform.rotation = newRotation;

                currentAngle = obj.transform.rotation.eulerAngles.y;

                yield return null;
            }
        }

        // 정확한 각도로 맞추기
        obj.transform.rotation = Quaternion.Euler(X, targetY, 0f);
        nowRotateY = targetY;
        // 총 관리 클래스에 옵저버 패턴으로 알림
        CallUndergroundObjectNorify(this);
        CharacterManager.Instance.ControlMng.ReturnOriginParents();
        isHitted = false;
        yield break;
    }

    public float NowRotateY
    {
        get { return nowRotateY; }
        set { nowRotateY = value; }
    }
    public bool IsRotatePossible
    {
        get { return isRotatePossible; }
        set { isRotatePossible = value; }
    }
    public bool IsHitted
    {
        get { return isHitted; }
        set { isHitted = value; }
    }
    public bool IsMovePossible
    {
        get { return isMovePossible; }
        set { isMovePossible = value; }
    }
    public ObjectTriggerEnterCheck CircleTrigger
    {
        get { return circle; }
    }
    public bool IsTopOjbect
    {
        get { return isTopObject; }
        set { isTopObject = value; }
    }
}
