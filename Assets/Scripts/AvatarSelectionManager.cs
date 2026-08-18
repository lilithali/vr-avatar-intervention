using System;
using UnityEngine;

public class AvatarSelectionManager : MonoBehaviour
{
    public enum AvatarGender
    {
        Male,
        Female
    }

    public enum AvatarEthnicity
    {
        White,
        Asian,
        Black,
        Brown
    }


    [Serializable]
    public class AvatarOption
    {
        [Header("Avatar Category")]

        [Tooltip("这个 Avatar 的性别")]
        public AvatarGender gender;

        [Tooltip("这个 Avatar 的外观分类")]
        public AvatarEthnicity ethnicity;


        [Header("Avatar Object")]

        [Tooltip("场景中的完整 Avatar 根物体，用于显示和隐藏")]
        public GameObject avatarObject;

        [Tooltip(
            "交给 Lip Sync、眨眼和眉毛控制器使用的 Avatar Root"
        )]
        public Transform avatarRoot;
    }


    [Header("Available Avatars")]

    [Tooltip("场景中可供选择的所有 Avatar")]
    [SerializeField]
    private AvatarOption[] avatarOptions;


    [Header("Face Controllers")]

    [Tooltip("VoiceAudio 上的 ULipSyncBoneController")]
    [SerializeField]
    private ULipSyncBoneController lipSyncBoneController;

    [Tooltip("FaceController 上的 FaceBlinkController")]
    [SerializeField]
    private FaceBlinkController faceBlinkController;

    [Tooltip("FaceController 上的 EyebrowMotionController")]
    [SerializeField]
    private EyebrowMotionController eyebrowMotionController;


    [Header("Dialogue Controller")]

    [Tooltip(
        "负责播放人物音频、显示字幕和响应 Space 的控制器"
    )]
    [SerializeField]
    private AudioSubtitleController audioSubtitleController;


    [Header("Male Voice Clips")]

    [Tooltip("男性 Avatar 使用的四段音频")]
    [SerializeField]
    private AudioClip[] maleVoiceClips;


    [Header("Female Voice Clips")]

    [Tooltip("女性 Avatar 使用的四段音频")]
    [SerializeField]
    private AudioClip[] femaleVoiceClips;


    [Header("Starting Behaviour")]

    [Tooltip("游戏开始时是否先关闭全部 Avatar")]
    [SerializeField]
    private bool hideAllAvatarsOnStart = true;


    private AvatarOption currentAvatar;


    private void Start()
    {
        /*
         * VR Scene 开始时，先关闭全部 Avatar。
         */
        if (hideAllAvatarsOnStart)
        {
            HideAllAvatars();
        }

        /*
         * 在读取保存的选择之前，先停止对话。
         */
        if (audioSubtitleController != null)
        {
            audioSubtitleController.StopDialogue();
        }

        /*
         * 读取在 AvatarSelectionScene 中保存的选择。
         */
        ApplySavedSelection();
    }


    /// <summary>
    /// 从 AvatarSelectionData 中读取之前保存的选择。
    /// </summary>
    private void ApplySavedSelection()
    {
        if (AvatarSelectionData.Instance == null)
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "没有找到 AvatarSelectionData。"
                + "请确认是从 AvatarSelectionScene 进入当前场景。",
                this
            );

            return;
        }

        if (!AvatarSelectionData.Instance.hasSelection)
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "AvatarSelectionData 中还没有保存人物选择。",
                this
            );

            return;
        }

        SelectAvatar(
            AvatarSelectionData.Instance.selectedGender,
            AvatarSelectionData.Instance.selectedEthnicity
        );
    }


    /// <summary>
    /// 根据性别和外观分类选择 Avatar。
    /// </summary>
    public void SelectAvatar(
        AvatarGender selectedGender,
        AvatarEthnicity selectedEthnicity
    )
    {
        if (avatarOptions == null ||
            avatarOptions.Length == 0)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + "Avatar Options 没有设置。",
                this
            );

            return;
        }

        AvatarOption matchedAvatar = null;

        foreach (AvatarOption option in avatarOptions)
        {
            if (option == null)
            {
                continue;
            }

            bool genderMatches =
                option.gender == selectedGender;

            bool ethnicityMatches =
                option.ethnicity == selectedEthnicity;

            if (genderMatches && ethnicityMatches)
            {
                matchedAvatar = option;
                break;
            }
        }

        if (matchedAvatar == null)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + "没有找到匹配的 Avatar。"
                + " Gender = "
                + selectedGender
                + ", Ethnicity = "
                + selectedEthnicity,
                this
            );

            return;
        }

        ActivateSelectedAvatar(matchedAvatar);
    }


    /// <summary>
    /// 显示选中的 Avatar，并更新相关控制器。
    /// </summary>
    private void ActivateSelectedAvatar(
        AvatarOption selectedAvatar
    )
    {
        if (selectedAvatar == null)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + "Selected Avatar 为空。",
                this
            );

            return;
        }

        if (selectedAvatar.avatarObject == null)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + "选中的 Avatar Object 没有连接。",
                this
            );

            return;
        }

        if (selectedAvatar.avatarRoot == null)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + "选中的 Avatar Root 没有连接。",
                this
            );

            return;
        }


        /*
         * 只显示选中的 Avatar，
         * 关闭其他所有 Avatar。
         */
        foreach (AvatarOption option in avatarOptions)
        {
            if (option == null ||
                option.avatarObject == null)
            {
                continue;
            }

            bool shouldBeVisible =
                option == selectedAvatar;

            option.avatarObject.SetActive(
                shouldBeVisible
            );
        }

        currentAvatar = selectedAvatar;


        /*
         * 更新 Lip Sync 的 Avatar Root。
         */
        if (lipSyncBoneController != null)
        {
            lipSyncBoneController.SetAvatarRoot(
                selectedAvatar.avatarRoot
            );
        }
        else
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "Lip Sync Bone Controller 没有连接。",
                this
            );
        }


        /*
         * 更新眨眼控制器的 Avatar Root。
         */
        if (faceBlinkController != null)
        {
            faceBlinkController.SetAvatarRoot(
                selectedAvatar.avatarRoot
            );
        }
        else
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "Face Blink Controller 没有连接。",
                this
            );
        }


        /*
         * 更新眉毛控制器的 Avatar Root。
         */
        if (eyebrowMotionController != null)
        {
            eyebrowMotionController.SetAvatarRoot(
                selectedAvatar.avatarRoot
            );
        }
        else
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "Eyebrow Motion Controller 没有连接。",
                this
            );
        }


        /*
         * 根据选中的 Avatar 性别，
         * 选择男声或女声。
         */
        AudioClip[] selectedVoiceClips =
            GetVoiceClipsForGender(
                selectedAvatar.gender
            );

        if (selectedVoiceClips == null ||
            selectedVoiceClips.Length == 0)
        {
            Debug.LogError(
                "AvatarSelectionManager: "
                + selectedAvatar.gender
                + " Voice Clips 没有设置。",
                this
            );

            return;
        }


        /*
         * 把声音交给 AudioSubtitleController。
         * StartDialogue() 后等待玩家按 Space 播放第一段。
         */
        if (audioSubtitleController != null)
        {
            audioSubtitleController.SetVoiceClips(
                selectedVoiceClips
            );

            audioSubtitleController.StartDialogue();
        }
        else
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "Audio Subtitle Controller 没有连接。",
                this
            );
        }


        Debug.Log(
            "Avatar selected: "
            + selectedAvatar.gender
            + " / "
            + selectedAvatar.ethnicity
            + ". Voice clips assigned: "
            + selectedVoiceClips.Length
            + ".",
            selectedAvatar.avatarObject
        );
    }


    private AudioClip[] GetVoiceClipsForGender(
        AvatarGender gender
    )
    {
        if (gender == AvatarGender.Male)
        {
            return maleVoiceClips;
        }

        return femaleVoiceClips;
    }


    public void HideAllAvatars()
    {
        if (avatarOptions == null)
        {
            return;
        }

        foreach (AvatarOption option in avatarOptions)
        {
            if (option != null &&
                option.avatarObject != null)
            {
                option.avatarObject.SetActive(false);
            }
        }

        currentAvatar = null;
    }


    public void ClearSelection()
    {
        HideAllAvatars();

        if (audioSubtitleController != null)
        {
            audioSubtitleController.StopDialogue();
        }
    }


    public GameObject GetCurrentAvatarObject()
    {
        if (currentAvatar == null)
        {
            return null;
        }

        return currentAvatar.avatarObject;
    }


    public Transform GetCurrentAvatarRoot()
    {
        if (currentAvatar == null)
        {
            return null;
        }

        return currentAvatar.avatarRoot;
    }


    public AvatarGender GetCurrentAvatarGender()
    {
        if (currentAvatar == null)
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "目前还没有选择 Avatar。",
                this
            );

            return AvatarGender.Female;
        }

        return currentAvatar.gender;
    }


    public AvatarEthnicity GetCurrentAvatarEthnicity()
    {
        if (currentAvatar == null)
        {
            Debug.LogWarning(
                "AvatarSelectionManager: "
                + "目前还没有选择 Avatar。",
                this
            );

            return AvatarEthnicity.White;
        }

        return currentAvatar.ethnicity;
    }


    // =========================================================
    // UI Button Methods
    // 如果以后需要在当前 Scene 直接放选择按钮，也可以使用。
    // =========================================================

    public void SelectMaleWhite()
    {
        SelectAvatar(
            AvatarGender.Male,
            AvatarEthnicity.White
        );
    }


    public void SelectMaleAsian()
    {
        SelectAvatar(
            AvatarGender.Male,
            AvatarEthnicity.Asian
        );
    }


    public void SelectMaleBlack()
    {
        SelectAvatar(
            AvatarGender.Male,
            AvatarEthnicity.Black
        );
    }


    public void SelectMaleBrown()
    {
        SelectAvatar(
            AvatarGender.Male,
            AvatarEthnicity.Brown
        );
    }


    public void SelectFemaleWhite()
    {
        SelectAvatar(
            AvatarGender.Female,
            AvatarEthnicity.White
        );
    }


    public void SelectFemaleAsian()
    {
        SelectAvatar(
            AvatarGender.Female,
            AvatarEthnicity.Asian
        );
    }


    public void SelectFemaleBlack()
    {
        SelectAvatar(
            AvatarGender.Female,
            AvatarEthnicity.Black
        );
    }


    public void SelectFemaleBrown()
    {
        SelectAvatar(
            AvatarGender.Female,
            AvatarEthnicity.Brown
        );
    }
}