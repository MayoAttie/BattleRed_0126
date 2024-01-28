using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
public class FireBaseManager : Singleton<FireBaseManager>
{
    FirebaseAuth auth;
    static bool isUserLogin;
    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    private void OnDestroy()
    {
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
}
