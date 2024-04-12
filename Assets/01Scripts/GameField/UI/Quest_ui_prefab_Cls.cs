using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Quest_ui_prefab_Cls : MonoBehaviour
{
    Button button;
    TextMeshProUGUI typeText;
    TextMeshProUGUI titleText;
    TextMeshProUGUI contentText;
    float f_FillProgress;

    Image img_fillProgress;
    QuestClass questData;

    Image img_parentObj;
    Image img_typeTextBgr;

    private void Awake()
    {
        img_parentObj = gameObject.GetComponent<Image>();
        img_typeTextBgr = transform.GetChild(0).GetComponent<Image>();
        typeText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        titleText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        contentText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        img_fillProgress = transform.GetChild(4).GetChild(0).GetComponent<Image>();
        button = transform.GetChild(5).GetComponent<Button>();
        f_FillProgress = 0;
    }
    public Image Img_parentObj
    {
        get { return img_parentObj; }
    }
    public Image Img_typeTextBgr
    {
        get { return img_typeTextBgr; }
    }
    public TextMeshProUGUI TypeText
    {
        get { return typeText; }
        set { typeText = value; }
    }
    public TextMeshProUGUI TitleText
    {
        get { return titleText; }
        set { titleText = value; }
    }
    public TextMeshProUGUI ContentText
    {
        get { return contentText; }
        set { contentText = value; }
    }

    public Button ButtonObj
    {
        get { return button; }
    }

    public float FillProgress
    {
        get { return f_FillProgress; }
        set 
        { 
            f_FillProgress = value;
            img_fillProgress.fillAmount = f_FillProgress;
        }
    }

    public QuestClass QuestData
    {
        get { return questData; }
        set { questData = value; }
    }

}
