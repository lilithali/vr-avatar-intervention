using System.Collections;
using UnityEngine;

public class FaceBlinkController : MonoBehaviour
{
    [Header("Auto Bone Assignment")]
    public Transform avatarRoot;
    public bool autoAssignOnStart = true;

    [Header("Eyelid Bones")]
    public Transform leftBlinkTop;
    public Transform rightBlinkTop;
    public Transform leftBlinkBottom;
    public Transform rightBlinkBottom;

    [Header("Blink Position Offsets")]
    public Vector3 leftTopBlinkOffset =
        new Vector3(0f, -0.005f, 0f);

    public Vector3 rightTopBlinkOffset =
        new Vector3(0f, -0.005f, 0f);

    public Vector3 leftBottomBlinkOffset =
        new Vector3(0f, 0.002f, 0f);

    public Vector3 rightBottomBlinkOffset =
        new Vector3(0f, 0.002f, 0f);

    [Header("Blink Timing")]
    public float minBlinkInterval = 2.5f;
    public float maxBlinkInterval = 5.5f;
    public float closeDuration = 0.06f;
    public float holdDuration = 0.03f;
    public float openDuration = 0.10f;

    private Vector3 leftTopRest;
    private Vector3 rightTopRest;
    private Vector3 leftBottomRest;
    private Vector3 rightBottomRest;

    private float blinkTimer;
    private float nextBlinkTime;
    private bool isBlinking;

    // Records which avatar the current bone references belong to.
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
        }

        ScheduleNextBlink();
    }

    void Update()
    {
        // Automatically reassign if Avatar Root changes during Play Mode.
        if (autoAssignOnStart &&
            avatarRoot != null &&
            assignedAvatarRoot != avatarRoot)
        {
            ReassignBonesFromAvatarRoot();
        }

        blinkTimer += Time.deltaTime;

        if (!isBlinking && blinkTimer >= nextBlinkTime)
        {
            StartCoroutine(Blink());
        }
    }

    public void SetAvatarRoot(Transform newAvatarRoot)
    {
        if (newAvatarRoot == null)
        {
            Debug.LogWarning(
                "FaceBlinkController: New Avatar Root is null.",
                this
            );
            return;
        }

        avatarRoot = newAvatarRoot;
        ReassignBonesFromAvatarRoot();
    }

    [ContextMenu("Reassign Blink Bones From Avatar Root")]
    public void ReassignBonesFromAvatarRoot()
    {
        if (avatarRoot == null)
        {
            Debug.LogWarning(
                "Avatar Root is not assigned. Blink bone assignment skipped.",
                this
            );

            return;
        }

        // Stop a blink that may still be controlling the previous avatar.
        StopAllCoroutines();
        isBlinking = false;

        ClearBoneReferences();

        Transform[] allBones =
            avatarRoot.GetComponentsInChildren<Transform>(true);

        leftBlinkTop = FindBone(
            allBones,
            "Bip01 LEyeBlinkTop",
            "LEyeBlinkTop",
            "LeftEyeBlinkTop",
            "LeftBlinkTop"
        );

        rightBlinkTop = FindBone(
            allBones,
            "Bip01 REyeBlinkTop",
            "REyeBlinkTop",
            "RightEyeBlinkTop",
            "RightBlinkTop"
        );

        leftBlinkBottom = FindBone(
            allBones,
            "Bip01 LEyeBlinkBottom",
            "LEyeBlinkBottom",
            "LeftEyeBlinkBottom",
            "LeftBlinkBottom",
            "LeftLowerLid"
        );

        rightBlinkBottom = FindBone(
            allBones,
            "Bip01 REyeBlinkBottom",
            "REyeBlinkBottom",
            "RightEyeBlinkBottom",
            "RightBlinkBottom",
            "RightLowerLid"
        );

        assignedAvatarRoot = avatarRoot;

        // Save the neutral positions of the newly assigned avatar.
        SaveRestPositions();

        blinkTimer = 0f;
        ScheduleNextBlink();

        Debug.Log(
            $"Blink bones assigned from Avatar Root: " +
            $"{GetHierarchyPath(avatarRoot)}",
            this
        );

        LogAssignedBone("Left blink top", leftBlinkTop);
        LogAssignedBone("Right blink top", rightBlinkTop);
        LogAssignedBone("Left blink bottom", leftBlinkBottom);
        LogAssignedBone("Right blink bottom", rightBlinkBottom);

        LogMissingBones();
    }

    void ClearBoneReferences()
    {
        leftBlinkTop = null;
        rightBlinkTop = null;
        leftBlinkBottom = null;
        rightBlinkBottom = null;
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
                if (bone == null || !bone.IsChildOf(avatarRoot))
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
                if (bone == null || !bone.IsChildOf(avatarRoot))
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
        if (leftBlinkTop != null)
        {
            leftTopRest = leftBlinkTop.localPosition;
        }

        if (rightBlinkTop != null)
        {
            rightTopRest = rightBlinkTop.localPosition;
        }

        if (leftBlinkBottom != null)
        {
            leftBottomRest = leftBlinkBottom.localPosition;
        }

        if (rightBlinkBottom != null)
        {
            rightBottomRest = rightBlinkBottom.localPosition;
        }
    }

    IEnumerator Blink()
    {
        isBlinking = true;

        // Close eyes.
        float t = 0f;

        while (t < closeDuration)
        {
            t += Time.deltaTime;

            float amount =
                Mathf.Clamp01(t / closeDuration);

            ApplyBlink(amount);

            yield return null;
        }

        ApplyBlink(1f);

        // Hold eyes closed briefly.
        yield return new WaitForSeconds(holdDuration);

        // Open eyes.
        t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;

            float amount =
                1f - Mathf.Clamp01(t / openDuration);

            ApplyBlink(amount);

            yield return null;
        }

        ApplyBlink(0f);

        isBlinking = false;
        blinkTimer = 0f;

        ScheduleNextBlink();
    }

    void ApplyBlink(float amount)
    {
        if (leftBlinkTop != null)
        {
            leftBlinkTop.localPosition =
                leftTopRest +
                leftTopBlinkOffset * amount;
        }

        if (rightBlinkTop != null)
        {
            rightBlinkTop.localPosition =
                rightTopRest +
                rightTopBlinkOffset * amount;
        }

        if (leftBlinkBottom != null)
        {
            leftBlinkBottom.localPosition =
                leftBottomRest +
                leftBottomBlinkOffset * amount;
        }

        if (rightBlinkBottom != null)
        {
            rightBlinkBottom.localPosition =
                rightBottomRest +
                rightBottomBlinkOffset * amount;
        }
    }

    void ScheduleNextBlink()
    {
        nextBlinkTime =
            Random.Range(
                minBlinkInterval,
                maxBlinkInterval
            );
    }

    void LogAssignedBone(string label, Transform bone)
    {
        if (bone == null)
        {
            return;
        }

        Debug.Log(
            $"{label} assigned to: {GetHierarchyPath(bone)}",
            bone
        );
    }

    void LogMissingBones()
    {
        if (leftBlinkTop == null)
        {
            Debug.LogWarning(
                "Left blink top bone was not found.",
                this
            );
        }

        if (rightBlinkTop == null)
        {
            Debug.LogWarning(
                "Right blink top bone was not found.",
                this
            );
        }

        if (leftBlinkBottom == null)
        {
            Debug.LogWarning(
                "Left blink bottom bone was not found.",
                this
            );
        }

        if (rightBlinkBottom == null)
        {
            Debug.LogWarning(
                "Right blink bottom bone was not found.",
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