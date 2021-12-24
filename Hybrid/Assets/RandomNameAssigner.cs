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
    public UnityEvent onSetAvatarName;

    public List<string> avatarNameOptionsMale;
    public List<string> avatarNameOptionsFemale;
    public List<string> avatarNameOptionsNonBinary;

    private System.Random rand = new System.Random(System.Guid.NewGuid().GetHashCode());
    private string avatarNameFile = "./../SaveData/avatarname.txt";
    private string avatarNameInternal;


    void Start()
    {
        if (System.IO.File.Exists(avatarNameFile))
        {
            string avatarName = System.IO.File.ReadAllText(avatarNameFile);
            onSetAvatarName.Invoke();

            Debug.Log("TODO: consider updating avatarname savesystem to be server based");
        }
        else
        {
            List<string> names = NameOptionsForGender(Gender.NonBinary);
            avatarNameInternal = names[rand.Next(0, names.Count)];
            System.IO.File.WriteAllText(avatarNameFile, avatarNameInternal);
            onSetAvatarName.Invoke();
        }
    }

    public void AssignNewAvatarName(Gender gender)
    {
        List<string> names = NameOptionsForGender(gender);
        avatarNameInternal = names[rand.Next(0, names.Count)];
        System.IO.File.WriteAllText(avatarNameFile, avatarNameInternal);
        onSetAvatarName.Invoke();
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
