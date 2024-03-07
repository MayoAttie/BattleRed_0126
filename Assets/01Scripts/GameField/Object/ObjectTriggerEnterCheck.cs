using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerEnterCheck : MonoBehaviour
{
    Dictionary<IObjectTriggerCheckFunc, Transform> dicComponent;
    IObjectTriggerCheckFunc[] parentComponents;
    bool isActive;
    private void Awake()
    {
        isActive = false;
        dicComponent = new Dictionary<IObjectTriggerCheckFunc, Transform>();
        parentComponents = transform.GetComponentsInParent<IObjectTriggerCheckFunc>();
        Transform[] componets = transform.GetComponentsInParent<Transform>();
        foreach (var obj in componets)
        {
            if (obj.GetComponent<IObjectTriggerCheckFunc>() != null)
            {
                IObjectTriggerCheckFunc component = obj.GetComponent<IObjectTriggerCheckFunc>();
                dicComponent.Add(component, obj);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            if (!isActive)
            {
                isActive = true;
                Transform parents = gameObject.transform.parent;
                // 부모 객체가 가진 인터페이스의 기능 함수를 호출.
                foreach (IObjectTriggerCheckFunc triggerCheckFunc in parentComponents)
                {
                    if (dicComponent.ContainsKey(triggerCheckFunc))
                    {
                        if (dicComponent[triggerCheckFunc] != parents)
                            continue;
                        else
                        {
                            triggerCheckFunc.EnterTriggerFunctionInit(this);
                            break;
                        }
                    }
                    else { }
                }
            }

        }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

}
