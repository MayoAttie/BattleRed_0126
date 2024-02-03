using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;
public class WeaponAndEquipCls : ItemClass
{
    int nLimitLevel;            // 한계레벨
    float nMainStat;            // 메인스텟
    float nSubStat;             // 서브스텟
    int nEffectLevel;           // 재련 돌파 레벨
    string sEffectText;         // 재련 스킬 텍스트
    int nCurrentExp;            // 현재 exp
    int nMaxExp;                // 최대 exp
    List<float> list_ExtraStat; // 엑스트라 스텟

    public WeaponAndEquipCls() { }
    public WeaponAndEquipCls(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel,int nLimitLevel, string sContent, string sSet, string sEffectText, int nEffectLevel, float nMainStat, float nSubStat, int nCurrentExp, int nMaxExp) : base(sTag, sName, nGrade, isActive, nNumber, nCost, nLevel, sContent, sSet)
    {
        this.nLimitLevel = nLimitLevel;
        this.sEffectText = sEffectText;
        this.nEffectLevel = nEffectLevel;
        this.nMainStat = nMainStat;
        this.nSubStat = nSubStat;
        this.nCurrentExp = nCurrentExp;
        this.nMaxExp = nMaxExp;
    }

    public int GetLimitLevel() { return nLimitLevel; }
    public int GetEffectLevel() { return nEffectLevel; }
    public string GetEffectText() { return sEffectText; }
    public int GetCurrentExp() { return nCurrentExp; }
    public int GetMaxExp() { return nMaxExp; }
    public float GetMainStat() { return nMainStat; }
    public float GetSubStat() { return nSubStat; }
    public List<float> GetExtraStat() { return list_ExtraStat; }

    public void SetCurrentExp(int nCurrentExp) { this.nCurrentExp = nCurrentExp; }
    public void SetMaxExp(int nMaxExp) { this.nMaxExp = nMaxExp; }
    public void SetLimitLevel(int nLimitLevel) { this.nLimitLevel = nLimitLevel;}
    public void SetEffectLevel(int nEffectLevel) { this.nEffectLevel = nEffectLevel; }
    public void SetSubStat(float nSubStat) { this.nSubStat = nSubStat;}
    public void SetMainStat(float nMainStat) { this.nMainStat = nMainStat; }
    public void SetEffectText(string sEffectText) { this.sEffectText = sEffectText; }
    public void SetExtraStat(List<float> stats) { this.list_ExtraStat = stats; }


    public override void CopyFrom(ItemClass other)
    {
        // 부모 클래스의 CopyFrom 호출
        base.CopyFrom(other);

        // 자식 클래스의 멤버에 대한 복사
        if (other is WeaponAndEquipCls)
        {
            WeaponAndEquipCls otherWeapon = (WeaponAndEquipCls)other;

            nLimitLevel = otherWeapon.nLimitLevel;
            sEffectText = otherWeapon.sEffectText;
            nEffectLevel = otherWeapon.nEffectLevel;
            nMainStat = otherWeapon.nMainStat;
            nSubStat = otherWeapon.nSubStat;
            nCurrentExp = otherWeapon.nCurrentExp;
            nMaxExp = otherWeapon.nMaxExp;

            // 엑스트라 스텟에 대한 복사
            if (otherWeapon.list_ExtraStat != null)
                list_ExtraStat = new List<float>(otherWeapon.list_ExtraStat);
        }
    }

    #region 파이어베이스 처리함수

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = base.ToDictionary();
        dict.Add("nLimitLevel", nLimitLevel);
        dict.Add("nMainStat", nMainStat);
        dict.Add("nSubStat", nSubStat);
        dict.Add("nEffectLevel", nEffectLevel);
        dict.Add("sEffectText", sEffectText);
        dict.Add("nCurrentExp", nCurrentExp);
        dict.Add("nMaxExp", nMaxExp);
        dict.Add("list_ExtraStat", list_ExtraStat);

        return dict;
    }

    public new void SetFromDictionary(Dictionary<string, object> dict)
    {
        base.SetFromDictionary(dict);

        if (dict.TryGetValue("nLimitLevel", out object limitLevelValue))
        {
            nLimitLevel = Convert.ToInt32(limitLevelValue);
        }

        if (dict.TryGetValue("nMainStat", out object mainStatValue))
        {
            nMainStat = Convert.ToSingle(mainStatValue);
        }

        if (dict.TryGetValue("nSubStat", out object subStatValue))
        {
            nSubStat = Convert.ToSingle(subStatValue);
        }

        if (dict.TryGetValue("nEffectLevel", out object effectLevelValue))
        {
            nEffectLevel = Convert.ToInt32(effectLevelValue);
        }

        if (dict.TryGetValue("sEffectText", out object effectTextValue))
        {
            sEffectText = effectTextValue.ToString();
        }

        if (dict.TryGetValue("nCurrentExp", out object currentExpValue))
        {
            nCurrentExp = Convert.ToInt32(currentExpValue);
        }

        if (dict.TryGetValue("nMaxExp", out object maxExpValue))
        {
            nMaxExp = Convert.ToInt32(maxExpValue);
        }

        if (dict.TryGetValue("list_ExtraStat", out object extraStatValue))
        {
            if (extraStatValue is List<object> extraStatList)
            {
                list_ExtraStat = new List<float>();
                foreach (var item in extraStatList)
                {
                    if (item is float statValue)
                    {
                        list_ExtraStat.Add(statValue);
                    }
                }
            }
        }
    }
    #endregion
}
