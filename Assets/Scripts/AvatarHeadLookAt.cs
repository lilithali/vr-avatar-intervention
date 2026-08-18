using UnityEngine;

public class AvatarHeadLookAt : MonoBehaviour
{
    [Header("自动寻找头显")]
    public bool autoFindHeadset = true;

    [Tooltip("XR Origin 中的 Main Camera 或 CenterEyeAnchor")]
    public Transform headsetTarget;

    [Header("Avatar 骨骼")]
    public Transform headBone;

    [Tooltip("可选。分担一部分旋转，让动作更自然")]
    public Transform neckBone;

    [Header("旋转设置")]
    [Range(0f, 1f)]
    public float headWeight = 0.8f;

    [Range(0f, 1f)]
    public float neckWeight = 0.25f;

    [Tooltip("头部转动速度")]
    public float rotationSpeed = 6f;

    [Tooltip("最多向左或向右转多少度")]
    public float maxHorizontalAngle = 55f;

    [Tooltip("最多向上看多少度")]
    public float maxUpAngle = 30f;

    [Tooltip("最多向下看多少度")]
    public float maxDownAngle = 25f;

    [Header("模型朝向修正")]
    [Tooltip("如果脸朝向错误，在这里调整，例如 Y=180")]
    public Vector3 headRotationOffset;

    private Quaternion headInitialLocalRotation;
    private Quaternion neckInitialLocalRotation;

    private void Start()
    {
        FindHeadset();

        if (headBone != null)
            headInitialLocalRotation = headBone.localRotation;

        if (neckBone != null)
            neckInitialLocalRotation = neckBone.localRotation;
    }

    private void FindHeadset()
    {
        if (!autoFindHeadset || headsetTarget != null)
            return;

        // 优先寻找带有 MainCamera 标签的 VR 摄像机
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            headsetTarget = mainCamera.transform;
            return;
        }

        // 适用于部分 Meta/Oculus 场景
        GameObject centerEye = GameObject.Find("CenterEyeAnchor");

        if (centerEye != null)
        {
            headsetTarget = centerEye.transform;
        }
    }

    private void LateUpdate()
    {
        if (headsetTarget == null || headBone == null)
            return;

        RotateBoneTowardsTarget(
            headBone,
            headInitialLocalRotation,
            headWeight
        );

        if (neckBone != null)
        {
            RotateBoneTowardsTarget(
                neckBone,
                neckInitialLocalRotation,
                neckWeight
            );
        }
    }

    private void RotateBoneTowardsTarget(
        Transform bone,
        Quaternion initialLocalRotation,
        float weight)
    {
        Vector3 targetDirection =
            headsetTarget.position - bone.position;

        if (targetDirection.sqrMagnitude < 0.001f)
            return;

        /*
         * 把目标方向转换到 Avatar 根节点的局部空间。
         * 这样限制角度时不会受到 Avatar 世界旋转的影响。
         */
        Vector3 localDirection =
            transform.InverseTransformDirection(targetDirection.normalized);

        float horizontalAngle =
            Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        float verticalAngle =
            -Mathf.Atan2(
                localDirection.y,
                new Vector2(localDirection.x, localDirection.z).magnitude
            ) * Mathf.Rad2Deg;

        horizontalAngle = Mathf.Clamp(
            horizontalAngle,
            -maxHorizontalAngle,
            maxHorizontalAngle
        );

        verticalAngle = Mathf.Clamp(
            verticalAngle,
            -maxUpAngle,
            maxDownAngle
        );

        Quaternion lookRotation = Quaternion.Euler(
            verticalAngle,
            horizontalAngle,
            0f
        );

        Quaternion weightedRotation = Quaternion.Slerp(
            initialLocalRotation,
            initialLocalRotation * lookRotation,
            weight
        );

        weightedRotation *= Quaternion.Euler(headRotationOffset);

        bone.localRotation = Quaternion.Slerp(
            bone.localRotation,
            weightedRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}