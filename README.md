# VR Avatar Intervention

A Unity-based VR research prototype exploring avatar-mediated self-dialogue for self-critical thoughts.

The project uses a virtual avatar to externalise a self-critical inner voice and guide participants through a structured dialogue. It investigates whether an interactive virtual character can support psychologically meaningful self-dialogue and self-compassion in an immersive VR environment.

The system was developed as part of a research project and was pilot-tested with nine participants.

---

## Project Overview

Self-critical thoughts can often feel automatic and difficult to distance from. This project explores whether representing this internal critical voice as an external virtual character can help participants interact with it from a different perspective.

The VR experience uses three roles:

- **Critic** — represents the participant's self-critical voice
- **Participant** — responds directly to the critic
- **Guide** — provides structured prompts during the interaction

Rather than presenting the avatar as a passive animated character, the project focuses on making the interaction feel socially responsive through speech-driven facial animation, eye contact, head movement and spatial presence.

---

## Key Features

- VR experience developed and deployed for **Meta Quest**
- Speech-driven avatar lip synchronisation using **uLipSync**
- Custom facial animation using avatar facial bones
- Jaw, upper-lip and mouth-corner movement driven by speech
- Blinking, eyebrow, cheek and subtle head-motion behaviours
- Avatar gaze tracking to maintain eye contact with seated participants
- Structured Critic–Participant–Guide dialogue
- Synchronized speech audio and subtitles
- Pilot study conducted with **9 participants**
- Iterative design improvements based on participant feedback

---

## Demo

### Avatar Interaction Demo

[▶ Watch Demo 1](demo/avatar_demo1.mp4)

[▶ Watch Demo 2](demo/avatar_demo2.mp4)

The videos demonstrate the avatar during the VR interaction, including speech-driven lip synchronisation, facial animation and the virtual environment used in the study.

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

The dialogue is structured around an externalised self-critical voice.

A typical interaction follows this progression:

1. The **Critic** expresses a self-critical thought.
2. The **Guide** helps the participant notice the thought without immediately arguing against it.
3. The **Participant** responds to the critic and explores what the critical voice may be trying to protect them from.
4. The interaction gradually shifts from confrontation towards curiosity and self-compassion.

This structure allows the avatar to function as a virtual social other rather than simply presenting written therapeutic prompts.

---

## Pilot Study

The prototype was evaluated in a pilot study with **nine participants**.

The purpose of the pilot was primarily to evaluate the interaction design and identify usability issues rather than to test clinical effectiveness.

Participant feedback highlighted several areas for improvement, including:

- providing more context before the avatar conversation begins
- increasing eye contact between the avatar and participant
- reducing interruptions from the guide during the main dialogue
- separating guidance from the avatar conversation more clearly
- improving the clarity of questionnaire wording
- exploring more personalised and responsive dialogue in future versions

The feedback was used to guide subsequent iterations of the avatar behaviour and study procedure.

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
