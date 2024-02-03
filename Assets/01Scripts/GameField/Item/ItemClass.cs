using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemClass : Objects
{
    int nNumber;
    int nCost;
    int nLevel;
    int nId;
    string sContent;
    string sSet;

    public ItemClass()
    { }

    public ItemClass(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel, string sContent, string sSet) : base(sTag, sName, nGrade, isActive)
    {
        this.nNumber = nNumber;
        this.nCost = nCost;
        this.nLevel = nLevel;
        this.sContent = sContent;
        this.sSet = sSet;
        this.nId = -1;
        //this.nId = GameManager.Instance.GetItem_Id_value();
        //GameManager.Instance.Item_Id_value_Upper();
    }

    #region 게터세터
    public void SetNumber(int nNumber) { this.nNumber = nNumber; }
    public void SetCost(int nCost) { this.nCost = nCost; }
    public void SetLevel(int nLevel) { this.nLevel= nLevel; }
    public void SetId(int id) { this.nId = id; }
    public int GetNumber() { return nNumber;}
    public int GetCost() { return nCost;}
    public int GetLevel() { return nLevel; }
    public string GetContent() { return sContent; }
    public string GetSet() { return sSet;}
    public int GetId() { return nId; }
    #endregion

    // 생성자 및 다른 멤버들은 그대로 두고 CopyFrom 메서드만 추가
    public virtual void CopyFrom(ItemClass other)
    {
        SetTag(other.GetTag());
        SetName(other.GetName());
        SetGrade(other.GetGrade());
        SetActive(other.GetIsActive());

        // 얕은 복사
        nNumber = other.nNumber;
        nCost = other.nCost;
        nLevel = other.nLevel;
        nId = other.nId;

        // 깊은 복사: 참조형 멤버들은 각각의 인스턴스를 생성하여 값을 복사
        sContent = string.Copy(other.sContent); //other.sContent; 
        sSet = string.Copy(other.sSet); //other.sSet; 
    }

    #region  파이어 베이스 처리 함수
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = base.ToDictionary();
        dict.Add("nNumber", nNumber);
        dict.Add("nCost", nCost);
        dict.Add("nLevel", nLevel);
        dict.Add("nId", nId);
        dict.Add("sContent", sContent);
        dict.Add("sSet", sSet);

        return dict;
    }
    public void SetFromDictionary(Dictionary<string, object> dict)
    {
        base.SetFromDictionary(dict);

        if (dict.TryGetValue("nNumber", out object numberValue))
        {
            nNumber = Convert.ToInt32(numberValue);
        }

        if (dict.TryGetValue("nCost", out object costValue))
        {
            nCost = Convert.ToInt32(costValue);
        }

        if (dict.TryGetValue("nLevel", out object levelValue))
        {
            nLevel = Convert.ToInt32(levelValue);
        }

        if (dict.TryGetValue("nId", out object idValue))
        {
            nId = Convert.ToInt32(idValue);
        }

        if (dict.TryGetValue("sContent", out object contentValue))
        {
            sContent = contentValue.ToString();
        }

        if (dict.TryGetValue("sSet", out object setValue))
        {
            sSet = setValue.ToString();
        }
    }
    #endregion

}
