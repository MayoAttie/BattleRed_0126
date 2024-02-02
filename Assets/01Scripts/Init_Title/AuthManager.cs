using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
public class AuthManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField mail_field;
    [SerializeField]
    TMP_InputField pwd_field;
    [SerializeField]
    TextMeshProUGUI loginText;

    FirebaseAuth auth;
    bool clickedStart;
    bool isLogin;
    bool isLogout;

    private void Awake()
    {
        isLogout = false;
        isLogin = false;
        clickedStart = false;
        auth = FirebaseAuth.DefaultInstance;
    }
    private void Start()
    {
        InvokeRepeating("CheckPossible",0.2f,0.2f);
    }

    public void LoginOrLogout()
    {
        if (loginText.text.Equals("로그인"))
        {
            Login();
        }
        else 
        {
            Logout();
        }
    }

    private void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(mail_field.text, pwd_field.text).ContinueWith(task =>
       {
           if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
           {
               isLogin = true;
               isLogout = false;
               Debug.Log(mail_field.text + "의 로그인 성공");
           }
           else
           {
               isLogin = false;
               Debug.LogError(mail_field.text + "의 로그인 실패");
           }
       });
    }

    private void Logout()
    {
        // 현재 로그인한 사용자 가져오기
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            isLogout = true;
            isLogin = false;
            FireBaseManager.Instance.DB_Set_Flag = false;
            // 사용자가 로그인 중인 경우 로그아웃 수행
            auth.SignOut();
            Debug.Log("로그아웃 성공");
        }
        else
        {
            isLogout = false;
            // 사용자가 로그인 중이 아닌 경우
            Debug.Log("로그인되어 있지 않습니다.");
        }
    }


    public void SignIn()
    {
        auth.CreateUserWithEmailAndPasswordAsync(mail_field.text, pwd_field.text).ContinueWith(task =>
        {
            if (!task.IsCanceled && !task.IsFaulted)
            {
                Debug.Log(mail_field.text + "의 회원가입");

                AuthResult authResult = task.Result;
                FirebaseUser newUser = authResult.User;

                string userID = newUser.UserId;

                // UserClass 객체를 JSON 형식의 문자열로 변환
                string jsonUserData = JsonUtility.ToJson(new UserClass());

                // Firebase Realtime Database에 저장
                FireBaseManager.Instance.DB_Reference.Child("userID").Child(userID).Child("UserData").SetValueAsync(jsonUserData)
                    .ContinueWithOnMainThread(databaseTask =>
                    {
                        if (databaseTask.IsCompleted)
                        {
                            Debug.Log(mail_field.text + "의 회원가입 및 데이터베이스 노드 추가 완료");
                        }
                        else
                        {
                            Debug.LogError(mail_field.text + "의 회원가입 성공, 데이터베이스 노드 추가 실패: " + databaseTask.Exception);
                        }
                    });
            }
            else
            {
                Debug.LogError(mail_field.text + "의 회원가입 실패");
            }
        });
    }


    void CheckPossible()
    {
        if (isLogin)
        {
            loginText.text = "로그아웃";
            FireBaseManager.Instance.IsUserLogin = true;
        }
        if (isLogout)
        {
            FireBaseManager.Instance.IsUserLogin = false;
            loginText.text = "로그인";
        }
    }


    public void GameStart()
    {
        FirebaseUser user = auth.CurrentUser;
        if(user != null && !clickedStart)
        {
            clickedStart = true;
            SceneLoadManager.Instance.SceneLoadder("GameField");
        }
        else
        {
            Debug.LogError("로그인 안됨");
        }

    }

}
