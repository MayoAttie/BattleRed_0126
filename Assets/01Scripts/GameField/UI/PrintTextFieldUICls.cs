using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;

public class PrintTextFieldUICls : MonoBehaviour
{
    TextMeshProUGUI contentText;                                        // 내용 텍스트 객체
    TextMeshProUGUI titleText;                                          // 제목 텍스트 객체
    GameObject obj_arrowImg;                                            // 화살표 이미지 객체
    Button nextButton;                                                  // 다음 버튼 객체

    bool isSkip;                        // 스킵 여부
    bool isNext;                        // 다음 여부

    int nowCnt = 0;
    int targetCnt = 0;

    private void Start()
    {
        isSkip = false;
        isNext = false;
        InitSetting();
        gameObject.SetActive(false);
    }

    #region 기본함수
    void InitSetting()
    {
        contentText = gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        titleText = gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        obj_arrowImg = gameObject.transform.GetChild(1).gameObject;
        nextButton = gameObject.transform.GetChild(2).GetComponent<Button>();
        nextButton.onClick.AddListener(() => ClickNextButton());
    }
    void OpenPrintUI_Object()
    {
        isSkip = false;
        isNext = false;
        gameObject.SetActive(true);
        obj_arrowImg.SetActive(false);
    }
    void ClosePrintUI_Object()
    {
        gameObject.SetActive(false);
        obj_arrowImg.SetActive(false);
    }
    #endregion

    #region 텍스트 출력

    // 출력
    public void ContentTextPrint(string[] contentTexts, string title, float intervalSec)
    {
        OpenPrintUI_Object();
        nowCnt = 0;
        targetCnt = contentTexts.Length;
        StartCoroutine(RepeatText(contentTexts, title, intervalSec));
    }

    IEnumerator RepeatText(string[] texts, string title, float intervalSec)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            contentText.text = "";
            titleText.text = title;
            yield return StartCoroutine(ShowText(texts[i], intervalSec));
        }
    }

    public void ContentTextPrint(string conText, string title, float intervalSec)
    {
        OpenPrintUI_Object();
        nowCnt = 0;
        targetCnt = 1;
        contentText.text = "";
        titleText.text = title;
        StartCoroutine(ShowText(conText, intervalSec));
    }

    IEnumerator ShowText(string text, float sec)
    {
        string curText = "";
        obj_arrowImg.SetActive(false);                              // 화살표 객체는 비활성화

        isSkip = false;              // 플래그 변수들을 다시 초기화
        isNext = false;

        // 인터벌 간격으로 텍스트 출력
        for (int i = 0; i < text.Length; i++)
        {
            if (isSkip == true)     // 스킵이 true라면 반복문 탈출
                break;

            curText = text.Substring(0, i + 1);
            contentText.text = curText;
            yield return new WaitForSeconds(sec);
        }

        contentText.text = text;        // 전체 텍스트 출력
        isSkip = true;                  // 무조건 넥스트 판정을 위한, 스킵 true화
        while (!isNext)
        {
            obj_arrowImg.SetActive(true);                        // 화살표 객체 활성화
            yield return new WaitForSeconds(1);
        }
        isSkip = false;              // 플래그 변수들을 다시 초기화
        isNext = false;
        nowCnt++;
        yield break;
    }

    // next 버튼
    void ClickNextButton()
    {
        if (nowCnt == targetCnt)
        {
            nowCnt = 0;
            targetCnt = 0;
            ClosePrintUI_Object();
            return;
        }

        if (isSkip)          // 스킵이 활성상태(전체 텍스트 출력 상태)라면, 넥스트 활성화
        {
            isNext = true;
        }
        else                // 스킵이 비활성상태라면, 스킵.
        {
            isSkip = true;
        }
    }

    #endregion

}