using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAtoB : MonoBehaviour, IObjectTriggerCheckFunc
{
    [SerializeField]
    Transform EndPosition;
    Transform StartPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other)
    {
        CharacterManager.Instance.IsControl = false;
        CharacterManager.Instance.ControlMng.MyController.enabled = false;
        StartPosition = other.transform;

        CharacterManager.Instance.ControlMng.Move_aPoint_to_bPoint(EndPosition.position);

        CharacterManager.Instance.ControlMng.MyController.enabled = true;
        CharacterManager.Instance.IsControl = true;
    }
}
