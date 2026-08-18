using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarSelectionUIController : MonoBehaviour
{
    [Header("VR Scene")]

    [Tooltip("选择完成后进入的 VR Scene 名称")]
    [SerializeField]
    private string vrSceneName = "YourVRSceneName";


    public void SelectMaleWhite()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Male,
            AvatarSelectionManager.AvatarEthnicity.White
        );
    }


    public void SelectMaleAsian()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Male,
            AvatarSelectionManager.AvatarEthnicity.Asian
        );
    }


    public void SelectMaleBlack()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Male,
            AvatarSelectionManager.AvatarEthnicity.Black
        );
    }


    public void SelectMaleBrown()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Male,
            AvatarSelectionManager.AvatarEthnicity.Brown
        );
    }


    public void SelectFemaleWhite()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Female,
            AvatarSelectionManager.AvatarEthnicity.White
        );
    }


    public void SelectFemaleAsian()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Female,
            AvatarSelectionManager.AvatarEthnicity.Asian
        );
    }


    public void SelectFemaleBlack()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Female,
            AvatarSelectionManager.AvatarEthnicity.Black
        );
    }


    public void SelectFemaleBrown()
    {
        SaveSelectionAndLoadScene(
            AvatarSelectionManager.AvatarGender.Female,
            AvatarSelectionManager.AvatarEthnicity.Brown
        );
    }


    private void SaveSelectionAndLoadScene(
        AvatarSelectionManager.AvatarGender gender,
        AvatarSelectionManager.AvatarEthnicity ethnicity
    )
    {
        if (AvatarSelectionData.Instance == null)
        {
            Debug.LogError(
                "AvatarSelectionUIController: "
                + "场景中没有 AvatarSelectionData。",
                this
            );

            return;
        }

        AvatarSelectionData.Instance.SaveSelection(
            gender,
            ethnicity
        );

        SceneManager.LoadScene(vrSceneName);
    }
}