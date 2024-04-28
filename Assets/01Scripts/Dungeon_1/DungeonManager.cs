using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    Transform monsters;
    [SerializeField] Transform[] objectTransforms;

    [SerializeField]
    Transform[] bossZones;                        // 보스존 객체
    [SerializeField]
    string[] bossNames_forRewardBos;


    protected void Awake()
    {
        monsters = GameObject.Find("GameObjParents").transform;
    }

    protected void Start()
    {

        ExitButtonTransfromSet();
    }

    // Update is called once per frame
    protected void Update()
    {
    }

    // 던전 나가기 버튼 위치 이동
    void ExitButtonTransfromSet()
    {
        var topCanvas = GameObject.FindGameObjectWithTag("TopCanvas");
        var controlCanvas = GameObject.FindGameObjectWithTag("Controller");

        // Dungeon Exit Button
        var dungeonExitBtnRect = controlCanvas.transform.GetChild(11).GetComponent<RectTransform>();

        // Minimap
        var minimapRect = topCanvas.transform.GetChild(0).GetComponent<RectTransform>();

        // 현재 위치를 복제하여 새로운 위치 계산
        Vector2 newExitBtnPosition = minimapRect.anchoredPosition + new Vector2(250.0f, 60.0f);

        // 새로운 위치로 이동
        dungeonExitBtnRect.anchoredPosition = newExitBtnPosition;
    }


    public virtual void ExitDungeon()
    {
        GameManager.Instance.MonsterHpBarPool.AllReturnToPool();
        SceneLoadManager.Instance.SceneLoadder("GameField");
    }

        


    // 게터 세터
    public Transform GetMonstersTrasform() { return monsters; }

    public Transform[] ObjectTransforms { get { return objectTransforms; } }

    public Transform[] BossZones{ get { return bossZones; } }
    public string[] BossNames { get { return bossNames_forRewardBos; } }

}
