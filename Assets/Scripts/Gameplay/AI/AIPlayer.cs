
using System;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{

    [SerializeField]
    CharacterModel characterModel;

    void Start()
    {
        MoveTowardsTheBall();
    }

    private void MoveTowardsTheBall()
    {
        Debug.Log("MoveTowardsTheBall");
        //move
        HitTheBall();
    }

    private void HitTheBall()
    {
        Debug.Log("HitTheBall");
    }
}