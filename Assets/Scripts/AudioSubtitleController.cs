using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSubtitleController : MonoBehaviour
{
    [Header("Audio")]

    [Tooltip("播放人物声音的 AudioSource，建议使用现有的 VoiceAudio")]
    [SerializeField]
    private AudioSource voiceAudioSource;

    [Tooltip("按照播放顺序放入人物音频")]
    [SerializeField]
    private AudioClip[] voiceClips;


    [Header("Voice Subtitles")]

    [Tooltip("每段人物音频播放时显示的字幕，顺序应与音频一致")]
    [TextArea(3, 12)]
    [SerializeField]
    private string[] voiceSubtitles;


    [Header("Prompt Subtitles")]

    [Tooltip("每段人物音频结束后显示的提示字幕")]
    [TextArea(3, 12)]
    [SerializeField]
    private string[] promptSubtitles;


    [Header("Subtitle UI")]

    [Tooltip("整个字幕背景物体，例如 subtitlebackground")]
    [SerializeField]
    private GameObject subtitleBackground;

    [Tooltip("Scroll View 的 Content 里面的 TextMeshPro 字幕文字")]
    [SerializeField]
    private TMP_Text subtitleText;

    [Tooltip("带有 Scroll Rect 组件的整个 SubtitleScrollView")]
    [SerializeField]
    private ScrollRect subtitleScrollRect;


    [Header("Playback Settings")]

    [Tooltip("进入场景后是否自动播放第一段音频")]
    [SerializeField]
    private bool playFirstClipOnStart = false;

    [Tooltip("音频结束后等待多少秒再显示提示字幕")]
    [Min(0f)]
    [SerializeField]
    private float delayBeforePrompt = 0f;


    [Header("Automatic Scrolling")]

    [Tooltip("人物说话期间，长字幕是否自动向下滚动")]
    [SerializeField]
    private bool autoScrollDuringAudio = true;

    [Tooltip("音频开始后等待多少秒再开始滚动")]
    [Min(0f)]
    [SerializeField]
    private float autoScrollStartDelay = 1f;

    [Tooltip("音频结束前多少秒完成滚动")]
    [Min(0f)]
    [SerializeField]
    private float autoScrollEndPadding = 0.3f;


    private int currentAudioIndex = 0;
    private bool isPlayingAudio = false;
    private bool dialogueFinished = false;
    private bool dialogueStarted = false;

    private Coroutine audioCoroutine;
    private Coroutine scrollResetCoroutine;


    private void Start()
    {
        ValidateSettings();

        // 游戏开始时先隐藏字幕。
        HideSubtitle();

        /*
         * Avatar 选择系统启用后，建议保持关闭，
         * 等 AvatarSelectionManager 设置好声音后再开始。
         */
        if (playFirstClipOnStart)
        {
            StartDialogue();
        }
    }


    private void Update()
    {
        if (!dialogueStarted)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        // 音频播放期间，不允许 Space 跳过当前音频。
        if (isPlayingAudio)
        {
            return;
        }

        // 全部音频播放完成后，不再响应 Space。
        if (dialogueFinished)
        {
            return;
        }

        PlayNextAudio();
    }


    /// <summary>
    /// 根据选中的 Avatar 性别替换整套人物音频。
    /// 例如传入四段男声或四段女声。
    /// </summary>
    public void SetVoiceClips(AudioClip[] newClips)
    {
        if (newClips == null || newClips.Length == 0)
        {
            Debug.LogWarning(
                "AudioSubtitleController: New Voice Clips are empty.",
                this
            );

            return;
        }

        StopCurrentAudio();

        voiceClips = new AudioClip[newClips.Length];

        for (int i = 0; i < newClips.Length; i++)
        {
            voiceClips[i] = newClips[i];
        }

        currentAudioIndex = 0;
        isPlayingAudio = false;
        dialogueFinished = false;
        dialogueStarted = false;

        HideSubtitle();
        ValidateSettings();

        Debug.Log(
            "AudioSubtitleController: Assigned "
            + voiceClips.Length
            + " new Voice Clips.",
            this
        );
    }


    /// <summary>
    /// Avatar 和声音设置完成后，启动对话系统。
    /// 默认不会马上播放，需要玩家按 Space 播放第一段。
    /// </summary>
    public void StartDialogue()
    {
        if (voiceAudioSource == null)
        {
            Debug.LogError(
                "AudioSubtitleController: Voice Audio Source 没有连接。",
                this
            );

            return;
        }

        if (voiceClips == null || voiceClips.Length == 0)
        {
            Debug.LogWarning(
                "AudioSubtitleController: "
                + "Cannot start because Voice Clips are empty.",
                this
            );

            return;
        }

        StopCurrentAudio();

        currentAudioIndex = 0;
        dialogueFinished = false;
        dialogueStarted = true;

        HideSubtitle();

        Debug.Log(
            "AudioSubtitleController: Dialogue is ready. "
            + "Press Space to play the first clip.",
            this
        );
    }


    /// <summary>
    /// Avatar 选择完成后立即播放第一段时，可以调用这个方法。
    /// </summary>
    public void StartDialogueAndPlayFirstClip()
    {
        StartDialogue();

        if (dialogueStarted)
        {
            PlayNextAudio();
        }
    }


    private void PlayNextAudio()
    {
        if (!dialogueStarted)
        {
            Debug.LogWarning(
                "AudioSubtitleController: "
                + "Dialogue has not been started.",
                this
            );

            return;
        }

        if (isPlayingAudio || dialogueFinished)
        {
            return;
        }

        if (voiceAudioSource == null)
        {
            Debug.LogError(
                "AudioSubtitleController: Voice Audio Source 没有连接。",
                this
            );

            return;
        }

        if (voiceClips == null || voiceClips.Length == 0)
        {
            Debug.LogError(
                "AudioSubtitleController: Voice Clips 中没有音频。",
                this
            );

            return;
        }

        if (currentAudioIndex >= voiceClips.Length)
        {
            FinishDialogue();
            return;
        }

        AudioClip currentClip = voiceClips[currentAudioIndex];

        if (currentClip == null)
        {
            Debug.LogWarning(
                "AudioSubtitleController: Voice Clips 的 Element "
                + currentAudioIndex
                + " 没有放入音频，已跳过。",
                this
            );

            currentAudioIndex++;

            if (currentAudioIndex >= voiceClips.Length)
            {
                FinishDialogue();
            }

            return;
        }

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
        }

        audioCoroutine = StartCoroutine(
            PlayAudioRoutine(currentAudioIndex)
        );
    }


    private IEnumerator PlayAudioRoutine(int audioIndex)
    {
        isPlayingAudio = true;

        // 播放人物音频时显示对应字幕。
        string voiceSubtitle = GetSubtitle(
            voiceSubtitles,
            audioIndex
        );

        ShowSubtitle(voiceSubtitle);

        voiceAudioSource.Stop();
        voiceAudioSource.clip = voiceClips[audioIndex];
        voiceAudioSource.time = 0f;
        voiceAudioSource.Play();

        /*
         * 等待一帧，让 AudioSource、TextMeshPro
         * 和 UI 布局系统完成更新。
         */
        yield return null;
        Canvas.ForceUpdateCanvases();

        float clipLength = voiceClips[audioIndex].length;
        float elapsedTime = 0f;

        while (voiceAudioSource != null &&
               voiceAudioSource.isPlaying)
        {
            elapsedTime += Time.deltaTime;

            if (autoScrollDuringAudio &&
                subtitleScrollRect != null)
            {
                UpdateAutomaticScroll(
                    elapsedTime,
                    clipLength
                );
            }

            yield return null;
        }

        if (delayBeforePrompt > 0f)
        {
            yield return new WaitForSeconds(
                delayBeforePrompt
            );
        }

        // 音频结束后显示对应提示字幕。
        string promptSubtitle = GetSubtitle(
            promptSubtitles,
            audioIndex
        );

        currentAudioIndex++;

        if (!string.IsNullOrWhiteSpace(promptSubtitle))
        {
            ShowSubtitle(promptSubtitle);
        }
        else
        {
            HideSubtitle();
        }

        isPlayingAudio = false;
        audioCoroutine = null;

        // 最后一段播放结束。
        if (currentAudioIndex >= voiceClips.Length)
        {
            dialogueFinished = true;

            /*
             * 如果最后一个 Prompt Subtitle 有内容，
             * 它会继续显示。
             * 如果为空，字幕框保持隐藏。
             */
        }
    }


    private void UpdateAutomaticScroll(
        float elapsedTime,
        float clipLength
    )
    {
        if (subtitleScrollRect == null)
        {
            return;
        }

        float scrollDuration =
            clipLength
            - autoScrollStartDelay
            - autoScrollEndPadding;

        if (scrollDuration <= 0f)
        {
            return;
        }

        if (elapsedTime < autoScrollStartDelay)
        {
            subtitleScrollRect.verticalNormalizedPosition = 1f;
            return;
        }

        float scrollProgress = Mathf.Clamp01(
            (elapsedTime - autoScrollStartDelay) /
            scrollDuration
        );

        // 1 是顶部，0 是底部。
        subtitleScrollRect.verticalNormalizedPosition =
            Mathf.Lerp(
                1f,
                0f,
                scrollProgress
            );
    }


    private void FinishDialogue()
    {
        dialogueFinished = true;
        isPlayingAudio = false;

        HideSubtitle();
    }


    private void ShowSubtitle(string message)
    {
        if (subtitleText == null)
        {
            Debug.LogError(
                "AudioSubtitleController: Subtitle Text 没有连接。",
                this
            );

            return;
        }

        bool hasText =
            !string.IsNullOrWhiteSpace(message);

        if (!hasText)
        {
            HideSubtitle();
            return;
        }

        if (subtitleBackground != null)
        {
            subtitleBackground.SetActive(true);
        }

        subtitleText.text = message;

        /*
         * 每次换成新的字幕时，
         * 先自动回到 Scroll View 顶部。
         */
        if (scrollResetCoroutine != null)
        {
            StopCoroutine(scrollResetCoroutine);
        }

        scrollResetCoroutine = StartCoroutine(
            ResetScrollToTop()
        );
    }


    private IEnumerator ResetScrollToTop()
    {
        /*
         * 等待一帧，让 TextMeshPro、
         * Vertical Layout Group 和
         * Content Size Fitter 重新计算高度。
         */
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (subtitleScrollRect != null)
        {
            subtitleScrollRect.StopMovement();

            subtitleScrollRect.verticalNormalizedPosition = 1f;
        }

        scrollResetCoroutine = null;
    }


    private string GetSubtitle(
        string[] subtitleArray,
        int index
    )
    {
        if (subtitleArray == null)
        {
            return string.Empty;
        }

        if (index < 0 ||
            index >= subtitleArray.Length)
        {
            return string.Empty;
        }

        return subtitleArray[index];
    }


    private void ValidateSettings()
    {
        if (voiceClips == null)
        {
            return;
        }

        if (voiceSubtitles == null ||
            voiceSubtitles.Length != voiceClips.Length)
        {
            Debug.LogWarning(
                "AudioSubtitleController: "
                + "Voice Subtitles 的 Size 最好与 Voice Clips 相同。",
                this
            );
        }

        if (promptSubtitles == null ||
            promptSubtitles.Length != voiceClips.Length)
        {
            Debug.LogWarning(
                "AudioSubtitleController: "
                + "Prompt Subtitles 的 Size 最好与 Voice Clips 相同。",
                this
            );
        }
    }


    public void StopCurrentAudio()
    {
        if (voiceAudioSource != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = null;
        }

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        isPlayingAudio = false;
    }


    public void RestartDialogue()
    {
        StopCurrentAudio();

        currentAudioIndex = 0;
        dialogueFinished = false;
        dialogueStarted = true;

        HideSubtitle();
    }


    public void StopDialogue()
    {
        StopCurrentAudio();

        currentAudioIndex = 0;
        dialogueFinished = false;
        dialogueStarted = false;

        HideSubtitle();
    }


    public void HideSubtitle()
    {
        if (scrollResetCoroutine != null)
        {
            StopCoroutine(scrollResetCoroutine);
            scrollResetCoroutine = null;
        }

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
        }

        if (subtitleScrollRect != null)
        {
            subtitleScrollRect.StopMovement();
            subtitleScrollRect.verticalNormalizedPosition = 1f;
        }

        if (subtitleBackground != null)
        {
            subtitleBackground.SetActive(false);
        }
    }


    public bool IsPlayingAudio()
    {
        return isPlayingAudio;
    }


    public bool IsDialogueFinished()
    {
        return dialogueFinished;
    }


    public bool HasDialogueStarted()
    {
        return dialogueStarted;
    }
}