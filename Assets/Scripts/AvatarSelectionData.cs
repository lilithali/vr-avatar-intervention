using UnityEngine;

public class AvatarSelectionData : MonoBehaviour
{
    public static AvatarSelectionData Instance;

    public AvatarSelectionManager.AvatarGender selectedGender;
    public AvatarSelectionManager.AvatarEthnicity selectedEthnicity;

    public bool hasSelection = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    public void SaveSelection(
        AvatarSelectionManager.AvatarGender gender,
        AvatarSelectionManager.AvatarEthnicity ethnicity
    )
    {
        selectedGender = gender;
        selectedEthnicity = ethnicity;
        hasSelection = true;
    }


    public void ClearSelection()
    {
        hasSelection = false;
    }
}