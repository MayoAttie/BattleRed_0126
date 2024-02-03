using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Analytics;
using Newtonsoft.Json;
public class FireBaseManager : Singleton<FireBaseManager>
{
    FirebaseAuth auth;
    DatabaseReference dbReference;
    static bool isUserLogin;
    bool isActive;

    private bool isQuitting = false;

    string db_URL;
    private void Awake()
    {
        db_URL = "https://battlered-61ae5-default-rtdb.firebaseio.com/";

        isActive = false;
        DB_Set();
        auth = FirebaseAuth.DefaultInstance;

    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        // OnApplicationQuitting 메서드를 Application.quitting 이벤트에 등록
        Application.quitting += OnApplicationQuitting;
    }

    private void OnDisable()
    {
        // OnApplicationQuitting 메서드를 Application.quitting 이벤트에서 제거
        Application.quitting -= OnApplicationQuitting;
    }

    private void OnApplicationQuit()
    {
        // 현재 로그인한 사용자 가져오기
        FirebaseUser user = auth.CurrentUser;
        if (!isQuitting)
        {

            // 프로그램 종료 중이 아닌 경우에만 데이터 저장
            SaveUserData(GameManager.Instance.GetUserClass(), user.UserId);
        }


        if (user != null)
        {
            // 사용자가 로그인 중인 경우 로그아웃 수행
            auth.SignOut();
            Debug.Log("로그아웃 성공");
        }
        else
        {
            // 사용자가 로그인 중이 아닌 경우
            Debug.Log("로그인되어 있지 않습니다.");
        }
    }

    private void OnApplicationQuitting()
    {
        // 프로그램 종료 중일 때 호출되는 메서드
        isQuitting = true;
    }
    public bool CheckAndCallFunction()
    {
        // 현재 로그인한 사용자 가져오기
        FirebaseUser user = auth.CurrentUser;

        // 사용자가 로그인 중인 경우
        if (user != null)
        {
            return true;
        }
        else
        {
            // 로그인하지 않은 경우
            return false;
        }
    }

    public bool IsUserLogin
    {
        get { return isUserLogin; }
        set { isUserLogin = value; }
    }


    #region 데이터 저장
    // 데이터 저장
    public void SaveUserData(UserClass userData, string userID)
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseUser user = auth.CurrentUser;
        DB_Writer(userID, userData);
    }

    void DB_Writer(string userID, UserClass userDatas)
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        CharacterClass userCharacter = userDatas.GetUserCharacter();
        if (userCharacter != null)
        {
            Dictionary<string, object> characterData = userCharacter.ToDictionary();
            dbReference.Child("userID").Child(userID).Child("UserData").Child("userCharacter").SetValueAsync(characterData)
                .ContinueWithOnMainThread(characterTask =>
                {
                    if (characterTask.IsCompleted)
                    {
                        Debug.Log("유저 캐릭터 데이터 저장 완료");
                    }
                    else
                    {
                        Debug.LogError("유저 캐릭터 데이터 저장 실패: " + characterTask.Exception);
                    }
                });
        }
        else
        {
            Debug.LogWarning("유저 캐릭터 데이터가 없습니다.");
        }

        WeaponAndEquipCls userEquippedWeapon = userDatas.GetUserEquippedWeapon() as WeaponAndEquipCls;
        if (userEquippedWeapon != null)
        {
            Dictionary<string, object> equippedWeaponData = userEquippedWeapon.ToDictionary();
            dbReference.Child("userID").Child(userID).Child("UserData").Child("userEquippedWeapon").SetValueAsync(equippedWeaponData)
                .ContinueWithOnMainThread(weaponTask =>
                {
                    if (weaponTask.IsCompleted)
                    {
                        Debug.Log("유저 장착 무기 데이터 저장 완료");
                    }
                    else
                    {
                        Debug.LogError("유저 장착 무기 데이터 저장 실패: " + weaponTask.Exception);
                    }
                });
        }
        else
        {
            Debug.LogWarning("유저 장착 무기 데이터가 없습니다.");
        }

        ItemClass[] userEquippedEquipment = userDatas.GetUserEquippedEquipment();
        if (userEquippedEquipment != null)
        {
            WeaponAndEquipCls[] weaponAndEquipArray = userEquippedEquipment.Select(item => item as WeaponAndEquipCls).ToArray();
            Dictionary<string, object>[] equippedEquipmentDataArray = weaponAndEquipArray.Select(weapon => weapon?.ToDictionary()).ToArray();
            dbReference.Child("userID").Child(userID).Child("UserData").Child("userEquippedEquipment").SetValueAsync(equippedEquipmentDataArray)
                .ContinueWithOnMainThread(EquippedEquipmentTask =>
                {
                    if (EquippedEquipmentTask.IsCompleted)
                    {
                        Debug.Log("유저 장착 성유물 데이터 저장 완료");
                    }
                    else
                    {
                        Debug.LogError("유저 장착 성유물 데이터 저장 실패: " + EquippedEquipmentTask.Exception);
                    }
                });
        }
        else
        {
            Debug.LogWarning("유저 장착 성유물 데이터가 없습니다.");
        }

        // 리스트 데이터 저장
        SaveItemList(userID, "hadWeaponList", userDatas.GetHadWeaponList(), true);
        SaveItemList(userID, "hadEquipmentList", userDatas.GetHadEquipmentList(), true);
        SaveItemList(userID, "hadGemList", userDatas.GetHadGemList(), false);
        SaveItemList(userID, "hadFoodList", userDatas.GetHadFoodList(), false);
        SaveItemList(userID, "hadGrowMaterialList", userDatas.GetHadGrowMaterialList(), false);
        SaveItemList(userID, "hadEtcItemList", userDatas.GetHadEtcItemList(), false);

        // 시간, 모라 등 데이터 저장
        DateTime? lastConnectTime = userDatas.GetUserLastConnectTime();
        if (lastConnectTime != null)
        {
            string jsonLastConnectTime = JsonUtility.ToJson(lastConnectTime);
            dbReference.Child("userID").Child(userID).Child("UserData").Child("userLastConnectTime").SetValueAsync(jsonLastConnectTime)
                .ContinueWithOnMainThread(connectTimeTask =>
                {
                    if (connectTimeTask.IsCompleted)
                    {
                        Debug.Log("마지막 접속시간 저장 완료");
                    }
                    else
                    {
                        Debug.LogError("마지막 접속시간 데이터 저장 실패: " + connectTimeTask.Exception);
                    }
                });
        }
        else
        {
            Debug.LogWarning("마지막 접속시간이 없습니다.");
        }

        int moraData = userDatas.GetMora();
        string jsonMoraData = JsonUtility.ToJson(moraData);
        dbReference.Child("userID").Child(userID).Child("UserData").Child("userMora").SetValueAsync(moraData.ToString())
            .ContinueWithOnMainThread(moraTask =>
            {
                if (moraTask.IsCompleted)
                {
                    Debug.Log("모라 저장 완료");
                }
                else
                {
                    Debug.LogError("모라 데이터 저장 실패: " + moraTask.Exception);
                }
            });

        int starLight = userDatas.NStarLight;
        string jsonStar = JsonUtility.ToJson(starLight);
        dbReference.Child("userID").Child(userID).Child("UserData").Child("userStar").SetValueAsync(starLight.ToString())
            .ContinueWithOnMainThread(starTask =>
            {
                if (starTask.IsCompleted)
                {
                    Debug.Log("스타라이트 저장 완료");
                }
                else
                {
                    Debug.LogError("스타라이트 데이터 저장 실패: " + starTask.Exception);
                }
            });

        string userMail = userDatas.UserMail;
        string jsonMail = JsonUtility.ToJson(userMail);
        dbReference.Child("userID").Child(userID).Child("UserData").Child("userMail").SetValueAsync(userMail)
            .ContinueWithOnMainThread(mailTask =>
            {
                if (mailTask.IsCompleted)
                {
                    Debug.Log("유저 메일 저장 완료");
                }
                else
                {
                    Debug.LogError("유저 메일 저장 실패: " + mailTask.Exception);
                }
            });

    }


    void SaveItemList(string userID, string nodeName, List<ItemClass> itemList, bool isWeaponEquips)
    {
        if (itemList != null)
        {
            if (isWeaponEquips)
            {
                // List<ItemClass>를 WeaponAndEquipCls[]으로 변환합니다.
                WeaponAndEquipCls[] weaponEquipmentArray = itemList.OfType<WeaponAndEquipCls>().ToArray();

                // 리스트 저장
                Dictionary<string, object>[] equippedEquipmentDataArray = weaponEquipmentArray.Select(weapon => weapon?.ToDictionary()).ToArray();
                dbReference.Child("userID").Child(userID).Child("UserData").Child(nodeName).SetValueAsync(equippedEquipmentDataArray)
                    .ContinueWithOnMainThread(itemListTask =>
                    {
                        if (itemListTask.IsCompleted)
                        {
                            Debug.Log("유저 " + nodeName + " 데이터 저장 완료");
                        }
                        else
                        {
                            Debug.LogError("유저 " + nodeName + " 데이터 저장 실패: " + itemListTask.Exception);
                        }
                    });
            }
            else
            {
                // 리스트 저장
                Dictionary<string, object>[] itemDataArray = itemList.Select(item => item?.ToDictionary()).ToArray();
                dbReference.Child("userID").Child(userID).Child("UserData").Child(nodeName).SetValueAsync(itemDataArray)
                    .ContinueWithOnMainThread(itemListTask =>
                    {
                        if (itemListTask.IsCompleted)
                        {
                            Debug.Log("유저 " + nodeName + " 데이터 저장 완료");
                        }
                        else
                        {
                            Debug.LogError("유저 " + nodeName + " 데이터 저장 실패: " + itemListTask.Exception);
                        }
                    });
            }
        }
        else
        {
            Debug.LogWarning("유저 " + nodeName + " 데이터가 없습니다.");
        }
    }

    #endregion


    #region 데이터 불러오기

    public void LoadUserDataForGameManager()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {

            // Firebase 데이터 로딩이 완료된 후에 다음 코드 실행

            // 현재 로그인한 사용자 가져오기
            FirebaseUser user = auth.CurrentUser;

            if (user != null)
            {
                // 사용자가 로그인 중인 경우, 해당 사용자의 데이터를 가져오기
                Debug.Log("데이터 로드");
                LoadUserData(user.UserId);
            }
            else
            {
                Debug.Log("로그인 아님");
            }
        });
    }


    // 데이터 불러오기
    private void LoadUserData(string userID)
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        dbReference = FirebaseDatabase.DefaultInstance.GetReference("userID").Child(userID).Child("UserData");

        // 해당 사용자의 데이터 가져오기
        dbReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // 데이터가 존재하는 경우
                    LoadUserCharacter(snapshot);
                    LoadUserEquippedWeapon(snapshot);
                    LoadUserEquippedEquipment(snapshot);
                    LoadItemList(snapshot, "hadWeaponList", GameManager.Instance.GetUserClass().GetHadWeaponList());
                    LoadItemList(snapshot, "hadEquipmentList", GameManager.Instance.GetUserClass().GetHadEquipmentList());
                    LoadItemList(snapshot, "hadGemList", GameManager.Instance.GetUserClass().GetHadGemList());
                    LoadItemList(snapshot, "hadFoodList", GameManager.Instance.GetUserClass().GetHadFoodList());
                    LoadItemList(snapshot, "hadGrowMaterialList", GameManager.Instance.GetUserClass().GetHadGrowMaterialList());
                    LoadItemList(snapshot, "hadEtcItemList", GameManager.Instance.GetUserClass().GetHadEtcItemList());

                    LoadUserLastConnectTime(snapshot);
                    LoadUserMora(snapshot);
                    LoadUserStar(snapshot);
                    LoadUserMail(snapshot);

                    Debug.Log("유저 데이터 로드 완료");
                }
                else
                {
                    // 데이터가 없는 경우 또는 해당 경로에 아무 데이터도 없는 경우
                    Debug.Log("데이터가 없습니다.");
                }
            }
        });
    }

    private void LoadUserCharacter(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                // 예시: userEquippedWeapon 데이터를 가져와서 초기화
                if (userDataDict.ContainsKey("userCharacter"))
                {
                    Dictionary<string, object> equippedWeaponData = userDataDict["userCharacter"] as Dictionary<string, object>;

                    if (equippedWeaponData != null)
                    {
                        GameManager.Instance.GetUserClass().GetUserCharacter().SetFromDictionary(equippedWeaponData);
                    }
                }
            }
        }

        //DataSnapshot userCharacterSnapshot = snapshot.Child("userCharacter");
        //if (userCharacterSnapshot.Exists)
        //{
        //    string jsonUserCharacter = userCharacterSnapshot.GetRawJsonValue();
        //    CharacterClass userCharacter = JsonConvert.DeserializeObject<CharacterClass>(jsonUserCharacter);

        //    GameManager.Instance.GetUserClass().SetUserCharacter(userCharacter);
        //}
    }


    private void LoadUserEquippedWeapon(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                // 예시: userEquippedWeapon 데이터를 가져와서 초기화
                if (userDataDict.ContainsKey("userEquippedWeapon"))
                {
                    Dictionary<string, object> equippedWeaponData = userDataDict["userEquippedWeapon"] as Dictionary<string, object>;

                    if (equippedWeaponData != null)
                    {
                        GameManager.Instance.GetUserClass().GetUserEquippedWeapon().SetFromDictionary(equippedWeaponData);
                    }
                }
            }
        }
        //DataSnapshot userEquippedWeaponSnapshot = snapshot.Child("userEquippedWeapon");
        //if (userEquippedWeaponSnapshot.Exists)
        //{
        //    string jsonUserEquippedWeapon = userEquippedWeaponSnapshot.GetRawJsonValue();
        //    WeaponAndEquipCls userEquippedWeapon = JsonConvert.DeserializeObject<WeaponAndEquipCls>(jsonUserEquippedWeapon);

        //    GameManager.Instance.GetUserClass().SetUserEquippedWeapon(userEquippedWeapon);
        //}
    }

    private void LoadUserEquippedEquipment(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                // 예시: userEquippedEquipment 데이터를 가져와서 초기화
                if (userDataDict.ContainsKey("userEquippedEquipment"))
                {
                    List<Dictionary<string, object>> equippedEquipmentDataList = userDataDict["userEquippedEquipment"] as List<Dictionary<string, object>>;

                    if (equippedEquipmentDataList != null)
                    {
                        List<WeaponAndEquipCls> userEquippedEquipmentList = new List<WeaponAndEquipCls>();
                        foreach (var equipData in equippedEquipmentDataList)
                        {
                            WeaponAndEquipCls equip = new WeaponAndEquipCls();
                            equip.SetFromDictionary(equipData);
                            userEquippedEquipmentList.Add(equip);
                        }
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(userEquippedEquipmentList.ToArray());
                    }
                }
            }
        }

        //DataSnapshot userEquippedEquipmentSnapshot = snapshot.Child("userEquippedEquipment");
        //if (userEquippedEquipmentSnapshot.Exists)
        //{
        //    List<WeaponAndEquipCls> userEquippedEquipmentList = new List<WeaponAndEquipCls>();
        //    foreach (DataSnapshot equipSnapshot in userEquippedEquipmentSnapshot.Children)
        //    {
        //        string jsonEquip = equipSnapshot.GetRawJsonValue();
        //        WeaponAndEquipCls equip = JsonConvert.DeserializeObject<WeaponAndEquipCls>(jsonEquip);
        //        userEquippedEquipmentList.Add(equip);
        //    }
        //    GameManager.Instance.GetUserClass().SetUserEquippedEquipment(userEquippedEquipmentList.ToArray());
        //}
    }

    private void LoadItemList(DataSnapshot snapshot, string nodeName, List<ItemClass> itemList)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                // 예시: itemList 데이터를 가져와서 초기화
                if (userDataDict.ContainsKey(nodeName))
                {
                    List<Dictionary<string, object>> itemDataList = userDataDict[nodeName] as List<Dictionary<string, object>>;

                    if (itemDataList != null)
                    {
                        List<ItemClass> loadedItemList = new List<ItemClass>();
                        foreach (var itemData in itemDataList)
                        {
                            ItemClass item = new ItemClass();
                            item.SetFromDictionary(itemData);
                            loadedItemList.Add(item);
                        }
                        itemList.Clear();
                        itemList.AddRange(loadedItemList);
                    }
                }
            }
        }

        //DataSnapshot itemListSnapshot = snapshot.Child(nodeName);
        //if (itemListSnapshot.Exists)
        //{
        //    List<ItemClass> loadedItemList = new List<ItemClass>();
        //    foreach (DataSnapshot itemSnapshot in itemListSnapshot.Children)
        //    {
        //        string jsonItem = itemSnapshot.GetRawJsonValue();
        //        ItemClass item = JsonConvert.DeserializeObject<ItemClass>(jsonItem);
        //        loadedItemList.Add(item);
        //    }
        //    itemList.Clear();
        //    itemList.AddRange(loadedItemList);
        //}
    }

    private void LoadUserLastConnectTime(DataSnapshot snapshot)
    {
        DataSnapshot userLastConnectTimeSnapshot = snapshot.Child("userLastConnectTime");
        if (userLastConnectTimeSnapshot.Exists)
        {
            string jsonLastConnectTime = userLastConnectTimeSnapshot.GetRawJsonValue();
            DateTime lastConnectTime = JsonUtility.FromJson<DateTime>(jsonLastConnectTime);
            GameManager.Instance.GetUserClass().SetUserLastConnectTime(lastConnectTime);
        }
    }

    private void LoadUserMora(DataSnapshot snapshot)
    {
        DataSnapshot userMoraSnapshot = snapshot.Child("userMora");
        if (userMoraSnapshot.Exists)
        {
            string jsonMora = userMoraSnapshot.GetRawJsonValue();
            int mora = int.Parse(jsonMora);
            GameManager.Instance.GetUserClass().SetMora(mora);
        }
    }

    private void LoadUserStar(DataSnapshot snapshot)
    {
        DataSnapshot userStarSnapshot = snapshot.Child("userStar");
        if (userStarSnapshot.Exists)
        {
            string jsonStar = userStarSnapshot.GetRawJsonValue();
            int starLight = int.Parse(jsonStar);
            GameManager.Instance.GetUserClass().NStarLight = starLight;
        }
    }

    private void LoadUserMail(DataSnapshot snapshot)
    {
        DataSnapshot userMailSnapshot = snapshot.Child("userMail");
        if (userMailSnapshot.Exists)
        {
            string jsonMail = userMailSnapshot.GetRawJsonValue();
            string userMail = JsonUtility.FromJson<string>(jsonMail);
            GameManager.Instance.GetUserClass().UserMail = userMail;
        }
    }
    #endregion

    void DB_Set()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(db_URL);

        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    public DatabaseReference DB_Reference
    {
        get 
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            return dbReference; 
        }
        set { dbReference = value; }
    }
    public bool DB_Set_Flag
    {
        get { return isActive; }
        set { isActive = value; }
    }
}
