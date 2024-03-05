using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundObject : MonoBehaviour, IObjectTriggerCheckFunc, Observer
{
    UnderObj_CircleBlock[] list_underObj_CircleBlock;
    private void Awake()
    {
        list_underObj_CircleBlock = GetComponentsInChildren<UnderObj_CircleBlock>();
        foreach(var i in list_underObj_CircleBlock)         // 옵저버 패턴 연결
        {
            i.Attach(this);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #region CircleBlock 관련 함수

    public void CheckPossibleRoot(UnderObj_CircleBlock other)
    {

    }
    
    #endregion


    #region 옵저버 패턴
    public void CallUndergroundObjectNorify(UnderObj_CircleBlock other)
    {
        CheckPossibleRoot(other);
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
