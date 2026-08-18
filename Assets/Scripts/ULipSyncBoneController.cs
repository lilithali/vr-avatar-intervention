using UnityEngine;
using uLipSync;

public class ULipSyncBoneController : MonoBehaviour
{
    [Header("Auto Bone Assignment")]
    public Transform avatarRoot;
    public bool autoAssignOnStart = true;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Bones")]
    public Transform jawBone;

    public Transform middleUpperLip;
    public Transform leftUpperLip;
    public Transform rightUpperLip;

    public Transform leftMouthCorner;
    public Transform rightMouthCorner;

    public Transform leftCheek;
    public Transform rightCheek;

    [Header("Lower Eyelid Bones")]
    public Transform leftEyeBlinkBottom;
    public Transform rightEyeBlinkBottom;

    [Header("Masseter Bones")]
    public Transform leftMasseter;
    public Transform rightMasseter;

    [Header("Group Switches")]
    public bool enableMPBGroup = false;

    [Header("Manual Test")]
    public bool manualTestMode = false;

    [Range(0, 4)]
    public int testPhoneme = 0;
    // 0 = neutral
    // 1 = a / e / i
    // 2 = m / p / b
    // 3 = o
    // 4 = u

    [Range(0f, 1f)]
    public float testOpenAmount = 1f;

    [Header("Jaw Rotation Offsets")]
    public Vector3 jawAEIGroupOffset = new Vector3(0f, 0f, -3f);
    public Vector3 jawOUGroupOffset = new Vector3(0f, 0f, -2f);

    [Header("Volume")]
    public float minRawVolume = 0.015f;
    public float maxRawVolume = 0.08f;

    [Header("A / E / I Group Upper Lip Offsets")]
    public Vector3 middleUpperLipAEIGroupOffset =
        new Vector3(0f, 0.002f, 0f);

    public Vector3 leftUpperLipAEIGroupOffset =
        new Vector3(0f, 0.001f, 0f);

    public Vector3 rightUpperLipAEIGroupOffset =
        new Vector3(0f, 0.001f, 0f);

    [Header("O / U Group Upper Lip Offsets")]
    public Vector3 middleUpperLipOUGroupOffset =
        new Vector3(0f, -0.001f, 0f);

    public Vector3 leftUpperLipOUGroupOffset =
        new Vector3(0f, -0.001f, 0f);

    public Vector3 rightUpperLipOUGroupOffset =
        new Vector3(0f, -0.001f, 0f);

    [Header("A / E / I Group Mouth Corner Offsets")]
    public Vector3 leftCornerAEIGroupOffset =
        new Vector3(-0.001f, 0.0005f, 0f);

    public Vector3 rightCornerAEIGroupOffset =
        new Vector3(0.001f, 0.0005f, 0f);

    [Header("M / P / B Group Mouth Corner Offsets")]
    public Vector3 leftCornerMPBGroupOffset =
        new Vector3(-0.0015f, 0f, 0f);

    public Vector3 rightCornerMPBGroupOffset =
        new Vector3(0.0015f, 0f, 0f);

    [Header("O / U Group Mouth Corner Offsets")]
    public Vector3 leftCornerOUGroupOffset =
        new Vector3(0.0005f, -0.0005f, 0f);

    public Vector3 rightCornerOUGroupOffset =
        new Vector3(-0.0005f, -0.0005f, 0f);

    [Header("Cheek Position Offsets")]
    public Vector3 leftCheekTalkOffset =
        new Vector3(-0.001f, -0.001f, 0f);

    public Vector3 rightCheekTalkOffset =
        new Vector3(0.001f, -0.001f, 0f);

    [Header("Lower Eyelid Speaking Motion")]
    public Vector3 leftLowerLidTalkOffset =
        new Vector3(0f, 0.00015f, 0f);

    public Vector3 rightLowerLidTalkOffset =
        new Vector3(0f, 0.00015f, 0f);

    [Range(0f, 1f)]
    public float lowerLidWaveAmount = 0.2f;

    public float lowerLidWaveSpeed = 5f;

    [Header("Masseter A / E / I Position Offsets")]
    public Vector3 leftMasseterAEIOffset =
        new Vector3(0f, -0.001f, 0f);

    public Vector3 rightMasseterAEIOffset =
        new Vector3(0f, -0.001f, 0f);

    [Header("Masseter M / P / B Position Offsets")]
    public Vector3 leftMasseterMPBOffset =
        new Vector3(-0.0005f, 0.0003f, 0f);

    public Vector3 rightMasseterMPBOffset =
        new Vector3(0.0005f, 0.0003f, 0f);

    [Header("Masseter O Position Offsets")]
    public Vector3 leftMasseterOOffset =
        new Vector3(0.0005f, -0.0005f, 0f);

    public Vector3 rightMasseterOOffset =
        new Vector3(-0.0005f, -0.0005f, 0f);

    [Header("Masseter U Position Offsets")]
    public Vector3 leftMasseterUOffset =
        new Vector3(0.0008f, -0.0003f, 0f);

    public Vector3 rightMasseterUOffset =
        new Vector3(-0.0008f, -0.0003f, 0f);

    [Header("Motion")]
    public float audioSmoothSpeed = 6f;
    public float openSpeed = 20f;
    public float closeSpeed = 30f;
    public float phonemeSmoothSpeed = 8f;
    public float closeThreshold = 0.12f;

    private Vector3 jawClosed;

    private Vector3 middleUpperLipClosedPosition;
    private Vector3 leftUpperLipClosedPosition;
    private Vector3 rightUpperLipClosedPosition;

    private Vector3 leftCornerClosedPosition;
    private Vector3 rightCornerClosedPosition;

    private Vector3 leftCheekClosedPosition;
    private Vector3 rightCheekClosedPosition;

    private Vector3 leftEyeBlinkBottomClosedPosition;
    private Vector3 rightEyeBlinkBottomClosedPosition;

    private Vector3 leftMasseterClosedPosition;
    private Vector3 rightMasseterClosedPosition;

    private readonly float[] samples = new float[256];

    private float currentOpenAmount = 0f;
    private float targetOpenAmount = 0f;
    private float smoothedRawOpen = 0f;

    private float aeiGroupAmount = 1f;
    private float mpbGroupAmount = 0f;
    private float ouGroupAmount = 0f;

    private float targetAEIGroup = 1f;
    private float targetMPBGroup = 0f;
    private float targetOUGroup = 0f;

    private float aeiMasseterAmount = 0f;
    private float mpbMasseterAmount = 0f;
    private float oMasseterAmount = 0f;
    private float uMasseterAmount = 0f;

    private float targetAEIMasseter = 0f;
    private float targetMPBMasseter = 0f;
    private float targetOMasseter = 0f;
    private float targetUMasseter = 0f;

    // Records which Avatar Root the current bone references came from.
    // This prevents references from a previous avatar being reused.
    private Transform assignedAvatarRoot;

    private void Start()
    {
        if (autoAssignOnStart)
        {
            ForceReassignBones();
        }
        else
        {
            SaveRestPositions();
        }
    }

    private void Update()
    {
        // If Avatar Root is changed during Play Mode, automatically bind
        // the bones belonging to the new root.
        if (autoAssignOnStart &&
            avatarRoot != null &&
            assignedAvatarRoot != avatarRoot)
        {
            ForceReassignBones();
        }

        if (manualTestMode)
        {
            TestPhonemeManually();
        }
        else
        {
            UpdateRawVolumeOpenAmount();
        }

        SmoothValues();
    }

    private void LateUpdate()
    {
        // Animator 更新完成后，再覆盖面部骨骼。
        ApplyMouth();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Reassign immediately when Avatar Root is changed in the Inspector.
        // Do not save rest positions here because OnValidate can run often.
        if (!Application.isPlaying &&
            autoAssignOnStart &&
            avatarRoot != null &&
            assignedAvatarRoot != avatarRoot)
        {
            AutoAssignBones();
        }
    }
#endif

    /// <summary>
    /// Changes the active Avatar Root and immediately reassigns all
    /// lip-sync-related bones from the selected avatar.
    /// </summary>
    public void SetAvatarRoot(Transform newAvatarRoot)
    {
        if (newAvatarRoot == null)
        {
            Debug.LogWarning(
                "ULipSyncBoneController: New Avatar Root is null.",
                this
            );
            return;
        }

        avatarRoot = newAvatarRoot;
        ForceReassignBones();
    }

    [ContextMenu("Reassign Bones From Avatar Root")]
    public void ForceReassignBones()
    {
        if (avatarRoot == null)
        {
            Debug.LogWarning(
                "Avatar Root is not assigned. Bone reassignment was skipped.",
                this
            );
            return;
        }

        ClearBoneReferences();
        AutoAssignBones();
        SaveRestPositions();

        // Reset the smoothed values so the newly assigned avatar starts
        // from its own neutral pose.
        currentOpenAmount = 0f;
        targetOpenAmount = 0f;
        smoothedRawOpen = 0f;

        assignedAvatarRoot = avatarRoot;

        Debug.Log(
            $"Bones reassigned only from Avatar Root: {GetHierarchyPath(avatarRoot)}",
            this
        );
    }

    private void ClearBoneReferences()
    {
        jawBone = null;

        middleUpperLip = null;
        leftUpperLip = null;
        rightUpperLip = null;

        leftMouthCorner = null;
        rightMouthCorner = null;

        leftCheek = null;
        rightCheek = null;

        leftEyeBlinkBottom = null;
        rightEyeBlinkBottom = null;

        leftMasseter = null;
        rightMasseter = null;
    }

    public void AutoAssignBones()
    {
        if (avatarRoot == null)
        {
            Debug.LogWarning(
                "Avatar Root is not assigned. Auto bone assignment skipped."
            );
            return;
        }

        Transform[] allBones =
            avatarRoot.GetComponentsInChildren<Transform>(true);

        jawBone = FindBone(allBones, "mjaw", "jaw");

        middleUpperLip = FindBone(
            allBones,
            "mupperlip",
            "middleupperlip",
            "upperlipm",
            "upperlipmiddle"
        );

        leftUpperLip = FindBone(
            allBones,
            "lupperlip",
            "leftupperlip",
            "upperlipl",
            "upperlipleft"
        );

        rightUpperLip = FindBone(
            allBones,
            "rupperlip",
            "rightupperlip",
            "upperlipr",
            "upperlipright"
        );

        leftMouthCorner = FindBone(
            allBones,
            "lmouthcorner",
            "leftmouthcorner",
            "mouthcornerl",
            "mouthcornerleft"
        );

        rightMouthCorner = FindBone(
            allBones,
            "rmouthcorner",
            "rightmouthcorner",
            "mouthcornerr",
            "mouthcornerright"
        );

        leftCheek = FindBone(
            allBones,
            "lcheek",
            "leftcheek",
            "cheekl",
            "cheekleft"
        );

        rightCheek = FindBone(
            allBones,
            "rcheek",
            "rightcheek",
            "cheekr",
            "cheekright"
        );

        leftEyeBlinkBottom = FindBone(
            allBones,
            "leyeblinkbottom",
            "leyeclosebottom",
            "leftblinkbottom",
            "lefteyeblinkbottom",
            "leftlowerlid",
            "llowerlid"
        );

        rightEyeBlinkBottom = FindBone(
            allBones,
            "reyeblinkbottom",
            "reyeclosebottom",
            "rightblinkbottom",
            "righteyeblinkbottom",
            "rightlowerlid",
            "rlowerlid"
        );

        leftMasseter = FindBone(
            allBones,
            "lmasseter",
            "leftmasseter",
            "masseterl",
            "masseterleft"
        );

        rightMasseter = FindBone(
            allBones,
            "rmasseter",
            "rightmasseter",
            "masseterr",
            "masseterright"
        );

        assignedAvatarRoot = avatarRoot;

        Debug.Log(
            $"Auto bone assignment completed from: {GetHierarchyPath(avatarRoot)}",
            this
        );

        LogAssignedBone("Jaw", jawBone);
        LogAssignedBone("Middle upper lip", middleUpperLip);
        LogAssignedBone("Left upper lip", leftUpperLip);
        LogAssignedBone("Right upper lip", rightUpperLip);
        LogAssignedBone("Left mouth corner", leftMouthCorner);
        LogAssignedBone("Right mouth corner", rightMouthCorner);
        LogAssignedBone("Left cheek", leftCheek);
        LogAssignedBone("Right cheek", rightCheek);
        LogAssignedBone("Left lower eyelid", leftEyeBlinkBottom);
        LogAssignedBone("Right lower eyelid", rightEyeBlinkBottom);
        LogAssignedBone("Left masseter", leftMasseter);
        LogAssignedBone("Right masseter", rightMasseter);

        LogMissingBones();
    }

    private Transform FindBone(
        Transform[] bones,
        params string[] keywords
    )
    {
        if (avatarRoot == null || bones == null)
        {
            return null;
        }

        // First pass: prefer an exact normalized name match.
        foreach (string keyword in keywords)
        {
            string key = NormalizeName(keyword);

            foreach (Transform bone in bones)
            {
                if (bone == null || !bone.IsChildOf(avatarRoot))
                {
                    continue;
                }

                if (NormalizeName(bone.name) == key)
                {
                    return bone;
                }
            }
        }

        // Second pass: allow partial matches for models with prefixes.
        foreach (string keyword in keywords)
        {
            string key = NormalizeName(keyword);

            foreach (Transform bone in bones)
            {
                if (bone == null || !bone.IsChildOf(avatarRoot))
                {
                    continue;
                }

                if (NormalizeName(bone.name).Contains(key))
                {
                    return bone;
                }
            }
        }

        return null;
    }

    private void LogAssignedBone(string label, Transform bone)
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

    private string GetHierarchyPath(Transform target)
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

    private string NormalizeName(string name)
    {
        return name
            .ToLower()
            .Replace(" ", "")
            .Replace("_", "")
            .Replace("-", "");
    }

    private void LogMissingBones()
    {
        if (jawBone == null)
        {
            Debug.LogWarning("Jaw bone not found.");
        }

        if (middleUpperLip == null)
        {
            Debug.LogWarning("Middle upper lip bone not found.");
        }

        if (leftUpperLip == null)
        {
            Debug.LogWarning("Left upper lip bone not found.");
        }

        if (rightUpperLip == null)
        {
            Debug.LogWarning("Right upper lip bone not found.");
        }

        if (leftMouthCorner == null)
        {
            Debug.LogWarning("Left mouth corner bone not found.");
        }

        if (rightMouthCorner == null)
        {
            Debug.LogWarning("Right mouth corner bone not found.");
        }

        if (leftCheek == null)
        {
            Debug.LogWarning("Left cheek bone not found.");
        }

        if (rightCheek == null)
        {
            Debug.LogWarning("Right cheek bone not found.");
        }

        if (leftEyeBlinkBottom == null)
        {
            Debug.LogWarning("Left lower eyelid bone not found.");
        }

        if (rightEyeBlinkBottom == null)
        {
            Debug.LogWarning("Right lower eyelid bone not found.");
        }

        if (leftMasseter == null)
        {
            Debug.LogWarning("Left masseter bone not found.");
        }

        if (rightMasseter == null)
        {
            Debug.LogWarning("Right masseter bone not found.");
        }
    }

    private void SaveRestPositions()
    {
        if (jawBone != null)
        {
            jawClosed = jawBone.localEulerAngles;
        }

        if (middleUpperLip != null)
        {
            middleUpperLipClosedPosition =
                middleUpperLip.localPosition;
        }

        if (leftUpperLip != null)
        {
            leftUpperLipClosedPosition =
                leftUpperLip.localPosition;
        }

        if (rightUpperLip != null)
        {
            rightUpperLipClosedPosition =
                rightUpperLip.localPosition;
        }

        if (leftMouthCorner != null)
        {
            leftCornerClosedPosition =
                leftMouthCorner.localPosition;
        }

        if (rightMouthCorner != null)
        {
            rightCornerClosedPosition =
                rightMouthCorner.localPosition;
        }

        if (leftCheek != null)
        {
            leftCheekClosedPosition =
                leftCheek.localPosition;
        }

        if (rightCheek != null)
        {
            rightCheekClosedPosition =
                rightCheek.localPosition;
        }

        if (leftEyeBlinkBottom != null)
        {
            leftEyeBlinkBottomClosedPosition =
                leftEyeBlinkBottom.localPosition;
        }

        if (rightEyeBlinkBottom != null)
        {
            rightEyeBlinkBottomClosedPosition =
                rightEyeBlinkBottom.localPosition;
        }

        if (leftMasseter != null)
        {
            leftMasseterClosedPosition =
                leftMasseter.localPosition;
        }

        if (rightMasseter != null)
        {
            rightMasseterClosedPosition =
                rightMasseter.localPosition;
        }
    }

    private void TestPhonemeManually()
    {
        targetOpenAmount = testOpenAmount;

        ResetTargets();

        if (testPhoneme == 1)
        {
            targetAEIGroup = 1f;
            targetAEIMasseter = 1f;
        }
        else if (testPhoneme == 2)
        {
            if (enableMPBGroup)
            {
                targetMPBGroup = 1f;
                targetMPBMasseter = 1f;
            }
            else
            {
                targetOpenAmount = 0f;
            }
        }
        else if (testPhoneme == 3)
        {
            targetOUGroup = 1f;
            targetOMasseter = 1f;
        }
        else if (testPhoneme == 4)
        {
            targetOUGroup = 1f;
            targetUMasseter = 1f;
        }
        else
        {
            targetOpenAmount = 0f;
        }
    }

    private void UpdateRawVolumeOpenAmount()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            targetOpenAmount = 0f;

            smoothedRawOpen = Mathf.Lerp(
                smoothedRawOpen,
                0f,
                Time.deltaTime * audioSmoothSpeed
            );

            return;
        }

        float rawVolume = GetRawVolume();

        float rawOpen = Mathf.InverseLerp(
            minRawVolume,
            maxRawVolume,
            rawVolume
        );

        rawOpen = Mathf.Clamp01(rawOpen);

        smoothedRawOpen = Mathf.Lerp(
            smoothedRawOpen,
            rawOpen,
            Time.deltaTime * audioSmoothSpeed
        );

        if (smoothedRawOpen < closeThreshold)
        {
            smoothedRawOpen = 0f;
        }

        targetOpenAmount = smoothedRawOpen;
    }

    private void SmoothValues()
    {
        float speed = targetOpenAmount > currentOpenAmount
            ? openSpeed
            : closeSpeed;

        currentOpenAmount = Mathf.MoveTowards(
            currentOpenAmount,
            targetOpenAmount,
            Time.deltaTime * speed
        );

        aeiGroupAmount = Mathf.Lerp(
            aeiGroupAmount,
            targetAEIGroup,
            Time.deltaTime * phonemeSmoothSpeed
        );

        mpbGroupAmount = Mathf.Lerp(
            mpbGroupAmount,
            targetMPBGroup,
            Time.deltaTime * phonemeSmoothSpeed
        );

        ouGroupAmount = Mathf.Lerp(
            ouGroupAmount,
            targetOUGroup,
            Time.deltaTime * phonemeSmoothSpeed
        );

        aeiMasseterAmount = Mathf.Lerp(
            aeiMasseterAmount,
            targetAEIMasseter,
            Time.deltaTime * phonemeSmoothSpeed
        );

        mpbMasseterAmount = Mathf.Lerp(
            mpbMasseterAmount,
            targetMPBMasseter,
            Time.deltaTime * phonemeSmoothSpeed
        );

        oMasseterAmount = Mathf.Lerp(
            oMasseterAmount,
            targetOMasseter,
            Time.deltaTime * phonemeSmoothSpeed
        );

        uMasseterAmount = Mathf.Lerp(
            uMasseterAmount,
            targetUMasseter,
            Time.deltaTime * phonemeSmoothSpeed
        );
    }

    public void OnLipSyncUpdate(LipSyncInfo info)
    {
        if (manualTestMode)
        {
            return;
        }

        string phoneme = string.Empty;

        if (!string.IsNullOrEmpty(info.phoneme))
        {
            phoneme = info.phoneme.ToLower();
        }

        ResetTargets();

        if (phoneme.Contains("a") ||
            phoneme.Contains("e") ||
            phoneme.Contains("i"))
        {
            targetAEIGroup = 1f;
            targetAEIMasseter = 1f;
        }
        else if (phoneme.Contains("m") ||
                 phoneme.Contains("p") ||
                 phoneme.Contains("b"))
        {
            if (enableMPBGroup)
            {
                targetMPBGroup = 1f;
                targetMPBMasseter = 1f;
            }

            targetOpenAmount = 0f;
            smoothedRawOpen = 0f;
        }
        else if (phoneme.Contains("o"))
        {
            targetOUGroup = 1f;
            targetOMasseter = 1f;
        }
        else if (phoneme.Contains("u"))
        {
            targetOUGroup = 1f;
            targetUMasseter = 1f;
        }
        else
        {
            targetAEIGroup = 1f;
            targetAEIMasseter = 1f;
        }
    }

    private void ResetTargets()
    {
        targetAEIGroup = 0f;
        targetMPBGroup = 0f;
        targetOUGroup = 0f;

        targetAEIMasseter = 0f;
        targetMPBMasseter = 0f;
        targetOMasseter = 0f;
        targetUMasseter = 0f;
    }

    private void ApplyMouth()
    {
        float open = currentOpenAmount;

        if (jawBone != null)
        {
            Vector3 jawOffset =
                jawAEIGroupOffset * aeiGroupAmount +
                jawOUGroupOffset * ouGroupAmount;

            jawBone.localEulerAngles =
                jawClosed +
                jawOffset * open;
        }

        if (middleUpperLip != null)
        {
            Vector3 offset =
                middleUpperLipAEIGroupOffset * aeiGroupAmount +
                middleUpperLipOUGroupOffset * ouGroupAmount;

            middleUpperLip.localPosition =
                middleUpperLipClosedPosition +
                offset * open;
        }

        if (leftUpperLip != null)
        {
            Vector3 offset =
                leftUpperLipAEIGroupOffset * aeiGroupAmount +
                leftUpperLipOUGroupOffset * ouGroupAmount;

            leftUpperLip.localPosition =
                leftUpperLipClosedPosition +
                offset * open;
        }

        if (rightUpperLip != null)
        {
            Vector3 offset =
                rightUpperLipAEIGroupOffset * aeiGroupAmount +
                rightUpperLipOUGroupOffset * ouGroupAmount;

            rightUpperLip.localPosition =
                rightUpperLipClosedPosition +
                offset * open;
        }

        Vector3 leftCornerOffset =
            leftCornerAEIGroupOffset * aeiGroupAmount +
            leftCornerMPBGroupOffset * mpbGroupAmount +
            leftCornerOUGroupOffset * ouGroupAmount;

        Vector3 rightCornerOffset =
            rightCornerAEIGroupOffset * aeiGroupAmount +
            rightCornerMPBGroupOffset * mpbGroupAmount +
            rightCornerOUGroupOffset * ouGroupAmount;

        if (leftMouthCorner != null)
        {
            leftMouthCorner.localPosition =
                leftCornerClosedPosition +
                leftCornerOffset * open;
        }

        if (rightMouthCorner != null)
        {
            rightMouthCorner.localPosition =
                rightCornerClosedPosition +
                rightCornerOffset * open;
        }

        if (leftCheek != null)
        {
            leftCheek.localPosition =
                leftCheekClosedPosition +
                leftCheekTalkOffset * open;
        }

        if (rightCheek != null)
        {
            rightCheek.localPosition =
                rightCheekClosedPosition +
                rightCheekTalkOffset * open;
        }

        float lowerLidWave =
            1f +
            Mathf.Sin(Time.time * lowerLidWaveSpeed) *
            lowerLidWaveAmount;

        float lowerLidTalkAmount = open * lowerLidWave;

        if (leftEyeBlinkBottom != null)
        {
            leftEyeBlinkBottom.localPosition =
                leftEyeBlinkBottomClosedPosition +
                leftLowerLidTalkOffset * lowerLidTalkAmount;
        }

        if (rightEyeBlinkBottom != null)
        {
            rightEyeBlinkBottom.localPosition =
                rightEyeBlinkBottomClosedPosition +
                rightLowerLidTalkOffset * lowerLidTalkAmount;
        }

        Vector3 leftMasseterOffset =
            leftMasseterAEIOffset * aeiMasseterAmount +
            leftMasseterMPBOffset * mpbMasseterAmount +
            leftMasseterOOffset * oMasseterAmount +
            leftMasseterUOffset * uMasseterAmount;

        Vector3 rightMasseterOffset =
            rightMasseterAEIOffset * aeiMasseterAmount +
            rightMasseterMPBOffset * mpbMasseterAmount +
            rightMasseterOOffset * oMasseterAmount +
            rightMasseterUOffset * uMasseterAmount;

        if (leftMasseter != null)
        {
            leftMasseter.localPosition =
                leftMasseterClosedPosition +
                leftMasseterOffset * open;
        }

        if (rightMasseter != null)
        {
            rightMasseter.localPosition =
                rightMasseterClosedPosition +
                rightMasseterOffset * open;
        }
    }

    private float GetRawVolume()
    {
        if (audioSource == null)
        {
            return 0f;
        }

        audioSource.GetOutputData(samples, 0);

        float volume = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            volume += Mathf.Abs(samples[i]);
        }

        volume /= samples.Length;

        return volume;
    }
}