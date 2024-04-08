using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserClass
{
    [SerializeField] private CharacterClass userCharacter;               // 유저 캐릭터
    [SerializeField] private ItemClass userEquippedWeapon;               // 장비한 무기
    [SerializeField] private ItemClass[] userEquippedEquipment;          // 장비한 성유물(꽃,깃털,모래,성배,왕관)

    [SerializeField] private List<ItemClass> hadWeaponList;              // 보유한 무기 리스트
    [SerializeField] private List<ItemClass> hadEquipmentList;           // 보유한 성유물 리스트
    [SerializeField] private List<ItemClass> hadGemList;                 // 보유한 광물 리스트
    [SerializeField] private List<ItemClass> hadFoodList;                // 보유한 음식 리스트
    [SerializeField] private List<ItemClass> hadGrowMaterialList;        // 보유한 육성 재화 리스트
    [SerializeField] private List<ItemClass> hadEtcItemList;

    [SerializeField] private DateTime userLastConnectTime;

    [SerializeField] private int nMora;
    [SerializeField] private int nStarLight;

    [SerializeField] private string userMail;

    List<QuestClass> questList;

    public UserClass() 
    {
        userCharacter = new CharacterClass(300, 300, 0, 100, 50, 20, 1, 20, 3.0f, CharacterClass.eCharactgerState.e_NONE, 50, 120, 50, 2.8f, "플레이어", "Knight", 0, true, 100, 20, 0, 0, 0);

        hadWeaponList = new List<ItemClass>();
        hadEquipmentList = new List<ItemClass>();
        hadGemList = new List<ItemClass>();
        hadFoodList = new List<ItemClass>();
        hadEtcItemList = new List<ItemClass>();
        hadGrowMaterialList = new List<ItemClass>();
        questList = new List<QuestClass>();

        userEquippedWeapon = new ItemClass();
        userEquippedEquipment = new ItemClass[5];
        nMora = 0;
        nStarLight = 0;
    }

    public UserClass(CharacterClass userCharacter, ItemClass userEquippedWeapon, ItemClass[] userEquippedEquipment, List<ItemClass> hadWeaponList, List<ItemClass> hadEquipmentList, List<ItemClass> hadGemList, List<ItemClass> hadFoodList, List<ItemClass> hadGrowMaterialList, List<ItemClass> hadEtcItemList, DateTime userLastConnectTime, int nMora, int nStarLight, string userMail)
    {
        this.userCharacter = userCharacter;
        this.userEquippedWeapon = userEquippedWeapon;
        this.userEquippedEquipment = userEquippedEquipment;
        this.hadWeaponList = hadWeaponList;
        this.hadEquipmentList = hadEquipmentList;
        this.hadGemList = hadGemList;
        this.hadFoodList = hadFoodList;
        this.hadGrowMaterialList = hadGrowMaterialList;
        this.hadEtcItemList = hadEtcItemList;
        this.userLastConnectTime = userLastConnectTime;
        this.nMora = nMora;
        this.nStarLight = nStarLight;
        this.userMail = userMail;
    }

    public UserClass(int nMora, int nStarLight, string userMail)
    {
        userCharacter = new CharacterClass(300, 300, 0, 100, 50, 20, 1, 20, 3.0f, CharacterClass.eCharactgerState.e_NONE, 50, 120, 50, 2.8f, "플레이어", "Knight", 0, true, 100, 20, 0, 0, 0);

        hadWeaponList = new List<ItemClass>();
        hadEquipmentList = new List<ItemClass>();
        hadGemList = new List<ItemClass>();
        hadFoodList = new List<ItemClass>();
        hadEtcItemList = new List<ItemClass>();
        hadGrowMaterialList = new List<ItemClass>();
        userEquippedWeapon = new ItemClass();
        userEquippedEquipment = new ItemClass[5];
        questList = new List<QuestClass>();


        this.nMora = nMora;
        this.nStarLight = nStarLight;
        this.userMail = userMail;
    }

    // 게터세터
    public void SetUserCharacter(CharacterClass userCharacter) { this.userCharacter = userCharacter; }
    public void SetUserEquippedWeapon(ItemClass userEquippedWeapon) { this.userEquippedWeapon = userEquippedWeapon; }
    public void SetUserEquippedEquipment(ItemClass[] userEquippedEquipment)
    {
        if (userEquippedEquipment.Length != 5)
        {
            // 입력 배열의 크기가 5가 아닌 경우, 크기가 5인 새 배열을 생성하고 데이터를 복사
            this.userEquippedEquipment = new ItemClass[5];
            foreach(var item in userEquippedEquipment)
            {
                switch(item.GetTag())
                {
                    case "꽃":
                        this.userEquippedEquipment[0] = item;
                        break;
                    case "깃털":
                        this.userEquippedEquipment[1] = item;
                        break;
                    case "모래":
                        this.userEquippedEquipment[2] = item;
                        break;
                    case "성배":
                        this.userEquippedEquipment[3] = item;
                        break;
                    case "왕관":
                        this.userEquippedEquipment[4] = item;
                        break;
                }
            }
        }
        else
        {
            // 입력 배열의 크기가 이미 5인 경우 그대로 할당
            this.userEquippedEquipment = userEquippedEquipment;
        }
    }
    public void SetUserEquippedEquipment(ItemClass userEquippedEquipment, int index) { this.userEquippedEquipment[index] = userEquippedEquipment; }
    public void SetHadWeaponList(List<ItemClass> hadWeaponList) { this.hadWeaponList = hadWeaponList; }
    public void SetHadEquipmentList(List<ItemClass> hadEquipmentList) { this.hadEquipmentList = hadEquipmentList; }
    public void SetEquipedEquipmentList(ItemClass equipEquipment,int index) { this.userEquippedEquipment[index] = equipEquipment; }
    public void SetHadGemList(List<ItemClass> hadGemList) { this.hadGemList = hadGemList; }
    public void SetHadFoodList(List<ItemClass> hadFoodList) { this.hadFoodList= hadFoodList; }
    public void SetHadEtcItemList(List<ItemClass> hadEtcItemList) { this.hadEtcItemList = hadEtcItemList; }
    public void SetHadGrowMaterialList(List<ItemClass> hadGrowMaterialList) { this.hadGrowMaterialList= hadGrowMaterialList; }
    public void SetQuestList(List<QuestClass> questList) { this.questList = questList; }
    public void SetUserLastConnectTime(DateTime userLastConnectTime) { this.userLastConnectTime = userLastConnectTime; }
    public void SetMora(int nMora) { this.nMora = nMora; }

    public CharacterClass GetUserCharacter() { return this.userCharacter; }
    public ItemClass GetUserEquippedWeapon() { return this.userEquippedWeapon;}
    public ItemClass[] GetUserEquippedEquipment() { return this.userEquippedEquipment;}
    public List<ItemClass> GetHadWeaponList() { return this.hadWeaponList; }
    public List<ItemClass> GetHadEquipmentList() { return this.hadEquipmentList; }
    public List<ItemClass> GetHadGemList() { return this.hadGemList; }
    public List<ItemClass> GetHadFoodList() { return this.hadFoodList; }
    public List<ItemClass> GetHadGrowMaterialList() { return this.hadGrowMaterialList; }
    public List<ItemClass> GetHadEtcItemList() { return hadEtcItemList; } 
    public List<QuestClass> GetQuestList() { return questList; }
    public DateTime GetUserLastConnectTime() { return this.userLastConnectTime; }
    public int GetMora() { return this.nMora; }


    // 프로퍼티 추가
    public CharacterClass UserCharacter { get { return userCharacter; } set { userCharacter = value; } }
    public ItemClass UserEquippedWeapon { get { return userEquippedWeapon; } set { userEquippedWeapon = value; } }
    public ItemClass[] UserEquippedEquipment { get { return userEquippedEquipment; } set { userEquippedEquipment = value; } }
    public List<ItemClass> HadWeaponList { get { return hadWeaponList; } set { hadWeaponList = value; } }
    public List<ItemClass> HadEquipmentList { get { return hadEquipmentList; } set { hadEquipmentList = value; } }
    public List<ItemClass> HadGemList { get { return hadGemList; } set { hadGemList = value; } }
    public List<ItemClass> HadFoodList { get { return hadFoodList; } set { hadFoodList = value; } }
    public List<ItemClass> HadGrowMaterialList { get { return hadGrowMaterialList; } set { hadGrowMaterialList = value; } }
    public List<ItemClass> HadEtcItemList { get { return hadEtcItemList; } set { hadEtcItemList = value; } }
    public DateTime UserLastConnectTime { get { return userLastConnectTime; } set { userLastConnectTime = value; } }
    public int NMora { get { return nMora; } set { nMora = value; } }
    public int NStarLight { get { return nStarLight; } set { nStarLight = value; } }
    public string UserMail { get { return userMail; } set { userMail = value; } }
}
