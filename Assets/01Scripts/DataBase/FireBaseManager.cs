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
    static string sUserEmail;
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
            DateTime currentDateTime = DateTime.Now;

            string jsonLastConnectTime = JsonConvert.SerializeObject(currentDateTime);
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


        // 퀘스트 데이터 저장
        SaveQuestData(userID);


        string mailValue = userDatas.UserMail;
        string jsonMail = JsonUtility.ToJson(mailValue);
        dbReference.Child("userID").Child(userID).Child("UserData").Child("userMail").SetValueAsync(jsonMail)
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

    void SaveQuestData(string userID)
    {
        List<QuestClass> questDatas = GameManager.Instance.GetUserClass().GetQuestList();

        // 리스트 저장
        Dictionary<string, object>[] itemDataArray = questDatas.Select(item => item?.ToDictionary()).ToArray();
        dbReference.Child("userID").Child(userID).Child("UserData").Child("questData").SetValueAsync(itemDataArray)
            .ContinueWithOnMainThread(itemListTask =>
            {
                if (itemListTask.IsCompleted)
                {
                    Debug.Log("유저 " + "퀘스트 데이터" + " 데이터 저장 완료");
                }
                else
                {
                    Debug.LogError("유저 " + "퀘스트 데이터" + " 데이터 저장 실패: " + itemListTask.Exception);
                }
            });
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
                    LoadItemList(snapshot, "hadWeaponList", GameManager.Instance.GetUserClass().GetHadWeaponList(),true);
                    LoadItemList(snapshot, "hadEquipmentList", GameManager.Instance.GetUserClass().GetHadEquipmentList(),true);
                    LoadItemList(snapshot, "hadGemList", GameManager.Instance.GetUserClass().GetHadGemList(),false);
                    LoadItemList(snapshot, "hadFoodList", GameManager.Instance.GetUserClass().GetHadFoodList(),false);
                    LoadItemList(snapshot, "hadGrowMaterialList", GameManager.Instance.GetUserClass().GetHadGrowMaterialList(),false);
                    LoadItemList(snapshot, "hadEtcItemList", GameManager.Instance.GetUserClass().GetHadEtcItemList(),false);

                    LoadUserLastConnectTime(snapshot);
                    LoadUserMora(snapshot);
                    LoadUserStar(snapshot);
                    LoadQuestData(snapshot);
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
    // 캐릭터 정보 불러오기
    private void LoadUserCharacter(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                if (userDataDict.ContainsKey("userCharacter"))
                {
                    Dictionary<string, object> equippedWeaponData = userDataDict["userCharacter"] as Dictionary<string, object>;

                    if (equippedWeaponData != null)
                    {
                        GameManager.Instance.GetUserClass().GetUserCharacter().SetFromDictionary(equippedWeaponData);
                    }
                    Invoke("CallHpModifyFunc", 1f);
                    Debug.Log("userCharacter 데이터 로드 완료");
                }
            }
        }
    }
    void CallHpModifyFunc()
    {   // 체력 수정 함수 호출
        CharacterManager.Instance.Hp_Modify();
    }

    // 착용 무기 정보 불러오기
    private void LoadUserEquippedWeapon(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                if (userDataDict.ContainsKey("userEquippedWeapon"))
                {
                    Dictionary<string, object> equippedWeaponData = userDataDict["userEquippedWeapon"] as Dictionary<string, object>;

                    if (equippedWeaponData != null)
                    {
                        GameManager.Instance.GetUserClass().GetUserEquippedWeapon().SetFromDictionary(equippedWeaponData);
                    }
                    Debug.Log("userEquippedWeapon 데이터 로드 완료");

                }
            }
        }
    }
    // 착용 장비 정보 불러오기
    private void LoadUserEquippedEquipment(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null)
            {
                if (userDataDict.ContainsKey("userEquippedEquipment"))
                {
                    object equippedEquipmentDataObject = userDataDict["userEquippedEquipment"];

                    if(equippedEquipmentDataObject is List<object> itemDataList)
                    {
                        List<WeaponAndEquipCls> fullList = new List<WeaponAndEquipCls>() { null, null, null, null, null };
                        foreach (var data in itemDataList)
                        {
                            if (data == "null" || data == null)
                                continue;
                            if (data is IDictionary<string,object> stringDataList)
                            {

                                Dictionary<string, object> convertDic = new Dictionary<string, object>();

                                foreach (var stringData in stringDataList)
                                {
                                    string key = stringData.Key;
                                    object value = stringData.Value;
                                    convertDic.Add(key, value);
                                }
                                WeaponAndEquipCls newItem = new WeaponAndEquipCls();
                                newItem.SetFromDictionary(convertDic);

                                // 태그에 따라서, 배열 인덱스 설정
                                string tag = newItem.GetTag();
                                int index = 0;
                                if (tag == "꽃") index = 0;
                                else if (tag == "깃털") index = 1;
                                else if (tag == "모래") index = 2;
                                else if (tag == "성배") index = 3;
                                else if (tag == "왕관") index = 4;

                                fullList[index] = newItem;
                            }
                            else
                            {
                                Debug.LogError("데이터 형식이 맞지 않습니다: " + data);

                            }
                            GameManager.Instance.GetUserClass().SetUserEquippedEquipment(fullList.ToArray());
                            Debug.Log("userEquippedEquipment 데이터 로드 완료");

                        }
                    }
                    else if(equippedEquipmentDataObject is IDictionary<string, object> equippedEquipmentDataDict)
                    {
                        List<WeaponAndEquipCls> userEquippedEquipmentList = new List<WeaponAndEquipCls>();

                        foreach (var equipData in equippedEquipmentDataDict)
                        {
                            if (equipData.Value is Dictionary<string, object> equipDict)
                            {
                                WeaponAndEquipCls equip = new WeaponAndEquipCls();
                                equip.SetFromDictionary(equipDict);
                                userEquippedEquipmentList.Add(equip);
                            }
                            else
                            {
                                Debug.LogError("자식 노드의 형식이 예상과 다릅니다.");
                            }
                        }

                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(userEquippedEquipmentList.ToArray());

                        Debug.Log("userEquippedEquipment 데이터 로드 완료");
                    }
                    else
                    {
                        Debug.LogError("userEquippedEquipment 데이터 형식 오류");
                    }
                }
            }
        }
    }
    // 아이템 리스트 정보 불러오기
    private void LoadItemList(DataSnapshot snapshot, string nodeName, List<ItemClass> itemList, bool isWeaponAndEuip)
    {
        if(isWeaponAndEuip)
        {
            if (snapshot.Exists)
            {
                Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;
                if (userDataDict != null)
                {
                    if (userDataDict.ContainsKey(nodeName))
                    {
                        object itemDataObject = userDataDict[nodeName];

                        if (itemDataObject is IList<object> itemDataList)
                        {
                            List<WeaponAndEquipCls> fullList = new List<WeaponAndEquipCls>();

                            foreach (var data in itemDataList)
                            {
                                if (data is IDictionary<string,object> stringDataList)
                                {
                                    Dictionary<string, object> convertDic = new Dictionary<string, object>();

                                    foreach (var stringData in stringDataList)
                                    {
                                        string key = stringData.Key;
                                        object value = stringData.Value;
                                        convertDic.Add(key, value);
                                    }

                                    WeaponAndEquipCls newItem = new WeaponAndEquipCls();
                                    newItem.SetFromDictionary(convertDic);
                                    fullList.Add(newItem);
                                }
                                else
                                {
                                    Debug.LogError("데이터 형식이 맞지 않습니다: " + data);
                                }
                            }

                            itemList.Clear();
                            itemList.AddRange(fullList);
                            Debug.Log(nodeName + " 데이터 로드 완료");
                        }
                        else
                        {
                            Debug.LogError(nodeName + " 데이터 형식 오류");
                        }
                    }
                }
            }
        }
        else
        {
            if (snapshot.Exists)
            {
                Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;
                if (userDataDict != null)
                {
                    if (userDataDict.ContainsKey(nodeName))
                    {
                        object itemDataObject = userDataDict[nodeName];

                        if (itemDataObject is IList<object> itemDataList)
                        {
                            List<ItemClass> fullList = new List<ItemClass>();

                            foreach (var data in itemDataList)
                            {
                                if (data is IDictionary<string, object> stringDataList)
                                {
                                    Dictionary<string, object> convertDic = new Dictionary<string, object>();

                                    foreach (var stringData in stringDataList)
                                    {
                                        string key = stringData.Key;
                                        object value = stringData.Value;
                                        convertDic.Add(key, value);
                                    }

                                    ItemClass newItem = new ItemClass();
                                    newItem.SetFromDictionary(convertDic);
                                    fullList.Add(newItem);
                                }
                                else
                                {
                                    Debug.LogError("데이터 형식이 맞지 않습니다: " + data);
                                }
                            }

                            itemList.Clear();
                            itemList.AddRange(fullList);
                            Debug.Log(nodeName + " 데이터 로드 완료");
                        }
                        else
                        {
                            Debug.LogError(nodeName + " 데이터 형식 오류");
                        }
                    }
                }
            }
        }
        
    }
    private void LoadUserLastConnectTime(DataSnapshot snapshot)
    {
        LoadSimpleData<DateTime>(snapshot, "userLastConnectTime", (lastConnectTime) =>
        {
            GameManager.Instance.GetUserClass().SetUserLastConnectTime(lastConnectTime);
            Debug.Log("userLastConnectTime 데이터 로드 완료");
        });
    }

    private void LoadUserMora(DataSnapshot snapshot)
    {
        LoadSimpleData<int>(snapshot, "userMora", (mora) =>
        {
            GameManager.Instance.GetUserClass().SetMora(mora);
            Debug.Log("userMora 데이터 로드 완료");
        });
    }

    private void LoadUserStar(DataSnapshot snapshot)
    {
        LoadSimpleData<int>(snapshot, "userStar", (starLight) =>
        {
            GameManager.Instance.GetUserClass().NStarLight = starLight;
            Debug.Log("userStar 데이터 로드 완료");
        });
    }

    private void LoadUserMail(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            // 사용자 이메일 키가 존재하면 이메일 값을 가져옵니다.
            if (userDataDict != null && userDataDict.TryGetValue("userMail", out object itemDataObject))
            {
                // 이메일 값을 string으로 변환합니다.
                if (itemDataObject is string itemDataString)
                {
                    string data = JsonConvert.DeserializeObject<string>(itemDataString);
                    GameManager.Instance.GetUserClass().UserMail = data;
                    Debug.Log("유저 이메일 데이터 로드 완료");
                }
            }
            else
            {
                GameManager.Instance.GetUserClass().UserMail = sUserEmail;
                Debug.Log("유저 이메일 데이터 저장 완료");
            }
        }
        else
            Debug.LogError("유저 이메일 데이터 로드 실패");
    }

    private void LoadSimpleData<T>(DataSnapshot snapshot, string nodeName, Action<T> onDataLoaded)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;

            if (userDataDict != null && userDataDict.ContainsKey(nodeName))
            {
                object itemDataObject = userDataDict[nodeName];

                if (itemDataObject is string itemDataString)
                {
                    T data = JsonConvert.DeserializeObject<T>(itemDataString);
                    onDataLoaded?.Invoke(data);
                }
            }
        }
    }

    void LoadQuestData(DataSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            List<QuestClass> questData = GameManager.Instance.GetUserClass().GetQuestList();
            Dictionary<string, object> userDataDict = snapshot.Value as Dictionary<string, object>;
            if (userDataDict != null)
            {
                if (userDataDict.ContainsKey("questData"))
                {
                    object itemDataObject = userDataDict["questData"];

                    if (itemDataObject is IList<object> itemDataList)
                    {
                        List<QuestClass> fullList = new List<QuestClass>();

                        foreach (var data in itemDataList)
                        {
                            if (data is IDictionary<string, object> stringDataList)
                            {
                                Dictionary<string, object> convertDic = new Dictionary<string, object>();

                                foreach (var stringData in stringDataList)
                                {
                                    string key = stringData.Key;
                                    object value = stringData.Value;

                                    if(value is IDictionary<string , object> valueDataDic)
                                    {
                                        List<object> insideList = new List<object>();
                                        foreach (var insideData in valueDataDic)
                                        {
                                            insideList.Add(insideData.Value);
                                        }
                                        convertDic.Add(key, insideList);
                                    }
                                    if(value is IList<object> valueDataList)
                                    {
                                        List<object> insideList = new List<object>();
                                        foreach(var insideData in valueDataList)
                                        {
                                            insideList.Add(insideData);
                                        }
                                        convertDic.Add(key, insideList);
                                    }
                                    else
                                    {
                                        convertDic.Add(key, value);
                                    }
                                }

                                QuestClass newItem = new QuestClass();
                                newItem.SetFromDictionary(convertDic);
                                fullList.Add(newItem);
                            }
                            else
                            {
                                Debug.LogError("데이터 형식이 맞지 않습니다: " + data);
                            }
                        }

                        questData.Clear();
                        questData.AddRange(fullList);

                        // 퀘스트 타임 체크 및 재설정
                        QuestManager questMng =  QuestManager.Instance;
                        if (questMng != null)
                            questMng.QuestD_DayRest();
                        
                        Debug.Log("퀘스트 데이터 로드 완료");
                    }
                    else
                    {
                        Debug.LogError("퀘스트 데이터 형식 오류");
                    }
                }
            }
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
    public string UserEmail
    {
        get { return sUserEmail; }
        set { sUserEmail = value; }
    }
}
