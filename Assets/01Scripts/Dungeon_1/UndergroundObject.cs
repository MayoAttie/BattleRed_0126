using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumetricLines;
public class UndergroundObject : MonoBehaviour, IObjectTriggerCheckFunc, Observer
{
    #region 변수
    UnderObj_CircleBlock[] list_underObj_CircleBlock;                   // UnderObj_CircleBlock 객체 리스트
    VolumetricLineStripBehavior[] list_LightLines;                      // VolumetricLineStripBehavior 객체 리스트(LightLine)
    Dictionary<string, UnderObj_CircleBlock> dic_blocks;                // 보정된 이름값과 객체 매핑
    Dictionary<string, Point> matchPathBlocks;                          // 교류가능한 지역과 Line 간 매핑
    [SerializeField]
    float[] targetRotates;                                              // 목표 각도 배열
    #endregion
    #region 구조체
    struct Point
    {
        public int index;
        public string blockName;
        public string LineName;

        public Point(int index, string blockName, string LineName)
        {
            this.index = index;
            this.blockName = blockName;
            this.LineName = LineName;
        }
    }
    #endregion
    private void Awake()
    {
        list_underObj_CircleBlock = GetComponentsInChildren<UnderObj_CircleBlock>();
        foreach(var i in list_underObj_CircleBlock)         // 옵저버 패턴 연결
        {
            i.Attach(this);
        }
        list_LightLines = GetComponentsInChildren<VolumetricLineStripBehavior>();
        foreach (var i in list_LightLines)
            i.gameObject.SetActive(false);
        InitBlocksMatch();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #region CircleBlock 관련 함수

    void InitBlocksMatch()
    {
        // A-B 이동 가능 객체 및 LightLine 객체 name 세팅
        matchPathBlocks = new Dictionary<string, Point>
        {
            {"start", new Point(0,"bot01", "01") },
            {"top01", new Point(1,"top02","02") },
            {"top02", new Point(2,"top03", "03") }
        };

        // 각 보정 이름에 따른, UnderObj_CircleBlock 오브젝트 매핑
        dic_blocks = new Dictionary<string, UnderObj_CircleBlock>();
        foreach (var i in list_underObj_CircleBlock)
        {
            string i_name = i.name;
            string str = NameCorrector(i_name);
            if (dic_blocks.ContainsKey(str) != true)
            {
                dic_blocks.Add(str, i);
            }
        }

    }

    // 직선 이동 _ 객체 간 각도 체크 후, 이동이 가능한지 판단하여 반환 / 가능하다면, true 반환 및 Line 객체 활성화
    bool CheckPossibleRoot(UnderObj_CircleBlock other)
    {
        // 필요 변수 인스턴스화
        bool result = false;
        string correctedName = NameCorrector(other.name);
        Debug.Log("correctedName : " + correctedName);
        int index = matchPathBlocks[correctedName].index;
        string laser = matchPathBlocks[correctedName].LineName;

        // 각도 가져오기
        float targetDegree = targetRotates[index];
        float nowDegree = other.transform.localRotation.y;

        // 각도 비교
        if (nowDegree == targetDegree)
        {
            foreach(var i in list_LightLines)
            {
                string subName = NameCorrector(i.name);
                if(subName == laser)
                {
                    i.gameObject.SetActive(true);
                    break;
                }
            }
            result = true;
        }
        else
        {
            result = false;
        }
        return result;
    }
    
    // 직선 이동.
    void Move_Between_TwoPoint(UnderObj_CircleBlock other)
    {
        // 필요 변수 인스턴스화
        string correctedName = NameCorrector(other.name);
        string destinationName = matchPathBlocks[correctedName].blockName;
        UnderObj_CircleBlock targetObject = dic_blocks[destinationName];
        
        Transform endPosTransform = targetObject.transform.Find("EndPos");
        Vector3 destinationPos = endPosTransform != null ? endPosTransform.position : Vector3.zero;
        Vector3 nowPos = CharacterManager.Instance.gameObject.transform.position;

        Transform[] targetColliders = targetObject.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "collider").ToArray();
        foreach (var i in targetColliders)
            i.gameObject.SetActive(false);

        Transform[] colliders = other.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "collider").ToArray();
        foreach (var i in colliders)
            i.gameObject.SetActive(false);

        // 이동 함수 호출.
        CharacterManager.Instance.ControlMng.Move_aPoint_to_bPoint(nowPos, destinationPos, 5f);

        StartCoroutine(CallColliderOn(targetColliders, 5.5f));
    }
    IEnumerator CallColliderOn(Transform[] colliders, float time)
    {
        yield return new WaitForSeconds(time);
        foreach (var i in colliders)
            i.gameObject.SetActive(true);
    }

    // 상하 지역 이동(Jump).
    void RotationConvertToDifferntBlock(UnderObj_CircleBlock other)
    {

    }


    string NameCorrector(string name)
    {
        return name.Substring(9);
    }

    #endregion


    #region 옵저버 패턴
    public void CallUndergroundObjectNorify(UnderObj_CircleBlock other)
    {
        if(other.IsHitted == true)      // 회전 후, 정답 체크
        {
            other.IsMovePossible =  CheckPossibleRoot(other);
        }
        else                            // 써클 진입 함수.
        {
            Transform originPos = other.transform.Find("EndPos");
            CharacterManager.Instance.ControlMng.Move_aPoint_to_bPoint(originPos.position);

            if (other.IsRotatePossible == false)        // 회전 불가 객체 (점프)
                RotationConvertToDifferntBlock(other);
            else
                if(other.IsMovePossible == true)        // 회전 가능 객체 (이동)
                    Move_Between_TwoPoint(other);

        }
    }

    public void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other) { }

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level) { }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value) { }

    public void GetEnemyFindNotify(List<Transform> findList) { }

    public void AttackSkillStartNotify() { }

    public void AttackSkillEndNotify() { }

    public void CheckPoint_PlayerPassNotify(int num) { }

    public void WorldMapOpenNotify() { }

    public void WorldMapCloseNotify() { }

    public void ConvertToTargetStateNotify(List<Vector3> listTarget) { }


    #endregion
}
