using System;

public class QuestClass
{
    public enum e_QuestType
    {
        None = 0,
        DayToDay,   // 일일임무
        WeekToWeek, // 주간임무
        Normal,     // 일반임무
        Max
    }

    private int _nQuestNumber;        // 퀘스트 아이디번호
    private e_QuestType _questType;   // 퀘스트 종류
    private DateTime _time;           // 퀘스트 리셋 시간
    private string _txt_Explain;      // 퀘스트 설명문
    private bool _isClear;            // 퀘스트 클리어 유무
    private int _nTargetNum;          // 퀘스트 목표 달성 수치
    private int _nCurrentNum;         // 퀘스트 현재 달성 수치

    public int QuestNumber
    {
        get { return _nQuestNumber; }
        set { _nQuestNumber = value; }
    }

    public e_QuestType QuestType
    {
        get { return _questType; }
        set { _questType = value; }
    }

    public DateTime Time
    {
        get { return _time; }
        set { _time = value; }
    }

    public string Explanation
    {
        get { return _txt_Explain; }
        set { _txt_Explain = value; }
    }

    public bool IsClear
    {
        get { return _isClear; }
        set { _isClear = value; }
    }

    public int TargetNumber
    {
        get { return _nTargetNum; }
        set { _nTargetNum = value; }
    }

    public int CurrentNumber
    {
        get { return _nCurrentNum; }
        set { _nCurrentNum = value; }
    }

    public QuestClass(int nQuestNumber, e_QuestType questType, DateTime time, string txt_Explain, bool isClear, int nTargetNum, int nCurrentNum)
    {
        QuestNumber = nQuestNumber;
        QuestType = questType;
        Time = time;
        Explanation = txt_Explain;
        IsClear = isClear;
        TargetNumber = nTargetNum;
        CurrentNumber = nCurrentNum;
    }

    public QuestClass()
    {
    }
}