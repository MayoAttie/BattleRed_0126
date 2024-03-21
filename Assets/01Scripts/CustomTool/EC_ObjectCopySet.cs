using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EC_ObjectCopySet : EditorWindow    
{
    GameObject originalObject; // 복사할 오브젝트
    GameObject parentObject; // 배치할 부모 오브젝트
    int numberOfCopies = 1; // 생성할 객체의 수
    float spacingX = 1f; // X축 간격
    float spacingZ = 1f; // Z축 간격
    float scaleSize = 1f;
    [MenuItem("Tools/EC_ObjectCopySet")]
    static void Init()
    {
        EC_ObjectCopySet window = (EC_ObjectCopySet)EditorWindow.GetWindow(typeof(EC_ObjectCopySet));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Custom Editor", EditorStyles.boldLabel);
        originalObject = EditorGUILayout.ObjectField("복사할 객체:", originalObject, typeof(GameObject), true) as GameObject;
        parentObject = EditorGUILayout.ObjectField("객체의 부모:", parentObject, typeof(GameObject), true) as GameObject;
        numberOfCopies = EditorGUILayout.IntField("한 라인의 객체 수:", numberOfCopies);
        spacingX = EditorGUILayout.FloatField("X축 간격:", spacingX);
        spacingZ = EditorGUILayout.FloatField("Z축 간격:", spacingZ);
        scaleSize = EditorGUILayout.FloatField("객체 사이즈 : ", scaleSize);
        
        Vector3 originVector3 = default;
        if (originalObject != null)
            originVector3 = originalObject.transform.position;
        if (GUILayout.Button("Copy Objects"))
        {
            if (originalObject != null)
            {
                if (parentObject != null)
                {
                    for (int i = 0; i < numberOfCopies; i++)
                    {
                        for (int j = 0; j < numberOfCopies; j++)
                        {
                            Vector3 newPosition = originVector3 + new Vector3(i * spacingX, 0f, j * spacingZ);
                            GameObject copiedObject = Instantiate(originalObject, newPosition, Quaternion.identity);
                            copiedObject.transform.SetParent(parentObject.transform);
                            copiedObject.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Please select a parent object to copy objects into.");
                }
            }
            else
            {
                Debug.LogError("Please select an original object to copy.");
            }
        }
    }
}
