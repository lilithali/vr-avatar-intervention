using UnityEngine;

public class SmileExpressionController : MonoBehaviour
{
    [Header("Mouth Corner Bones")]
    public Transform leftMouthCorner;
    public Transform rightMouthCorner;

    [Header("Cheek Bones")]
    public Transform leftCheek;
    public Transform rightCheek;

    [Header("Upper Lip Bones")]
    public Transform middleUpperLip;
    public Transform leftUpperLip;
    public Transform rightUpperLip;

    [Header("Eye Blink / Eyelid Bones")]
    public Transform leftEyeBlinkTop;
    public Transform rightEyeBlinkTop;
    public Transform leftEyeBlinkBottom;
    public Transform rightEyeBlinkBottom;

    [Header("Eyebrow Bones")]
    public Transform leftInnerEyebrow;
    public Transform rightInnerEyebrow;
    public Transform leftOuterEyebrow;
    public Transform rightOuterEyebrow;

    [Header("Mouth Corner Smile Offsets")]
    public Vector3 leftCornerSmileOffset = new Vector3(0f, 0.0002f, 0f);
    public Vector3 rightCornerSmileOffset = new Vector3(0f, 0.0002f, 0f);

    [Header("Cheek Smile Offsets")]
    public Vector3 leftCheekSmileOffset = new Vector3(-0.002f, -0.002f, 0f);
    public Vector3 rightCheekSmileOffset = new Vector3(-0.002f, -0.002f, 0f);

    [Header("Upper Lip Smile Offsets")]
    public Vector3 middleUpperLipSmileOffset = new Vector3(0f, 0.0005f, 0f);
    public Vector3 leftUpperLipSmileOffset = new Vector3(0f, 0.0003f, 0f);
    public Vector3 rightUpperLipSmileOffset = new Vector3(0f, 0.0003f, 0f);

    [Header("Eye Smile / Squint Offsets")]
    public Vector3 leftEyeTopSmileOffset = new Vector3(0f, -0.001f, 0f);
    public Vector3 rightEyeTopSmileOffset = new Vector3(0f, -0.001f, 0f);
    public Vector3 leftEyeBottomSmileOffset = new Vector3(0f, 0.0005f, 0f);
    public Vector3 rightEyeBottomSmileOffset = new Vector3(0f, 0.0005f, 0f);

    [Header("Eyebrow Smile Offsets")]
    public Vector3 leftInnerEyebrowSmileOffset = new Vector3(0f, 0.0003f, 0f);
    public Vector3 rightInnerEyebrowSmileOffset = new Vector3(0f, 0.0003f, 0f);
    public Vector3 leftOuterEyebrowSmileOffset = new Vector3(0f, 0.0005f, 0f);
    public Vector3 rightOuterEyebrowSmileOffset = new Vector3(0f, 0.0005f, 0f);

    [Header("Smile Control")]
    [Range(0f, 1f)]
    public float smileAmount = 0f;

    public float smoothSpeed = 5f;

    private float currentSmileAmount = 0f;

    private Vector3 leftCornerRest;
    private Vector3 rightCornerRest;

    private Vector3 leftCheekRest;
    private Vector3 rightCheekRest;

    private Vector3 middleUpperLipRest;
    private Vector3 leftUpperLipRest;
    private Vector3 rightUpperLipRest;

    private Vector3 leftEyeTopRest;
    private Vector3 rightEyeTopRest;
    private Vector3 leftEyeBottomRest;
    private Vector3 rightEyeBottomRest;

    private Vector3 leftInnerEyebrowRest;
    private Vector3 rightInnerEyebrowRest;
    private Vector3 leftOuterEyebrowRest;
    private Vector3 rightOuterEyebrowRest;

    void Start()
    {
        if (leftMouthCorner != null)
            leftCornerRest = leftMouthCorner.localPosition;

        if (rightMouthCorner != null)
            rightCornerRest = rightMouthCorner.localPosition;

        if (leftCheek != null)
            leftCheekRest = leftCheek.localPosition;

        if (rightCheek != null)
            rightCheekRest = rightCheek.localPosition;

        if (middleUpperLip != null)
            middleUpperLipRest = middleUpperLip.localPosition;

        if (leftUpperLip != null)
            leftUpperLipRest = leftUpperLip.localPosition;

        if (rightUpperLip != null)
            rightUpperLipRest = rightUpperLip.localPosition;

        if (leftEyeBlinkTop != null)
            leftEyeTopRest = leftEyeBlinkTop.localPosition;

        if (rightEyeBlinkTop != null)
            rightEyeTopRest = rightEyeBlinkTop.localPosition;

        if (leftEyeBlinkBottom != null)
            leftEyeBottomRest = leftEyeBlinkBottom.localPosition;

        if (rightEyeBlinkBottom != null)
            rightEyeBottomRest = rightEyeBlinkBottom.localPosition;

        if (leftInnerEyebrow != null)
            leftInnerEyebrowRest = leftInnerEyebrow.localPosition;

        if (rightInnerEyebrow != null)
            rightInnerEyebrowRest = rightInnerEyebrow.localPosition;

        if (leftOuterEyebrow != null)
            leftOuterEyebrowRest = leftOuterEyebrow.localPosition;

        if (rightOuterEyebrow != null)
            rightOuterEyebrowRest = rightOuterEyebrow.localPosition;
    }

    void Update()
    {
        currentSmileAmount = Mathf.Lerp(
            currentSmileAmount,
            smileAmount,
            Time.deltaTime * smoothSpeed
        );

        ApplySmile(currentSmileAmount);
    }

    void ApplySmile(float amount)
    {
        if (leftMouthCorner != null)
            leftMouthCorner.localPosition =
                leftCornerRest + leftCornerSmileOffset * amount;

        if (rightMouthCorner != null)
            rightMouthCorner.localPosition =
                rightCornerRest + rightCornerSmileOffset * amount;

        if (leftCheek != null)
            leftCheek.localPosition =
                leftCheekRest + leftCheekSmileOffset * amount;

        if (rightCheek != null)
            rightCheek.localPosition =
                rightCheekRest + rightCheekSmileOffset * amount;

        if (middleUpperLip != null)
            middleUpperLip.localPosition =
                middleUpperLipRest + middleUpperLipSmileOffset * amount;

        if (leftUpperLip != null)
            leftUpperLip.localPosition =
                leftUpperLipRest + leftUpperLipSmileOffset * amount;

        if (rightUpperLip != null)
            rightUpperLip.localPosition =
                rightUpperLipRest + rightUpperLipSmileOffset * amount;

        if (leftEyeBlinkTop != null)
            leftEyeBlinkTop.localPosition =
                leftEyeTopRest + leftEyeTopSmileOffset * amount;

        if (rightEyeBlinkTop != null)
            rightEyeBlinkTop.localPosition =
                rightEyeTopRest + rightEyeTopSmileOffset * amount;

        if (leftEyeBlinkBottom != null)
            leftEyeBlinkBottom.localPosition =
                leftEyeBottomRest + leftEyeBottomSmileOffset * amount;

        if (rightEyeBlinkBottom != null)
            rightEyeBlinkBottom.localPosition =
                rightEyeBottomRest + rightEyeBottomSmileOffset * amount;

        if (leftInnerEyebrow != null)
            leftInnerEyebrow.localPosition =
                leftInnerEyebrowRest + leftInnerEyebrowSmileOffset * amount;

        if (rightInnerEyebrow != null)
            rightInnerEyebrow.localPosition =
                rightInnerEyebrowRest + rightInnerEyebrowSmileOffset * amount;

        if (leftOuterEyebrow != null)
            leftOuterEyebrow.localPosition =
                leftOuterEyebrowRest + leftOuterEyebrowSmileOffset * amount;

        if (rightOuterEyebrow != null)
            rightOuterEyebrow.localPosition =
                rightOuterEyebrowRest + rightOuterEyebrowSmileOffset * amount;
    }
}