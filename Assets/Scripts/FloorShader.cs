using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShader : MonoBehaviour
{
    public static List<Transform> interactors = new List<Transform>();
    private Material mat;
    private int matPlayerPosHash;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        matPlayerPosHash = Shader.PropertyToID("_PlayerPos");
    }
    void Update()
    {
        float minDistance = float.PositiveInfinity;
        Transform closestObj = null;
        foreach (var i in interactors)
        {
            if (Vector3.Distance(transform.position, i.position) < minDistance)
            {
                closestObj = i;
                minDistance = Vector3.Distance(transform.position, i.position);
            }
        }
        mat.SetVector(matPlayerPosHash, closestObj.position);
    }
}
