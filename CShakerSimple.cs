using UnityEngine;
using URandom = UnityEngine.Random;

public class CShakerSimple: ICamShaker
{
    private float shakeDecay = 0.005f;
    private float shakeIntensity;

    Transform originTransform;

    private Vector3? originPosition;
    private Quaternion? originRotation;

    public CShakerSimple(Transform shakeTransform)
    {
        originTransform = shakeTransform;
    }

    public void SetShake(float shakePower)
    {
        if (originPosition == null) { originPosition = originTransform.position; }
        if (originRotation == null) { originRotation = originTransform.rotation; }
        shakeIntensity = shakePower;
    }

    public void ShakeUpdater()
    {
        if (originPosition == null) return;

        originTransform.position = originPosition.Value + URandom.insideUnitSphere * shakeIntensity;
        originTransform.rotation = new Quaternion(
                        originRotation.Value.x + URandom.Range(-shakeIntensity, shakeIntensity) * .1f,
                        originRotation.Value.y + URandom.Range(-shakeIntensity, shakeIntensity) * .1f,
                        originRotation.Value.z + URandom.Range(-shakeIntensity, shakeIntensity) * .1f,
                        originRotation.Value.w + URandom.Range(-shakeIntensity, shakeIntensity) * .1f);
        shakeIntensity -= shakeDecay;

        if (shakeIntensity <= 0)
        {
            originTransform.position = originPosition.Value;
            originTransform.rotation = originRotation.Value;
            originPosition = null;
            originRotation = null;
        }
    }
}
