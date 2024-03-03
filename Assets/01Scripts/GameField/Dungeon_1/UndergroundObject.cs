using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundObject : MonoBehaviour, IObjectTriggerCheckFunc
{
    [SerializeField]
    Transform EndPosition;
    Transform StartPosition;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other)
    {
        CharacterManager.Instance.IsControl = false;
        CharacterManager.Instance.ControlMng.MyController.enabled = false;
        StartPosition = other.transform;

        float x = EndPosition.position.x;
        float y = EndPosition.position.y;
        float z = EndPosition.position.z;
        CharacterManager.Instance.ControlMng.MyController.transform.position = new Vector3(x,y,z);
        
        Vector3 direction = EndPosition.position - CharacterManager.Instance.gameObject.transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        CharacterManager.Instance.gameObject.transform.rotation = rotation;

        Debug.Log("CharacterManager.Instance.gameObject.transform.position : " + CharacterManager.Instance.gameObject.transform.position);
        Debug.Log("EndPosition.position : " + EndPosition.position);


        CharacterManager.Instance.ControlMng.MyController.enabled = true;
        CharacterManager.Instance.IsControl = true;
    }

}
