using UnityEngine;
using System.Collections;

public class EyebrowMotionController : MonoBehaviour
{
    [Header("Auto Bone Assignment")]
    public Transform avatarRoot;
    public bool autoAssignOnStart = true;

    [Header("Eyebrow Bones")]
    public Transform leftInnerEyebrow;
    public Transform rightInnerEyebrow;
    public Transform leftOuterEyebrow;
    public Transform rightOuterEyebrow;

    [Header("Position Offsets")]
    public Vector3 leftInnerLiftOffset =
        new Vector3(0f, 0.002f, 0f);

    public Vector3 rightInnerLiftOffset =
        new Vector3(0f, 0.002f, 0f);

    public Vector3 leftOuterLiftOffset =
        new Vector3(0f, 0.001f, 0f);

    public Vector3 rightOuterLiftOffset =
        new Vector3(0f, 0.001f, 0f);

    [Header("Timing")]
    public float minInterval = 2.5f;
    public float maxInterval = 5.0f;
    public float moveInDuration = 0.35f;
    public float holdDuration = 0.6f;
    public float moveOutDuration = 0.5f;

    [Header("Expression Strength")]
    public float minStrength = 0.3f;
    public float maxStrength = 1.0f;

    private Vector3 leftInnerRest;
    private Vector3 rightInnerRest;
    private Vector3 leftOuterRest;
    private Vector3 rightOuterRest;

    private bool isMoving = false;

    // Records which Avatar Root the current bone references belong to.
    private Transform assignedAvatarRoot;

    void Start()
    {
        if (autoAssignOnStart)
        {
            ReassignBonesFromAvatarRoot();
        }
        else
        {
            SaveRestPositions();
            StartCoroutine(ExpressionLoop());
        }
    }

    void Update()
    {
        // If Avatar Root changes during Play Mode,
        // automatically bind the new avatar's eyebrow bones.
        if (autoAssignOnStart &&
            avatarRoot != null &&
            assignedAvatarRoot != avatarRoot)
        {
            ReassignBonesFromAvatarRoot();
        }
    }

    public void SetAvatarRoot(Transform newAvatarRoot)
    {
        if (newAvatarRoot == null)
        {
            Debug.LogWarning(
                "EyebrowMotionController: New Avatar Root is null.",
                this
            );

            return;
        }

        avatarRoot = newAvatarRoot;
        ReassignBonesFromAvatarRoot();
    }

    [ContextMenu("Reassign Eyebrow Bones From Avatar Root")]
    public void ReassignBonesFromAvatarRoot()
    {
        if (avatarRoot == null)
        {
            Debug.LogWarning(
                "Avatar Root is not assigned. Eyebrow bone assignment skipped.",
                this
            );

            return;
        }

        // Stop expressions that may still be controlling the old avatar.
        StopAllCoroutines();
        isMoving = false;

        // Clear references from the previous avatar.
        ClearBoneReferences();

        Transform[] allBones =
            avatarRoot.GetComponentsInChildren<Transform>(true);

        leftInnerEyebrow = FindBone(
            allBones,
            "Bip01 LInnerEyebrow",
            "LInnerEyebrow",
            "LeftInnerEyebrow",
            "InnerEyebrowL",
            "InnerEyebrowLeft"
        );

        rightInnerEyebrow = FindBone(
            allBones,
            "Bip01 RInnerEyebrow",
            "RInnerEyebrow",
            "RightInnerEyebrow",
            "InnerEyebrowR",
            "InnerEyebrowRight"
        );

        leftOuterEyebrow = FindBone(
            allBones,
            "Bip01 LOuterEyebrow",
            "LOuterEyebrow",
            "LeftOuterEyebrow",
            "OuterEyebrowL",
            "OuterEyebrowLeft"
        );

        rightOuterEyebrow = FindBone(
            allBones,
            "Bip01 ROuterEyebrow",
            "ROuterEyebrow",
            "RightOuterEyebrow",
            "OuterEyebrowR",
            "OuterEyebrowRight"
        );

        assignedAvatarRoot = avatarRoot;

        // Save the new avatar's neutral eyebrow positions.
        SaveRestPositions();

        Debug.Log(
            $"Eyebrow bones assigned from Avatar Root: " +
            $"{GetHierarchyPath(avatarRoot)}",
            this
        );

        LogAssignedBone(
            "Left inner eyebrow",
            leftInnerEyebrow
        );

        LogAssignedBone(
            "Right inner eyebrow",
            rightInnerEyebrow
        );

        LogAssignedBone(
            "Left outer eyebrow",
            leftOuterEyebrow
        );

        LogAssignedBone(
            "Right outer eyebrow",
            rightOuterEyebrow
        );

        LogMissingBones();

        // Restart the expression loop for the newly assigned avatar.
        StartCoroutine(ExpressionLoop());
    }

    void ClearBoneReferences()
    {
        leftInnerEyebrow = null;
        rightInnerEyebrow = null;
        leftOuterEyebrow = null;
        rightOuterEyebrow = null;
    }

    Transform FindBone(
        Transform[] bones,
        params string[] possibleNames
    )
    {
        if (avatarRoot == null || bones == null)
        {
            return null;
        }

        // First pass: exact normalized-name matching.
        foreach (string possibleName in possibleNames)
        {
            string targetName = NormalizeName(possibleName);

            foreach (Transform bone in bones)
            {
                if (bone == null ||
                    !bone.IsChildOf(avatarRoot))
                {
                    continue;
                }

                if (NormalizeName(bone.name) == targetName)
                {
                    return bone;
                }
            }
        }

        // Second pass: partial matching for names with prefixes.
        foreach (string possibleName in possibleNames)
        {
            string targetName = NormalizeName(possibleName);

            foreach (Transform bone in bones)
            {
                if (bone == null ||
                    !bone.IsChildOf(avatarRoot))
                {
                    continue;
                }

                if (NormalizeName(bone.name).Contains(targetName))
                {
                    return bone;
                }
            }
        }

        return null;
    }

    string NormalizeName(string boneName)
    {
        return boneName
            .ToLower()
            .Replace(" ", "")
            .Replace("_", "")
            .Replace("-", "");
    }

    void SaveRestPositions()
    {
        if (leftInnerEyebrow != null)
        {
            leftInnerRest =
                leftInnerEyebrow.localPosition;
        }

        if (rightInnerEyebrow != null)
        {
            rightInnerRest =
                rightInnerEyebrow.localPosition;
        }

        if (leftOuterEyebrow != null)
        {
            leftOuterRest =
                leftOuterEyebrow.localPosition;
        }

        if (rightOuterEyebrow != null)
        {
            rightOuterRest =
                rightOuterEyebrow.localPosition;
        }
    }

    IEnumerator ExpressionLoop()
    {
        while (true)
        {
            float waitTime =
                Random.Range(minInterval, maxInterval);

            yield return new WaitForSeconds(waitTime);

            if (!isMoving)
            {
                yield return StartCoroutine(
                    DoEyebrowExpression()
                );
            }
        }
    }

    IEnumerator DoEyebrowExpression()
    {
        isMoving = true;

        float strength =
            Random.Range(minStrength, maxStrength);

        // Small asymmetry makes the movement more natural.
        float leftStrength =
            strength * Random.Range(0.8f, 1.1f);

        float rightStrength =
            strength * Random.Range(0.8f, 1.1f);

        // Move in.
        float t = 0f;

        while (t < moveInDuration)
        {
            t += Time.deltaTime;

            float amount = Mathf.SmoothStep(
                0f,
                1f,
                t / moveInDuration
            );

            ApplyEyebrows(
                amount,
                leftStrength,
                rightStrength
            );

            yield return null;
        }

        ApplyEyebrows(
            1f,
            leftStrength,
            rightStrength
        );

        // Hold.
        yield return new WaitForSeconds(holdDuration);

        // Move out.
        t = 0f;

        while (t < moveOutDuration)
        {
            t += Time.deltaTime;

            float amount =
                1f -
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t / moveOutDuration
                );

            ApplyEyebrows(
                amount,
                leftStrength,
                rightStrength
            );

            yield return null;
        }

        ApplyEyebrows(
            0f,
            leftStrength,
            rightStrength
        );

        isMoving = false;
    }

    void ApplyEyebrows(
        float amount,
        float leftStrength,
        float rightStrength
    )
    {
        if (leftInnerEyebrow != null)
        {
            leftInnerEyebrow.localPosition =
                leftInnerRest +
                leftInnerLiftOffset *
                amount *
                leftStrength;
        }

        if (rightInnerEyebrow != null)
        {
            rightInnerEyebrow.localPosition =
                rightInnerRest +
                rightInnerLiftOffset *
                amount *
                rightStrength;
        }

        if (leftOuterEyebrow != null)
        {
            leftOuterEyebrow.localPosition =
                leftOuterRest +
                leftOuterLiftOffset *
                amount *
                leftStrength;
        }

        if (rightOuterEyebrow != null)
        {
            rightOuterEyebrow.localPosition =
                rightOuterRest +
                rightOuterLiftOffset *
                amount *
                rightStrength;
        }
    }

    void LogAssignedBone(
        string label,
        Transform bone
    )
    {
        if (bone == null)
        {
            return;
        }

        Debug.Log(
            $"{label} assigned to: " +
            $"{GetHierarchyPath(bone)}",
            bone
        );
    }

    void LogMissingBones()
    {
        if (leftInnerEyebrow == null)
        {
            Debug.LogWarning(
                "Left inner eyebrow bone was not found.",
                this
            );
        }

        if (rightInnerEyebrow == null)
        {
            Debug.LogWarning(
                "Right inner eyebrow bone was not found.",
                this
            );
        }

        if (leftOuterEyebrow == null)
        {
            Debug.LogWarning(
                "Left outer eyebrow bone was not found.",
                this
            );
        }

        if (rightOuterEyebrow == null)
        {
            Debug.LogWarning(
                "Right outer eyebrow bone was not found.",
                this
            );
        }
    }

    string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return "(null)";
        }

        string path = target.name;
        Transform current = target.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}