using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    void OnEnable()
    {
        FloorCube.interactors.Add(transform);
        FloorShader.interactors.Add(transform);
    }
    void OnDisable()
    {
        FloorCube.interactors.Remove(transform);
        FloorShader.interactors.Remove(transform);
    }
}
