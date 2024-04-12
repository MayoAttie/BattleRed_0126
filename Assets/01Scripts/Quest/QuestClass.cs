using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class QuestClass
{
    public enum e_QuestType
    {
        None = 0,
        DayToDay,   // 일일임무
        WeekToWeek, // 주간임무
        Normal,     // 일반임무
        All,        // 전체(정렬용)
        Max
    }

    private int nQuestNumber;        // 퀘스트 아이디번호
    private e_QuestType questType;   // 퀘스트 종류
    private DateTime time;           // 퀘스트 리셋 시간
    private string txt_Explain;      // 퀘스트 설명문
    private bool isClear;            // 퀘스트 클리어 유무
    private bool isQuestActive;      // 퀘스트 활성상태 유무
    private List<float> list_TargetNum;          // 퀘스트 목표 달성 수치
    private List<float> list_CurrentNum;         // 퀘스트 현재 달성 수치

    public int QuestNumber
    {
        get { return nQuestNumber; }
        set { nQuestNumber = value; }
    }

    public e_QuestType QuestType
    {
        get { return questType; }
        set { questType = value; }
    }

    public DateTime Time
    {
        get { return time; }
        set { time = value; }
    }

    public string Explanation
    {
        get { return txt_Explain; }
        set { txt_Explain = value; }
    }

    public bool IsClear
    {
        get { return isClear; }
        set { isClear = value; }
    }
    public bool IsQuestActive
    {
        get { return isQuestActive; }
        set { isQuestActive = value; }
    }

    public List<float> List_TargetNum
    {
        get { return list_TargetNum; }
        set { list_TargetNum = value; }
    }

    public List<float> List_CurrentNum
    {
        get { return list_CurrentNum; }
        set { list_CurrentNum = value; }
    }

    public QuestClass(int nQuestNumber, e_QuestType questType, DateTime time, string txt_Explain, bool isClear, bool isQuestActive, List<float> list_TargetNum, List<float> list_CurrentNum)
    {
        this.nQuestNumber = nQuestNumber;
        this.questType = questType;
        this.time = time;
        this.txt_Explain = txt_Explain;
        this.isClear = isClear;
        this.list_TargetNum = list_TargetNum;
        this.list_CurrentNum = list_CurrentNum;
        this.isQuestActive = isQuestActive;
    }

    public QuestClass()
    {
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            {"nQuestNumber",nQuestNumber },
            {"questType",(int)questType },
            {"time", time.ToString("yyyy-MM-dd HH:mm:ss") },
            {"txt_Explain", txt_Explain },
            {"isClear", isClear },
            {"isQuestActive",isQuestActive },
            {"list_TargetNum", list_TargetNum },
            {"list_CurrentNum", list_CurrentNum },
        };
        return dict;
    }

    public void SetFromDictionary(Dictionary<string, object> dict)
    {
        if (dict.TryGetValue("nQuestNumber", out object numberValue))
        {
            nQuestNumber = Convert.ToInt32(numberValue);
        }
        if (dict.TryGetValue("questType", out object typeValue))
        {
            questType = (e_QuestType)(Convert.ToInt32(typeValue));
        }
        if (dict.TryGetValue("time", out object timeValue) && timeValue is string)
        {
            string timeString = (string)timeValue;
            time = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }
        if (dict.TryGetValue("txt_Explain", out object explainValue))
        {
            txt_Explain = (string)explainValue;
        }
        if (dict.TryGetValue("isClear", out object clearValue))
        {
            isClear = Convert.ToBoolean(clearValue);
        }
        if (dict.TryGetValue("isQuestActive", out object activeValue))
        {
            isQuestActive = Convert.ToBoolean(activeValue);
        }
        if (dict.TryGetValue("list_TargetNum", out object targetValue))
        {
            if (targetValue is List<object> targetList)
            {
                list_TargetNum = new List<float>();
                foreach (object target in targetList)
                {
                    if (target is object targetObject)
                    {
                        if (targetObject is double doubleValue)
                        {
                            list_TargetNum.Add((float)doubleValue);
                        }
                        else if (targetObject is float floatValue)
                        {
                            list_TargetNum.Add(floatValue);
                        }
                        else if (targetObject is int intValue)
                        {
                            list_TargetNum.Add(intValue);
                        }
                        else if (targetObject is string stringValue)
                        {
                            if (float.TryParse(stringValue, out float parsedValue))
                            {
                                list_TargetNum.Add(parsedValue);
                            }
                        }
                        else if (targetObject is char charValue)
                        {
                            list_TargetNum.Add((float)charValue);
                        }
                        else
                        {
                            object oTmp = targetObject;
                            float tmp = (float)oTmp;
                            list_TargetNum.Add(tmp);
                        }
                    }
                    else if (target is int intValue)
                    {
                        list_TargetNum.Add((float)intValue); 
                    }
                    else if (target is float floatValue)
                    {
                        list_TargetNum.Add(floatValue); 
                    }
                }
            }
        }
        if (dict.TryGetValue("list_CurrentNum", out object currentValue))
        {
            if (currentValue is List<object> currentList)
            {
                list_CurrentNum = new List<float>();
                foreach (object current in currentList)
                {
                    if (current is object currentObject)
                    {
                        if (currentObject is double doubleValue)
                        {
                            list_CurrentNum.Add((float)doubleValue);
                        }
                        else if (currentObject is float floatValue)
                        {
                            list_CurrentNum.Add(floatValue);
                        }
                        else if (currentObject is int intValue)
                        {
                            list_CurrentNum.Add(intValue);
                        }
                        else if (currentObject is string stringValue)
                        {
                            if (float.TryParse(stringValue, out float parsedValue))
                            {
                                list_CurrentNum.Add(parsedValue);
                            }
                        }
                        else if (currentObject is char charValue)
                        {
                            list_CurrentNum.Add((float)charValue);
                        }
                        else
                        {
                            object oTmp = currentObject;
                            float tmp = (float)oTmp;
                            list_CurrentNum.Add(tmp);
                        }
                    }
                    else if (current is int intValue)
                    {
                        list_CurrentNum.Add((float)intValue); 
                    }
                    else if (current is float floatValue)
                    {
                        list_CurrentNum.Add(floatValue); 
                    }
                }
            }
        }
    }

}