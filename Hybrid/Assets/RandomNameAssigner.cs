using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Gender
{
    Male,
    Female,
    NonBinary
}

public class RandomNameAssigner : MonoBehaviour
{
    public StringReference avatarName;
    public UnityEvent onSetAvatarName;

    public List<string> avatarNameOptionsMale;
    public List<string> avatarNameOptionsFemale;
    public List<string> avatarNameOptionsNonBinary;

    private System.Random rand = new System.Random(System.Guid.NewGuid().GetHashCode());
    private string avatarNameFile = "./../SaveData/avatarname.txt";


    void Start()
    {
        if (System.IO.File.Exists(avatarNameFile))
        {
            avatarName.Value = System.IO.File.ReadAllText(avatarNameFile);
            onSetAvatarName.Invoke();

            Debug.Log("TODO: consider updating avatarname savesystem to be server based");
        }
        else
        {
            UpdateAvatarNameFromList(NameOptionsForGender(Gender.NonBinary));
        }
    }

    public void AssignNewAvatarName(Gender gender)
    {
        UpdateAvatarNameFromList(NameOptionsForGender(gender));
    }

    private void UpdateAvatarNameFromList(List<string> names)
    {
        avatarName.Value = names[rand.Next(0, names.Count)];
        onSetAvatarName.Invoke();

        System.IO.File.WriteAllText(avatarNameFile, avatarName.Value);
    }

    private List<string> NameOptionsForGender(Gender gender)
    {
        switch (gender)
        {
            case Gender.Male:
                return avatarNameOptionsMale;
            case Gender.Female:
                return avatarNameOptionsFemale;
            case Gender.NonBinary:
                return avatarNameOptionsNonBinary;
            default:
                Debug.LogWarning("Something wrong with the Gender enum.");
                return avatarNameOptionsNonBinary;
        }
    }
}
