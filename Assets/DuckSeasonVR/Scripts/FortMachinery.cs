using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FortMachinery : MonoBehaviour
{
    public GameObject FortGameObject;
    public GvrAudioSource MachinerySound;
    public Light FortLight;

    float undergroundPosition = -0.82f;
    float lightDefaultIntensity;
    Quaternion burriedRotation;
    Quaternion resurrectedRotation;
    Sequence seq;

    // Use this for initialization
    void Start()
    {
        Vector3 p = FortGameObject.transform.localPosition;
        FortGameObject.transform.localPosition = new Vector3(p.x, undergroundPosition, p.z);
        FortGameObject.transform.localRotation = Quaternion.Euler(180f, 0, 0);
        lightDefaultIntensity = FortLight.intensity;
        FortLight.intensity = 0;
        burriedRotation = Quaternion.Euler(180f, 0, 0);
        resurrectedRotation = Quaternion.Euler(0, 0, 0);
        seq = DOTween.Sequence();
    }


    public void Resurrect()
    {
        // stop previous tween
        DOTween.KillAll();

        Vector3 p = FortGameObject.transform.localPosition;
        MachinerySound.Play();
        DOTween.To(() => FortLight.intensity, x => { FortLight.intensity = x; }, lightDefaultIntensity, 4f);

        FortGameObject.transform.localPosition = new Vector3(0, undergroundPosition, 0);
        FortGameObject.transform.localRotation = burriedRotation;

        seq.Append(FortGameObject.transform.DOLocalMove(new Vector3(p.x, 0, p.z), 0.5f))
            .Append(FortGameObject.transform.DOLocalRotateQuaternion(resurrectedRotation, 2f));
    }

    public void Burry()
    {
        Vector3 p = FortGameObject.transform.localPosition;
        MachinerySound.Play();
        DOTween.To(() => FortLight.intensity, x => { FortLight.intensity = x; }, 0, 2f);
        seq.Append(FortGameObject.transform.DOLocalMove(new Vector3(p.x, undergroundPosition, p.z), 3f))
            .Append(FortGameObject.transform.DOLocalRotateQuaternion(burriedRotation, 0f));
            
    }
}
