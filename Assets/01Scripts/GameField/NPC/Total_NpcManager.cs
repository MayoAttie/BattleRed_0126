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

    void Start()
    {
        
    }

    void Update()
    {
        
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
        var tmp = dic_dropUI_NpcObj[npcData];
        GameManager.Instance.InterectionObjUI_Pool.ReturnToPool(tmp);
        dic_dropUI_NpcObj.Remove(npcData);
    }

    #endregion

    #region NPC 대화 및 상호작용 처리

    

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
