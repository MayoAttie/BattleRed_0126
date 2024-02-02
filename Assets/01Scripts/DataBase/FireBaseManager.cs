using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Analytics;
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

        
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        //{
        //    FirebaseApp app = FirebaseApp.DefaultInstance;
        //    dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        //    // Firebase 데이터 로딩이 완료된 후에 다음 코드 실행
        //    LoadUserDataForGameManager();
        //});

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
        if (!isQuitting)
        {
            // 프로그램 종료 중이 아닌 경우에만 데이터 저장
            SaveUserData(GameManager.Instance.GetUserClass());
        }

        // 현재 로그인한 사용자 가져오기
        FirebaseUser user = auth.CurrentUser;

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

    // 데이터 저장
    public void SaveUserData(UserClass userData)
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        string jsonUserData = JsonUtility.ToJson(userData);
        Debug.Log("jsonUserData : " + jsonUserData);

        // 현재 로그인한 사용자 가져오기
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            string userID = user.UserId;

            // UserID 하위의 "UserData" 노드에 접근
            dbReference.Child("userID").Child(userID).Child("UserData").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        // "UserData" 노드가 이미 존재하면 해당 노드의 데이터를 수정
                        dbReference.Child("userID").Child(userID).Child("UserData").SetValueAsync(jsonUserData)
                            .ContinueWithOnMainThread(updateTask =>
                            {
                                if (updateTask.IsCompleted)
                                {
                                    Debug.Log("유저 데이터 수정");
                                }
                                else
                                {
                                    Debug.LogError("유저 데이터 수정 실패: " + updateTask.Exception);
                                }
                            });
                    }
                    else
                    {
                        // "UserData" 노드가 존재하지 않으면 새로운 노드 생성
                        dbReference.Child("userID").Child(userID).Child("UserData").GetValueAsync().ContinueWithOnMainThread(task2 =>
                        {
                            if (task.IsCompleted)
                            {
                                DataSnapshot snapshot2 = task2.Result;

                                if (!snapshot2.Exists)
                                {
                                    Debug.Log("새로운 유저 데이터 저장1");

                                    dbReference.Child("userID").Child(userID).Child("UserData").SetValueAsync(jsonUserData)
                                        .ContinueWithOnMainThread(newDataSaveTask =>
                                        {
                                            if (newDataSaveTask.IsCompleted)
                                            {
                                                Debug.Log("기본 데이터 저장 완료");
                                            }
                                            else
                                            {
                                                Debug.LogError("기본 데이터 저장 실패: " + newDataSaveTask.Exception);
                                            }
                                        });
                                }
                            }
                        });
                    }
                }
                else
                {
                    // 새로운 유저 데이터를 생성하여 저장하는 예시
                    UserClass newUser = GameManager.Instance.GetUserClass();
                    string jsonNewUserData = JsonUtility.ToJson(newUser);
                    dbReference.Child("userID").Child(userID).Child("UserData").SetValueAsync(jsonNewUserData);
                    Debug.Log("새로운 유저 데이터 저장2");
                }
            });
        }
        else
        {
            Debug.LogError("사용자 로그인 중이 아님");
        }
    }


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
                    //// 데이터가 존재하는 경우
                    //string jsonUserData = snapshot.GetRawJsonValue();
                    //UserClass userData = JsonUtility.FromJson<UserClass>(jsonUserData);

                    //// userData를 활용하여 불러온 데이터를 처리
                    //GameManager.Instance.SetUserClass(userData);
                    //Debug.Log("유저 데이터 로드 완료");


                    // 데이터가 존재하는 경우
                    int nMora = Convert.ToInt32(snapshot.Child("nMora").Value);
                    int nStarLight = Convert.ToInt32(snapshot.Child("nStarLight").Value);
                    string userMail = snapshot.Child("userMail").Value.ToString();

                    // 새로운 UserClass 인스턴스 생성
                    UserClass userData = new UserClass(nMora, nStarLight, userMail);

                    // userData를 활용하여 불러온 데이터를 처리
                    GameManager.Instance.SetUserClass(userData);
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

    void DB_Set()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(db_URL);

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
