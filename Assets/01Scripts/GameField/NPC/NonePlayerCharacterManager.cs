using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonePlayerCharacterManager : MonoBehaviour
{
    CharacterClass npcCharaceter;

    Animator aniController;
    [SerializeField]
    int nNpcDataIndex;

    void Awake()
    {
        aniController = GetComponent<Animator>();
    }

    void Start()
    {
        npcCharaceter = GameManager.Instance.Getlist_npcDatas()[nNpcDataIndex];
    }

    void Update()
    {
        CharacterStateActor();
    }

    void CharacterStateActor()
    {
        switch (npcCharaceter.GetState())
        {
            case CharacterClass.eCharactgerState.e_Idle:
                aniController.SetInteger("Controller", 0);
                break;
            case CharacterClass.eCharactgerState.e_WALK:
                aniController.SetInteger("Controller", 1);
                break;
            default:
                aniController.SetInteger("Controller", -1);
                break;
        }
    }
}
