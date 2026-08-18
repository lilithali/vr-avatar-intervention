using UnityEngine;

public class SpeakingHeadMotion : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Bones")]
    public Transform neckBone;
    public Transform headBone;

    [Header("Audio Range")]
    public float minVolume = 0.015f;
    public float maxVolume = 0.08f;

    [Header("Neck Motion")]
    public Vector3 neckTalkRotation = new Vector3(0.05f, 0.05f, 0f);

    [Header("Head Motion")]
    public Vector3 headTalkRotation = new Vector3(0.1f, 0.08f, 0.05f);

    [Header("Motion Settings")]
    public float audioSmoothSpeed = 5f;
    public float motionSpeed = 1.2f;
    public float followSpeed = 5f;

    private Quaternion neckRestRotation;
    private Quaternion headRestRotation;

    private float[] samples = new float[256];
    private float talkAmount = 0f;

    void Start()
    {
        if (neckBone != null)
            neckRestRotation = neckBone.localRotation;

        if (headBone != null)
            headRestRotation = headBone.localRotation;
    }

    void Update()
    {
        float targetTalkAmount = GetTalkAmount();

        talkAmount = Mathf.Lerp(
            talkAmount,
            targetTalkAmount,
            Time.deltaTime * audioSmoothSpeed
        );

        ApplyHeadMotion();
    }

    float GetTalkAmount()
    {
        if (audioSource == null || !audioSource.isPlaying)
            return 0f;

        audioSource.GetOutputData(samples, 0);

        float volume = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            volume += Mathf.Abs(samples[i]);
        }

        volume /= samples.Length;

        float amount = Mathf.InverseLerp(
            minVolume,
            maxVolume,
            volume
        );

        return Mathf.Clamp01(amount);
    }

    void ApplyHeadMotion()
    {
        float nodWave = Mathf.Sin(Time.time * motionSpeed);
        float sideWave = Mathf.Sin(Time.time * motionSpeed * 0.7f + 1.3f);
        float tiltWave = Mathf.Sin(Time.time * motionSpeed * 1.4f + 0.5f);

        if (neckBone != null)
        {
            Vector3 neckOffset = new Vector3(
                neckTalkRotation.x * nodWave,
                neckTalkRotation.y * sideWave,
                neckTalkRotation.z * tiltWave
            ) * talkAmount;

            Quaternion neckTarget =
                neckRestRotation * Quaternion.Euler(neckOffset);

            neckBone.localRotation = Quaternion.Slerp(
                neckBone.localRotation,
                neckTarget,
                Time.deltaTime * followSpeed
            );
        }

        if (headBone != null)
        {
            Vector3 headOffset = new Vector3(
                headTalkRotation.x * nodWave,
                headTalkRotation.y * sideWave,
                headTalkRotation.z * tiltWave
            ) * talkAmount;

            Quaternion headTarget =
                headRestRotation * Quaternion.Euler(headOffset);

            headBone.localRotation = Quaternion.Slerp(
                headBone.localRotation,
                headTarget,
                Time.deltaTime * followSpeed
            );
        }
    }
}