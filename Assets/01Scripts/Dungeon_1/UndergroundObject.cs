using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
public class UndergroundObject : MonoBehaviour, IObjectTriggerCheckFunc, Observer
{
    #region 변수
    UnderObj_CircleBlock[] list_underObj_CircleBlock;                   // UnderObj_CircleBlock 객체 리스트
    VolumetricLineStripBehavior[] list_LightLines;                      // VolumetricLineStripBehavior 객체 리스트(LightLine)
    Dictionary<string, UnderObj_CircleBlock> dic_blocks;                // 보정된 이름값과 객체 매핑
    Dictionary<string, Point> matchPathBlocks;                          // 교류가능한 지역과 Line 간 매핑
    #endregion
    #region 구조체
    struct Point
    {
        public string blockName;
        public string LineName;

        public Point(string blockName, string LineName)
        {
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
            {"start", new Point("bot01", "01") },
            {"top01", new Point("top02","02") },
            {"top02", new Point("top03", "03") }
        };

        // 각 이름에 따른, UnderObj_CircleBlock 오브젝트 매핑
        dic_blocks = new Dictionary<string, UnderObj_CircleBlock>();
        foreach (var i in matchPathBlocks)
        {
            string name1 = i.Key;
            string name2 = i.Value.blockName;

            UnderObj_CircleBlock value1;
            UnderObj_CircleBlock value2;

            bool isValue1_Clear = false;
            bool isValue2_Clear = false;

            if (dic_blocks.ContainsKey(name1))
                isValue1_Clear = true;
            if (dic_blocks.ContainsKey(name2))
                isValue2_Clear = true;

            foreach (var j in list_underObj_CircleBlock)
            {
                string j_name = j.name;
                string str = j_name.Substring(10, 3);
                
                if (isValue1_Clear && isValue2_Clear)
                    break;

                if(str == name1)
                {
                    dic_blocks.Add(name1, j);
                    isValue1_Clear = true;
                    continue;
                }
                else if(str == name2)
                {
                    dic_blocks.Add(name2, j);
                    isValue2_Clear = true;
                    continue;
                }
            }
        }

    }

    // 직선 이동 _ 객체 간 각도 체크 후, 이동이 가능한지 판단하여 반환 / 가능하다면, true 반환 및 Line 객체 활성화
    bool CheckPossibleRoot(UnderObj_CircleBlock other)
    {
        bool result = false;
        return result;
    }
    
    // 직선 이동.
    void Move_Between_TwoPoint(UnderObj_CircleBlock other)
    {
    
    }

    // 상하 지역 이동(Jump).
    void RotationConvertToDifferntBlock(UnderObj_CircleBlock other)
    {

    }
    
    #endregion


    #region 옵저버 패턴
    public void CallUndergroundObjectNorify(UnderObj_CircleBlock other)
    {
        if(other.IsHitted == true)
        {
            other.IsMovePossible =  CheckPossibleRoot(other);
        }
        else
        {
            if (other.IsRotatePossible == false)        // 회전 불가 객체 (점프)
                RotationConvertToDifferntBlock(other);
            else
                if(other.IsMovePossible == true)
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
