using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static QuestClass;
public class QuestManager : Singleton<QuestManager>
{

    #region 구조체
    struct QuestReward
    {
        public int nId;                 // 퀘스트 아이디
        public string sTag;             // 태그
        public string sValue;           // 보상 객체 이름
        public int nNumber;             // 보상 객체 수량

        public QuestReward(int nId, string sTag, string sValue, int nNumber)
        {
            this.nId = nId;
            this.sTag = sTag;
            this.sValue = sValue;
            this.nNumber = nNumber;
        }
    }
    struct idxSet
    {
        public int questIdx;                // 퀘스트 번호 인덱스
        public int targetNumIdx;            // 퀘스트 클래스 - list_TargetNum/list_CurrentNum 인덱스

        public idxSet(int questIdx, int targetNumIdx)
        {
            this.questIdx = questIdx;
            this.targetNumIdx = targetNumIdx;
        }
    }
    public enum e_ClearedQuest     // 클리어한 퀘스트 enum 
    {
        NONE,
        Dungeon_UndgerLabyrinth,
        Dungeon_DicePuzzle,
        MAX
    }

    #endregion

    Dictionary<string, List<idxSet>> dic_kill_MonsterAnd_questIdx;          // 킬몬스터 _ 퀘스트 인덱스 find 맵
    Dictionary<int, QuestReward> dic_rewardSet;                             // 퀘스트 클리어 후 보상 find 맵
    Dictionary<e_ClearedQuest, List<idxSet>> dic_questCleared_Idx;          // 퀘스트 진행 후, 인덱스 find 맵
    private void Awake()
    {
        InitDataSet();
    }

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

            // 즉시 활성화
            QuestClass data1 = new QuestClass(0, e_QuestType.DayToDay, nowTime, "선인장 5마리 사냥", false, true, new List<float> { 5 }, new List<float> { 0 });
            QuestClass data2 = new QuestClass(1, e_QuestType.DayToDay, nowTime, "버섯 5마리 사냥", false, true, new List<float> { 5 }, new List<float> { 0 });
            QuestClass data3 = new QuestClass(2, e_QuestType.WeekToWeek, nowTime, "선인장 10마리, 버섯 10마리 사냥", false, true, new List<float> { 10, 10 }, new List<float> { 0, 0 });
            QuestClass data4 = new QuestClass(3, e_QuestType.Normal, nowTime, "던전 골렘 보스 1회 사냥", false, true, new List<float> { 1 }, new List<float> { 0 });
            // 이후 활성화
            QuestClass data5 = new QuestClass(4, e_QuestType.Normal, nowTime, "던전의 지하 미궁 탈출", false, false, new List<float> { 1 }, new List<float> { 0 });
            QuestClass data6 = new QuestClass(5, e_QuestType.Normal, nowTime, "던전의 지하 주사위 퍼즐 해결", false, false, new List<float> { 1 }, new List<float> { 0 });

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
    void InitDataSet()
    {
        // idx - 보상 딕셔너리 Set
        dic_rewardSet = new Dictionary<int, QuestReward>();
        QuestReward quest1 = new QuestReward(0, "모라", "모라", 2000);
        QuestReward quest2 = new QuestReward(1, "모라", "모라", 2000);
        QuestReward quest3 = new QuestReward(2, "광물", "철광석", 30);
        QuestReward quest4 = new QuestReward(3, "음식", "달콤달콤 닭고기 스튜", 7);
        QuestReward quest5 = new QuestReward(4, "모라", "모라", 15000);
        QuestReward quest6 = new QuestReward(5, "광물", "백철", 10);

        dic_rewardSet.Add(0, quest1);
        dic_rewardSet.Add(1, quest2);
        dic_rewardSet.Add(2, quest3);
        dic_rewardSet.Add(3, quest4);
        dic_rewardSet.Add(4, quest5);
        dic_rewardSet.Add(5, quest6);

        // idx - 몬스터 딕셔너리 Set
        dic_kill_MonsterAnd_questIdx = new Dictionary<string, List<idxSet>>();
        List<idxSet> cactusList = new List<idxSet> { new idxSet(0, 0), new idxSet(2, 0) };
        List<idxSet> mushroomList = new List<idxSet> { new idxSet(1, 0), new idxSet(2, 1) };
        List<idxSet> golemBossList = new List<idxSet> { new idxSet(3, 0) };

        dic_kill_MonsterAnd_questIdx.Add("Cactus", cactusList);
        dic_kill_MonsterAnd_questIdx.Add("MushroomAngry", mushroomList);
        dic_kill_MonsterAnd_questIdx.Add("Golem_Boss", golemBossList);

        // idx - 퀘스트 진행에 따른 딕셔너리 Set
        dic_questCleared_Idx = new Dictionary<e_ClearedQuest, List<idxSet>>();
        List<idxSet> dungeon_UndgerLabyrinthList = new List<idxSet> { new idxSet(4, 0) };
        List<idxSet> dungeon_DicePuzzleList = new List<idxSet> { new idxSet(5, 0) };

        dic_questCleared_Idx.Add(e_ClearedQuest.Dungeon_UndgerLabyrinth, dungeon_UndgerLabyrinthList);
        dic_questCleared_Idx.Add(e_ClearedQuest.Dungeon_DicePuzzle, dungeon_DicePuzzleList);
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
            if (questDatas[i].QuestType == e_QuestType.DayToDay)
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
                if (questDatas[i].QuestType == e_QuestType.WeekToWeek)
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


    #region 클리어 체크 및 처리

    // 클리어 상태 체크
    public void ClearQuestCheck(Quest_ui_prefab_Cls obj, UI_Manager.Quest_UI master)
    {
        float maxValue = 0;
        foreach (float i in obj.QuestData.List_TargetNum)
        { maxValue += i; }

        float curValue = 0;
        foreach (float i in obj.QuestData.List_CurrentNum)
        { curValue += i; }

        // 퀘스트 클리어 가능일 시,
        if(curValue == maxValue && obj.QuestData.IsClear == false)
        {   // 보상창 출력
            master.RewardObjectOpen(CompensationProcessing(obj));
            obj.QuestData.IsClear = true;

            // 클리어 시, 내용 수정
            master.Quest_ui_prefab_Print(obj, obj.QuestData);
        }
        else
        {
            return;
        }
    }

    string CompensationProcessing(Quest_ui_prefab_Cls obj)
    {
        string textValue = "";
        
        // 데이터 인스턴스화
        QuestClass quest = obj.QuestData;
        int idx = quest.QuestNumber;
        QuestReward rewardData = dic_rewardSet[idx];
        UserClass userData = GameManager.Instance.GetUserClass();

        // 태그에 따른 분류 후, 보상 처리.
        if (rewardData.sTag == "모라")
        {
            int nowMora = userData.GetMora();
            userData.SetMora(nowMora + rewardData.nNumber);
        }
        else if(rewardData.sTag == "음식")
        {
            var havData = userData.GetHadFoodList().Find(tmp => tmp.GetName().Equals(rewardData.sValue));

            if(havData == null)
            {
                ItemClass find_item = GameManager.Instance.GetItemDataList().Find(tmp => tmp.GetName().Equals(rewardData.sValue));
                if (find_item != null)
                {
                    ItemClass item = new ItemClass();
                    item.CopyFrom(find_item);
                    item.SetNumber(rewardData.nNumber);
                    userData.GetHadFoodList().Add(item);
                }
            }
            else
            {
                int havCnt = havData.GetNumber();
                havData.SetNumber(havCnt + rewardData.nNumber);
            }
        }
        else if(rewardData.sTag == "광물")
        {
            var havData = userData.GetHadGemList().Find(tmp => tmp.GetName().Equals(rewardData.sValue));

            if (havData == null)
            {
                ItemClass find_item = GameManager.Instance.GetItemDataList().Find(tmp => tmp.GetName().Equals(rewardData.sValue));
                if (find_item != null)
                {
                    ItemClass item = new ItemClass();
                    item.CopyFrom(find_item);
                    item.SetNumber(rewardData.nNumber);
                    userData.GetHadGemList().Add(item);
                }
            }
            else
            {
                int havCnt = havData.GetNumber();
                havData.SetNumber(havCnt + rewardData.nNumber);
            }
        }
        // 보상 텍스트 작성
        textValue = "보상 : " + rewardData.sValue + ", " + rewardData.nNumber + " 획득!";
        
        return textValue;
    }

    #endregion

    #region 퀘스트 진행 처리

    // 몬스터 사냥 후, 퀘스트 진행도 반영 함수
    public void Kill_andProgressUp(Monster mob)
    {
        string mobName = mob.GetName();
        var questList = GameManager.Instance.GetUserClass().GetQuestList();

        List<idxSet> idxValues = dic_kill_MonsterAnd_questIdx[mobName];

        foreach (idxSet value in idxValues)
        {
            QuestClass quest = questList.Find(tmp => tmp.QuestNumber == value.questIdx);                // idx에 해당하는 퀘스트 찾기.
            float targertNum = quest.List_TargetNum[value.targetNumIdx];
            float curNum = quest.List_CurrentNum[value.targetNumIdx];
            float val = Mathf.Min(curNum + 1, targertNum);
            quest.List_CurrentNum[value.targetNumIdx] = val;
        }
    }

    // 퀘스트 진행 후, 퀘스트 진행도 반영 함수
    public void QuestGetProgressUp(e_ClearedQuest data)
    {
        var questList = GameManager.Instance.GetUserClass().GetQuestList();
        List<idxSet> indexValues = dic_questCleared_Idx[data];

        foreach(idxSet value in indexValues)
        {
            QuestClass quest = questList.Find(tmp => tmp.QuestNumber == value.questIdx);                // idx에 해당하는 퀘스트 찾기.
            float targertNum = quest.List_TargetNum[value.targetNumIdx];
            float curNum = quest.List_CurrentNum[value.targetNumIdx];
            float val = Mathf.Min(curNum + 1, targertNum);
            quest.List_CurrentNum[value.targetNumIdx] = val;
        }
    }

    #endregion

    #region 퀘스트 활성화 처리



    #endregion
}
