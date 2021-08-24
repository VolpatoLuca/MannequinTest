using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.interactors.Add(transform);
    }
    void OnDisable()
    {
        GameManager.interactors.Remove(transform);
    }
}
