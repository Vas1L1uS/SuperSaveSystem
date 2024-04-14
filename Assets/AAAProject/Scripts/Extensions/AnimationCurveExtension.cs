using UnityEngine;

namespace AAAProject.Scripts.Extensions
{
    public static class AnimationCurveExtension
    {
        public static AnimationCurve GetInvert(this AnimationCurve curve)
        {
            Keyframe[] keys = curve.keys;
            Keyframe[] invertedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                invertedKeys[i] = new Keyframe(keys[i].time, 1 - keys[i].value, keys[i].inTangent, keys[i].outTangent);
            }

            AnimationCurve invertedCurve = new AnimationCurve(invertedKeys);
            return invertedCurve;
        }
    }
}