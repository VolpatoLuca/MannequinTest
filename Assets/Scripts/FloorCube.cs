using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCube : MonoBehaviour
{
    public static List<Transform> interactors = new List<Transform>();

    public bool isActive;

    public CubeSettings[] settings;
    private CubeSettings currentSettings;
    public static int settingsIndex = 0;
    private int currentIndex = 0;
    private Vector3 topPosition;
    private float randomFrequence;
    private float pingPongT;
    private float checkTimer;

    private Material mat;
    private int matColorDistanceHash;
    private int matFarColortHash;
    private int matNearColorHash;
    private int matFarHeighttHash;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        randomFrequence = Random.Range(1f, 2f);
        matColorDistanceHash = Shader.PropertyToID("_ColorDistance");
        matFarColortHash = Shader.PropertyToID("_FarColor");
        matNearColorHash = Shader.PropertyToID("_NearColor");
        matFarHeighttHash = Shader.PropertyToID("_FarHeight");
        ChangeSettings();
    }

    public void ChangeSettings()
    {
        currentSettings = settings[settingsIndex];
        ResetToDefault();
        mat.SetColor(matFarColortHash, currentSettings.farColor);
        mat.SetColor(matNearColorHash, currentSettings.nearColor);
        mat.SetFloat(matFarHeighttHash, currentSettings.farHeight);
    }

    private void ResetToDefault()
    {
        Vector3 pos = transform.position;
        pos.y = currentSettings.farHeight;
        transform.position = pos;
        mat.SetFloat(matColorDistanceHash, 0);
        transform.localScale = new Vector3(currentSettings.farScale, currentSettings.farScale, currentSettings.farScale);
    }

    private void Update()
    {
        CheckForSettingsChange();

        checkTimer -= Time.deltaTime;
        if (checkTimer > 0)
            return;

        //calculate Distance based on near height, not current
        topPosition = transform.position;
        topPosition.y = currentSettings.nearHeight;

        Transform closestObj = GetClosestObj();

        if (Vector3.Distance(closestObj.position, topPosition) < currentSettings.maxDistance + 4f)
        {
            UpdatePosColorScale(closestObj);

            Tremble(closestObj);
        }
        else
        {
            checkTimer = 0.4f;
        }
    }

    private void Tremble(Transform closestO)
    {
        if (currentSettings.isTrembling)
        {
            if (GetDistanceT(closestO, topPosition) > 0.9f)
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
        foreach (var i in interactors)
        {
            if (Vector3.Distance(topPosition, i.position) < minDistance)
            {
                obj = i;
                minDistance = Vector3.Distance(topPosition, i.position);
            }
        }
        return obj;
    }

    private void CheckForSettingsChange()
    {
        if (currentIndex != settingsIndex)
        {
            if (settingsIndex >= settings.Length)
                settingsIndex = 0;
            currentIndex = settingsIndex;
            ChangeSettings();
        }
    }

    private void UpdatePosColorScale(Transform closestO)
    {
        float t;
        t = GetDistanceT(closestO, topPosition);

        //Position
        float posT = currentSettings.heightCurve.Evaluate(t);
        float height = Mathf.Lerp(currentSettings.farHeight, currentSettings.nearHeight, posT);
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        //Color
        float colorT = currentSettings.colorCurve.Evaluate(t);
        mat.SetFloat(matColorDistanceHash, colorT);

        //Scale
        float scaleT = currentSettings.scaleCurve.Evaluate(t);
        float scale;
        scale = Mathf.Lerp(currentSettings.farScale, currentSettings.nearScale, scaleT);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private float GetDistanceT(Transform o, Vector3 topPosition)
    {
        float t = Mathf.Clamp(Vector3.Distance(o.position, topPosition), currentSettings.minDistance, currentSettings.maxDistance);
        t -= currentSettings.minDistance;
        t /= currentSettings.maxDistance - currentSettings.minDistance;
        t = 1 - t;
        return t;
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
