using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private void Start()
    {
        QuestDataSetting();
    }

    #region 퀘스트 데이터 세팅
    void QuestDataSetting()
    {
        var questList = GameManager.Instance.GetUserClass().GetQuestList();
        DateTime nowTime = DateTime.Now;
        if(questList.Count <=0)
        {
            List<QuestClass> newData = new List<QuestClass>();

            List<float> zero = new List<float> { 0 };
            List<float> one = new List<float> { 1 };
            List<float> five = new List<float> { 5 };
            List<float> twoZero = new List<float> { 0, 0 };
            List<float> twoFive = new List<float> { 5,5 };

            // 즉시 활성화
            QuestClass data1 = new QuestClass(0, QuestClass.e_QuestType.DayToDay, nowTime, "선인장 5마리 사냥", false, true, five, zero);
            QuestClass data2 = new QuestClass(1, QuestClass.e_QuestType.DayToDay, nowTime, "버섯 5마리 사냥", false, true, five, zero);
            QuestClass data3 = new QuestClass(2, QuestClass.e_QuestType.WeekToWeek, nowTime, "선인장 10마리, 버섯 10마리 사냥", false, true, twoFive, twoZero);
            QuestClass data4 = new QuestClass(3, QuestClass.e_QuestType.Normal, nowTime, "던전 골렘 보스 1회 사냥", false, true, one, zero);
            
            // 이후 활성화
            QuestClass data5 = new QuestClass(4, QuestClass.e_QuestType.Normal, nowTime, "던전의 지하 미궁 탈출", false, false, one, zero);
            QuestClass data6 = new QuestClass(5, QuestClass.e_QuestType.Normal, nowTime, "던전의 지하 주사위 퍼즐 해결", false, false, one, zero);
            
            newData.Add(data1);
            newData.Add(data2);
            newData.Add(data3);
            newData.Add(data4);
            newData.Add(data5);
            newData.Add(data6);

            // 퀘스트 세팅
            GameManager.Instance.GetUserClass().SetQuestList(newData);
        }
    }
    #endregion

    #region 일일 및 주간 퀘스트 초기화

    public void QuestD_DayRest()
    {
        DateTime nextResetTime = DateTime.MinValue;

        // 현재 시간을 가져옴
        DateTime currentTime = DateTime.Now;

        var questDatas = GameManager.Instance.GetUserClass().GetQuestList();

        // 일일 퀘스트의 경우
        if (currentTime.Hour >= 4)
        {
            // 다음 날 새벽 4시로 설정
            nextResetTime = currentTime.Date.AddDays(1).AddHours(4);
        }
        else
        {
            // 당일 새벽 4시로 설정
            nextResetTime = currentTime.Date.AddHours(4);
        }

        // 일일 퀘스트 초기화
        for (int i = 0; i < questDatas.Count; i++)
        {
            if (questDatas[i].QuestType == QuestClass.e_QuestType.DayToDay)
            {
                if (DateTime.Now > questDatas[i].Time)
                {
                    // 값 변경
                    for(int j =0; j<questDatas[i].List_CurrentNum.Count; j++)
                    {
                        questDatas[i].List_CurrentNum[j] = 0;
                    }
                    questDatas[i].IsClear = false;
                    questDatas[i].Time = nextResetTime;
                }
            }
        }

        // 주간 퀘스트 초기화
        if (currentTime.DayOfWeek == DayOfWeek.Monday && currentTime.Hour >= 4)
        {
            // 다음 주 월요일 새벽 4시로 설정
            nextResetTime = currentTime.AddDays(7).Date.AddHours(4);

            for (int i = 0; i < questDatas.Count; i++)
            {
                if (questDatas[i].QuestType == QuestClass.e_QuestType.WeekToWeek)
                {
                    if (DateTime.Now > questDatas[i].Time)
                    {
                        // 값 변경
                        for (int j = 0; j < questDatas[i].List_CurrentNum.Count; j++)
                        {
                            questDatas[i].List_CurrentNum[j] = 0;
                        }
                        questDatas[i].IsClear = false;
                        questDatas[i].Time = nextResetTime;
                    }
                }
            }
        }
    }

    #endregion
}
