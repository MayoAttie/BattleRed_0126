using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Objects
{
    private string sTag;
    private string sName;
    private int nGrade;
    private bool isActive;

    public Objects(string sTag, string sName, int nGrade, bool isActive)
    {
        this.sTag = sTag;
        this.sName = sName;
        this.nGrade = nGrade;
        this.isActive = isActive;
    }
        

    public Objects()
    {
    }


    #region 게터세터
    public void SetTag(string sTag)
    {
        this.sTag = sTag;   
    }

    public void SetName(string sName)
    {
        this.sName = sName;
    }
    public void SetCount(int nGrade)
    {
        this.nGrade = nGrade;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
    public void SetGrade(int grade) { this.nGrade = grade; }

    public string GetTag()
    {
        return sTag;
    }
    public string GetName()
    {
        return sName;
    }
    public int GetGrade()
    {
        return nGrade;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
    #endregion


    #region 파이어베이스 처리함수


    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            { "sTag", sTag },
            { "sName", sName },
            { "nGrade", nGrade },
            { "isActive", isActive }
        };

        return dict;
    }
    public void SetFromDictionary(Dictionary<string, object> dict)
    {
        if (dict.TryGetValue("sTag", out object tagValue))
        {
            sTag = tagValue.ToString();
        }

        if (dict.TryGetValue("sName", out object nameValue))
        {
            sName = nameValue.ToString();
        }

        if (dict.TryGetValue("nGrade", out object gradeValue))
        {
            nGrade = Convert.ToInt32(gradeValue);
        }

        if (dict.TryGetValue("isActive", out object activeValue))
        {
            isActive = Convert.ToBoolean(activeValue);
        }
    }
    #endregion
}
