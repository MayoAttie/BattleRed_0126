using System.Collections;
using System.Collections.Generic;
using System;
public class Element
{
    #region 변수

    e_Element element;
    bool isActive;
    bool isChild;

    #endregion

    #region 구조체
    public enum e_Element
    {
        None,
        Fire,
        Water,
        Plant,
        Lightning,
        Wind,
        Max
    }
    #endregion

    // 깊은 복사용 데이터
    public Element Clone()
    {
        // 값 타입 필드들은 복사가 자동으로 이루어지므로 별도의 처리가 필요 없음
        return new Element(this.element, this.isActive, this.isChild);
    }


    #region 세터게터

    public void SetElement(e_Element element)
    {
        this.element = element;
    }
    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;
    }
    public void SetIsChild(bool isChild)
    {
        this.isChild = isChild;
    }

    public e_Element GetElement() { return element; }
    public bool GetIsActive() { return isActive; }
    public bool GetIsChild() { return isChild;}

    #endregion

    public Element() { }
    public Element(e_Element element, bool isActive, bool isChild)
    {
        this.element = element;
        this.isActive = isActive;
        this.isChild = isChild;
    }

    #region 파이어 베이스 처리 함수
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            { "element", element.ToString() },
            { "isActive", isActive },
            { "isChild", isChild }
        };
        return dict;
    }

    public void SetFromDictionary(Dictionary<string, object> dict)
    {
        if (dict.TryGetValue("element", out object elementValue) && Enum.TryParse(elementValue.ToString(), out e_Element parsedElement))
        {
            element = parsedElement;
        }

        if (dict.TryGetValue("isActive", out object isActiveValue))
        {
            isActive = Convert.ToBoolean(isActiveValue);
        }

        if (dict.TryGetValue("isChild", out object isChildValue))
        {
            isChild = Convert.ToBoolean(isChildValue);
        }
    }
    #endregion
}
