using System.Collections;
using UnityEngine;

public class CriticDialogueController : MonoBehaviour
{
    [Header("Voice Audio")]
    public AudioSource voiceAudio;

    [Header("Critic Voice Clips")]
    public AudioClip[] criticClips;

    [Header("Settings")]
    public bool playFirstClipOnStart = false;
    public float delayBeforeNextCritic = 0.5f;

    private int currentClipIndex = 0;
    private bool criticIsSpeaking = false;
    private bool waitingForParticipant = false;
    private bool dialogueFinished = false;

    private Coroutine nextCriticCoroutine;


    private void Start()
    {
        if (voiceAudio == null)
        {
            voiceAudio = GetComponent<AudioSource>();
        }

        if (playFirstClipOnStart)
        {
            StartDialogue();
        }
    }


    private void Update()
    {
        if (voiceAudio == null)
        {
            return;
        }

        // Detect when the Critic has finished speaking.
        if (criticIsSpeaking && !voiceAudio.isPlaying)
        {
            criticIsSpeaking = false;
            waitingForParticipant = true;

            Debug.Log(
                "Critic finished. Waiting for participant response."
            );
        }

        /*
         * Temporary computer test:
         * press Space after the participant finishes speaking.
         */
        if (waitingForParticipant &&
            Input.GetKeyDown(KeyCode.Space))
        {
            ParticipantFinishedSpeaking();
        }
    }


    private void PlayCurrentCriticClip()
    {
        if (voiceAudio == null)
        {
            Debug.LogWarning(
                "CriticDialogueController: Voice Audio is not assigned.",
                this
            );
            return;
        }

        if (criticClips == null ||
            criticClips.Length == 0)
        {
            Debug.LogWarning(
                "CriticDialogueController: Critic Clips are empty.",
                this
            );
            return;
        }

        if (currentClipIndex >= criticClips.Length)
        {
            dialogueFinished = true;
            waitingForParticipant = false;
            criticIsSpeaking = false;

            Debug.Log(
                "Critic dialogue finished.",
                this
            );

            return;
        }

        AudioClip clip = criticClips[currentClipIndex];

        if (clip == null)
        {
            Debug.LogWarning(
                "CriticDialogueController: Critic clip "
                + (currentClipIndex + 1)
                + " is missing.",
                this
            );

            return;
        }

        waitingForParticipant = false;
        criticIsSpeaking = true;

        voiceAudio.Stop();
        voiceAudio.clip = clip;
        voiceAudio.time = 0f;
        voiceAudio.Play();

        Debug.Log(
            "Playing Critic voice part "
            + (currentClipIndex + 1)
            + ".",
            this
        );
    }


    /// <summary>
    /// Replaces the current Critic voice clips.
    /// This will normally be called after the player selects an Avatar.
    /// </summary>
    public void SetCriticClips(AudioClip[] newClips)
    {
        if (newClips == null ||
            newClips.Length == 0)
        {
            Debug.LogWarning(
                "CriticDialogueController: "
                + "New Critic Clips are empty.",
                this
            );

            return;
        }

        StopCurrentDialogue();

        /*
         * Copy the array so this controller has its own
         * reference list.
         */
        criticClips = new AudioClip[newClips.Length];

        for (int i = 0; i < newClips.Length; i++)
        {
            criticClips[i] = newClips[i];
        }

        currentClipIndex = 0;
        criticIsSpeaking = false;
        waitingForParticipant = false;
        dialogueFinished = false;

        Debug.Log(
            "CriticDialogueController: Assigned "
            + criticClips.Length
            + " new voice clips.",
            this
        );
    }


    /// <summary>
    /// Starts the dialogue from the first assigned Critic clip.
    /// Call this after the Avatar and voice have been selected.
    /// </summary>
    public void StartDialogue()
    {
        if (voiceAudio == null)
        {
            voiceAudio = GetComponent<AudioSource>();
        }

        if (voiceAudio == null)
        {
            Debug.LogWarning(
                "CriticDialogueController: "
                + "Voice Audio is not assigned.",
                this
            );

            return;
        }

        if (criticClips == null ||
            criticClips.Length == 0)
        {
            Debug.LogWarning(
                "CriticDialogueController: "
                + "Cannot start because Critic Clips are empty.",
                this
            );

            return;
        }

        StopCurrentDialogue();

        currentClipIndex = 0;
        criticIsSpeaking = false;
        waitingForParticipant = false;
        dialogueFinished = false;

        PlayCurrentCriticClip();
    }


    public void ParticipantFinishedSpeaking()
    {
        if (!waitingForParticipant ||
            criticIsSpeaking ||
            dialogueFinished)
        {
            return;
        }

        waitingForParticipant = false;
        currentClipIndex++;

        if (currentClipIndex >= criticClips.Length)
        {
            dialogueFinished = true;

            Debug.Log(
                "Critic dialogue finished.",
                this
            );

            return;
        }

        if (nextCriticCoroutine != null)
        {
            StopCoroutine(nextCriticCoroutine);
        }

        nextCriticCoroutine = StartCoroutine(
            PlayNextCriticAfterDelay()
        );
    }


    private IEnumerator PlayNextCriticAfterDelay()
    {
        yield return new WaitForSeconds(
            delayBeforeNextCritic
        );

        nextCriticCoroutine = null;
        PlayCurrentCriticClip();
    }


    /// <summary>
    /// Stops the current audio and resets the runtime state.
    /// </summary>
    public void StopCurrentDialogue()
    {
        if (nextCriticCoroutine != null)
        {
            StopCoroutine(nextCriticCoroutine);
            nextCriticCoroutine = null;
        }

        StopAllCoroutines();

        if (voiceAudio != null)
        {
            voiceAudio.Stop();
            voiceAudio.clip = null;
        }

        criticIsSpeaking = false;
        waitingForParticipant = false;
        dialogueFinished = false;
    }


    /// <summary>
    /// Restarts the currently assigned voice set from the beginning.
    /// </summary>
    public void RestartDialogue()
    {
        StartDialogue();
    }


    public bool IsCriticSpeaking()
    {
        return criticIsSpeaking;
    }


    public bool IsWaitingForParticipant()
    {
        return waitingForParticipant;
    }


    public bool IsDialogueFinished()
    {
        return dialogueFinished;
    }
}