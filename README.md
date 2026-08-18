# VR Avatar Intervention

A Unity-based VR research prototype exploring avatar-mediated self-dialogue for self-critical thoughts.

The project uses a virtual avatar to externalise a self-critical inner voice and guide participants through a structured dialogue. It investigates whether an interactive virtual character can support psychologically meaningful self-dialogue and self-compassion in an immersive VR environment.

The system was developed as part of a research project and was pilot-tested with nine participants.

---
## Project Overview

Self-critical thoughts can often feel automatic and difficult to distance from. This project explores whether representing an internal critical voice as an external virtual character can help participants interact with it from a different perspective.

During the experience, the avatar represents a self-critical voice and speaks directly to the participant. The participant is then encouraged to respond to the avatar, creating a face-to-face dialogue with an externalised representation of their self-critical thoughts.

The technical focus of the project is to make this interaction feel socially responsive through speech-driven facial animation, gaze behaviour, subtle head movement and immersive spatial presence.

---

## Key Features

- VR experience developed and deployed for **Meta Quest**
- Speech-driven avatar lip synchronisation using **uLipSync**
- Custom facial animation using avatar facial bones
- Jaw, upper-lip and mouth-corner movement driven by speech
- Blinking, eyebrow, cheek and subtle head-motion behaviours
- Avatar gaze tracking to maintain eye contact with seated participants
- Synchronized speech audio and subtitles
- Pilot study conducted with **9 participants**
---

## Demo

### Demo 1 — Unity Avatar Animation

[▶ Watch Demo 1](demo/avatar_demo1.mp4)

Recorded directly in Unity, this demo shows the avatar animation system, including speech-driven lip synchronisation, facial movement, blinking and subtle head motion.

### Demo 2 — VR Experience

[▶ Watch Demo 2](demo/avatar_demo2.mp4)

Recorded from the VR experience, this demo shows how the avatar appears to the participant inside the immersive environment during the study.
---

## My Contributions

My main responsibility was the design and implementation of the VR avatar and its interaction behaviour.

### Avatar Lip Synchronisation

I integrated **uLipSync** into the Unity avatar system and adapted its output to control the available facial bones.

Instead of relying only on jaw rotation, I mapped speech information to several facial components, including:

- jaw rotation
- centre, left and right upper-lip positions
- left and right mouth corners
- cheeks
- eyebrows
- blinking

The avatar model did not provide a lower-lip bone, so the lip-sync system had to be adapted around the available facial rig.

I also tuned speech-volume thresholds and smoothing parameters to reduce excessive mouth movement and prevent the avatar from appearing to continuously open and close its mouth during speech.

---

### Facial Expression and Head Motion

Lip movement alone made the avatar appear visually stiff, so I extended the animation system with additional facial behaviour.

I implemented:

- natural blinking
- eyebrow movement
- cheek movement
- subtle head and neck rotation
- small expression changes during speech

Head movement was deliberately kept small because larger procedural rotations quickly appeared unnatural in VR.

The final implementation combines speech-driven mouth motion with lightweight procedural facial animation to create a more responsive character without relying on a fully pre-authored animation sequence.

---

### Gaze and Eye Contact

Participants use the system while seated, which means the headset position changes depending on each participant's height and sitting posture.

To avoid fixing the avatar's gaze at a single world-space position, the avatar uses the VR headset position as its gaze target.

This allows the avatar to dynamically adjust its gaze towards the participant's head position and maintain more consistent eye contact during the interaction.

---

## Interaction Design

The avatar represents an externalised version of the participant's self-critical voice.

During the interaction, the avatar expresses self-critical thoughts directly to the participant. The participant then responds verbally to the avatar, allowing the internal critical voice to become something that can be observed and addressed as a separate conversational partner.

As the dialogue progresses, the participant is encouraged to explore the intention behind the critical voice and respond from a more reflective and compassionate perspective.

Guidance during the study was provided separately by the experimenter and was not part of the avatar system itself.
---

## Pilot Study

The prototype was evaluated in a pilot study with **nine participants**.

The pilot was used to assess the usability and presentation of the VR experience and to identify issues in the study procedure and avatar interaction.

Participant feedback highlighted several areas for future improvement, including:

- providing clearer context before the avatar conversation begins
- increasing eye contact between the avatar and participant
- reducing interruptions during the main conversation
- separating experimenter guidance more clearly from the avatar dialogue
- improving the clarity of questionnaire wording
- exploring more personalised dialogue in future versions

The pilot primarily served as a feasibility and design evaluation rather than an assessment of clinical effectiveness.

---

## Technical Implementation

The avatar system combines several components inside Unity:

```text
Speech Audio
     │
     ▼
   uLipSync
     │
     ▼
Phoneme / Volume Analysis
     │
     ├── Jaw Rotation
     ├── Upper Lip Movement
     ├── Mouth Corner Movement
     └── Facial Motion
             │
             ├── Blink
             ├── Eyebrow
             ├── Cheek
             └── Head Motion
