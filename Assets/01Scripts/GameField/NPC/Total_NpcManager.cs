using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Total_NpcManager : MonoBehaviour
{
    [SerializeField]            // npc 객체 저장 배열
    NonePlayerCharacterManager[] list_npcObjs;

    // npc 객체와 상호작용을 위한 dropUI의 active 여부.
    Dictionary<NonePlayerCharacterManager, bool> dic_isDropUI_openToObj;
    Dictionary<NonePlayerCharacterManager, DropItem_UI> dic_dropUI_NpcObj;

    private void Awake()
    {
        dic_isDropUI_openToObj = new Dictionary<NonePlayerCharacterManager, bool>();

        foreach(var i in list_npcObjs)
        {
            dic_isDropUI_openToObj.Add(i, false);
        }

        dic_dropUI_NpcObj = new Dictionary<NonePlayerCharacterManager, DropItem_UI>();
    }

    #region DropUI_관련
    public DropItem_UI DropUI_ObjectSetInit(Transform scrollViewTransform, NonePlayerCharacterManager npcData)
    {
        var tmp = GameManager.Instance.InterectionObjUI_Pool.GetFromPool(Vector3.zero, Quaternion.identity, scrollViewTransform);
        tmp.ImgSymbol.sprite = ItemSpritesSaver.Instance.SpritesSet[2];
        tmp.Text.text = npcData.NpcCharaceter.GetName();
        tmp.ItemCls = null;
        tmp.Button.onClick.RemoveAllListeners();
        tmp.Button.onClick.AddListener(() => ObjectUICldickEvent(npcData));
        dic_dropUI_NpcObj.Add(npcData, tmp);
        dic_isDropUI_openToObj[npcData] = true;
        return tmp;
    }

    private void ObjectUICldickEvent(NonePlayerCharacterManager npcData)
    {
        //CharacterManager.Instance.Npc_DropUIObjectReturnCall(npcData);
        
        string npcName = npcData.NpcCharaceter.GetName();
        DivideFunctuon_ByNpc(npcName);                                  // 이름에 따른연결 함수 호출
        
        // drop UI 제거
        var tmp = dic_dropUI_NpcObj[npcData];
        GameManager.Instance.InterectionObjUI_Pool.ReturnToPool(tmp);
        dic_dropUI_NpcObj.Remove(npcData);
    }

    #endregion

    #region NPC 대화 및 상호작용 처리

    void DivideFunctuon_ByNpc(string npcName)
    {
        switch(npcName)
        {
            case "카단":
                KadanNpc_Func();
                break;
            case "루더렉":
                LutherreckNPC_Func();
                break;
        }
    }
    
    void KadanNpc_Func()
    {
        string[] scripts =
        {
            "안녕하신가. 내 이름은 카단이라네.",
            "자네 혹시 던전에 대해서 알고 있나?",
            "모른다면 잘 됐군, 이 근처에 던전으로 향하는 입구가 존재하네",
            "만약 던전에 가게 되면, 내 부탁을 하나 들어주겠나?",
            "부탁은 간단하네. 던전에는 지하 미궁이 존재하네.",
            "지하미궁의 마지막에는 보물상자가 있다고 하지.",
            "그것을 확인해주게나",
        };
        UI_Manager.Instance.printTextFieldUI_Cls.ContentTextPrint(scripts,"카단",0.1f);
        GameManager.Instance.GetUserClass().GetQuestList()[4].IsQuestActive = true;
    }
    void LutherreckNPC_Func()
    {
        string[] scripts =
        {
            "안녕하세요. 제 이름은 루더렉입니다.",
            "제 옆에 있는 구조물이, 던전 입구라네요.",
            "하지만, 저는 용기가 없어서 들어갈 수가 없습니다.",
            "친구들과 내기를 했는데 혹시 대신 해결해 주실 수 있나요?",
            "겁쟁이라고 놀림받기 너무 무섭습니다. 부탁드려요.",
            "던전 내부에, 주사위로 이루어진 퍼즐이 있다고 해요.",
            "그 퍼즐을 풀어주셨으면 합니다. 그게 제 친구들과의 내기 내용입니다.",
            "정말 감사합니다. 용사님!!"
        };
        UI_Manager.Instance.printTextFieldUI_Cls.ContentTextPrint(scripts, "루더렉", 0.1f);
        GameManager.Instance.GetUserClass().GetQuestList()[5].IsQuestActive = true;
    }


    #endregion



    #region 게터세터
    public NonePlayerCharacterManager[] List_npcObjs
    {
        get { return list_npcObjs; }
    }
    public Dictionary<NonePlayerCharacterManager, bool> Dic_IsDropUI_openToObj
    {
        get { return dic_isDropUI_openToObj; }
        set { dic_isDropUI_openToObj = value; }
    }
    public Dictionary<NonePlayerCharacterManager, DropItem_UI> Dic_dropUI_NPcObj
    {
        get { return dic_dropUI_NpcObj; }
        set { dic_dropUI_NpcObj = value; }
    }

    #endregion


}
