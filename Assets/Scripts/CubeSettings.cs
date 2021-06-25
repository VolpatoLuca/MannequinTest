using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "Scriptable/CubeSettings")]
public class CubeSettings : ScriptableObject
{
    public float minDistance = 1.5f;
    public float maxDistance = 3f;
    [Space]
    public float farHeight;
    public float nearHeight;
    public AnimationCurve heightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public float farScale;
    public float nearScale;
    public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Color farColor;
    public Color nearColor;
    public AnimationCurve colorCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [Space]
    public bool isTrembling = false;
    [Range(0,1)]
    public float trembleAmount;
    [Range(0,2)]
    public float trembleFrequency;
}
