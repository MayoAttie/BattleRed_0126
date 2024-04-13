using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UseTool
{
    static float fEpsilon = 0.0001f;
    public static float F_Epsilon
    {
        get { return fEpsilon; }
    }

    // 애니메이션의 길이를 얻는 함수
    public static float GetAnimationLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0f;
    }

    public static void AddItemToList_CopyData(string itemName, List<ItemClass> itemList)
    {
        ItemClass item = GameManager.Instance.GetItemDataList().Find(tmp => tmp.GetName().Equals(itemName));
        if (item != null)
        {
            ItemClass copy = new ItemClass();
            copy.CopyFrom(item);
            itemList.Add(copy);
        }
    }

    public static bool IsCurrentSceneNameCorrect(string name)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Equals(name))
            return true;
        else
            return false;
    }

    public static bool IsTagEquips(ItemClass tmp)
    {
        if (tmp.GetTag() == "꽃" || tmp.GetTag() == "성배" || tmp.GetTag() == "왕관" || tmp.GetTag() == "모래" || tmp.GetTag() == "깃털")
            return true;
        else
            return false;
    }

    // 캐릭터의 제어를 잠금 관리.
    public static void IsControlLock(bool isLock)
    {
        if(isLock)  // 잠금 상태
        {
            CharacterManager.Instance.ControlMng.SetControllerFloat(0, 0);
            CharacterManager.Instance.ControlMng.MyController.enabled = false;
            CharacterManager.Instance.IsControl = false;
        }
        else        // 잠금 상태 해제
        {
            CharacterManager.Instance.IsControl = true;
            CharacterManager.Instance.ControlMng.MyController.enabled = true;
        }
    }

    // 포물선을 계산하는 함수
    public static Vector3 CalculateParabola(Vector3 start, Vector3 middle, Vector3 end, float t)
    {
        float mt = 1f - t;
        return mt * mt * start + 2f * mt * t * middle + t * t * end;
    }


    public static void ObjectValueConverter<T>(ref List<T> targetList, List<object> objList)
    {
        if (objList == null)
        {
            return;
        }

        targetList = new List<T>();
        foreach (object item in objList)
        {
            // 각 항목의 유형을 처리하는 메서드 호출
            ConvertObjectToType(ref targetList, item);
        }
    }
    private static void ConvertObjectToType<T>(ref List<T> targetList, object obj)
    {
        if (obj is double doubleValue)
        {
            targetList.Add((T)(object)((float)doubleValue));
        }
        else if (obj is float floatValue)
        {
            targetList.Add((T)(object)floatValue);
        }
        else if (obj is int intValue)
        {
            targetList.Add((T)(object)intValue);
        }
        else if (obj is long longValue)
        {
            targetList.Add((T)(object)((float)longValue));
        }
        else if (obj is uint uintValue)
        {
            targetList.Add((T)(object)uintValue);
        }
        else if (obj is ulong ulongValue)
        {
            targetList.Add((T)(object)((float)ulongValue));
        }
        else if (obj is short shortValue)
        {
            targetList.Add((T)(object)shortValue);
        }
        else if (obj is ushort ushortValue)
        {
            targetList.Add((T)(object)ushortValue);
        }
        else if (obj is byte byteValue)
        {
            targetList.Add((T)(object)byteValue);
        }
        else if (obj is sbyte sbyteValue)
        {
            targetList.Add((T)(object)sbyteValue);
        }
        else if (obj is string stringValue && float.TryParse(stringValue, out float parsedValue))
        {
            targetList.Add((T)(object)parsedValue);
        }
        else
        {
        }
    }

}
