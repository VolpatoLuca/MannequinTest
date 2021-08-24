using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class FloorCubeOld : MonoBehaviour
{
    private CubeSettings currentSettings;

    private Vector3 nearPosition;
    private float smallestDistance;
    private float distanceT = 0;
    private float checkTimer;

    private float randomFrequence;
    private float pingPongT;

    private Material mat;


    private void Start()
    {
        currentSettings = GameManager.currentSetting;
        CheckForSettingsChange();
        mat = GetComponent<MeshRenderer>().material;
        randomFrequence = Random.Range(1f, 2f);
        pingPongT = 1;
        ResetToDefault();
    }


    private void ResetToDefault()
    {
        Vector3 pos = transform.position;
        pos.y = currentSettings.farHeight;
        transform.position = pos;
        mat.color = GetColor();
        transform.localScale = new Vector3(currentSettings.farScale, currentSettings.farScale, currentSettings.farScale);
    }

    private void Update()
    {
        CheckForSettingsChange();

        checkTimer -= Time.deltaTime;
        if (checkTimer > 0)
            return;

        //calculate Distance based on near height, not current
        nearPosition = transform.position;
        nearPosition.y = currentSettings.nearHeight;

        Transform closestObj = GetClosestObj();

        smallestDistance = Vector3.Distance(closestObj.position, nearPosition);

        bool isClosestObjNear = smallestDistance < currentSettings.maxDistance + 4f;
        if (isClosestObjNear)
        {
            distanceT = GetDistanceT(closestObj.position);

            UpdatePosColorScale();

            Tremble(closestObj);
        }
        else
        {
            checkTimer = Mathf.InverseLerp(0, currentSettings.maxDistance, smallestDistance) / 2;
        }
    }

    private void Tremble(Transform closestO)
    {
        if (currentSettings.isTrembling)
        {
            bool isTooClose = distanceT > 0.9f;
            if (isTooClose)
            {
                return;
            }
            pingPongT += Time.deltaTime * randomFrequence * currentSettings.trembleFrequency;
            float smoothMultiplier = Mathf.InverseLerp(currentSettings.trembleAmount, 0, Mathf.Abs(Mathf.PingPong(pingPongT, currentSettings.trembleAmount * 2) - currentSettings.trembleAmount));
            smoothMultiplier = Mathf.Clamp(smoothMultiplier, 0, 1);
            pingPongT -= Time.deltaTime * randomFrequence * currentSettings.trembleFrequency;
            smoothMultiplier /= 2;
            smoothMultiplier += 0.5f;
            pingPongT += Time.deltaTime * randomFrequence * currentSettings.trembleFrequency * smoothMultiplier;

            //Position
            float heightOffset = Mathf.PingPong(pingPongT, currentSettings.trembleAmount * 2) - currentSettings.trembleAmount;
            Vector3 pos = transform.position;
            pos.y += heightOffset;
            transform.position = pos;

            //Scale
            float scale = transform.localScale.x;
            float scaleOffset = Mathf.PingPong(pingPongT, 1);
            scaleOffset += 0.5f;
            scale *= scaleOffset;
            Vector3 newScale = new Vector3(scale, scale, scale);
            transform.localScale = newScale;
        }
    }

    private Transform GetClosestObj()
    {
        float minDistance = float.PositiveInfinity;
        Transform obj = null;
        foreach (var i in GameManager.interactors)
        {
            float interactorDistance = Vector3.Distance(nearPosition, i.position);
            if (interactorDistance < minDistance)
            {
                obj = i;
                minDistance = interactorDistance;
            }
        }
        return obj;
    }

    private void CheckForSettingsChange()
    {
        if (currentSettings != GameManager.currentSetting)
        {
            currentSettings = GameManager.currentSetting;
            ResetToDefault();
        }
    }

    private void UpdatePosColorScale()
    {
        //Position
        float posT = currentSettings.heightCurve.Evaluate(distanceT);
        float height = Mathf.Lerp(currentSettings.farHeight, currentSettings.nearHeight, posT);
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        //Color

        mat.color = GetColor();

        //Scale
        float scaleT = currentSettings.scaleCurve.Evaluate(distanceT);
        float scale;
        scale = Mathf.Lerp(currentSettings.farScale, currentSettings.nearScale, scaleT);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private float GetDistanceT(Vector3 closestObj)
    {
        float t = Mathf.Clamp(Vector3.Distance(closestObj, nearPosition), currentSettings.minDistance, currentSettings.maxDistance);
        t -= currentSettings.minDistance;
        t /= currentSettings.maxDistance - currentSettings.minDistance;
        t = 1 - t;
        return t;
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private Color GetColor()
    {
        return Color.Lerp(currentSettings.farColor, currentSettings.nearColor, currentSettings.colorCurve.Evaluate(distanceT));
    }
}
