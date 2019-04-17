
using UnityEngine;

public class NameGenerator : MonoBehaviour
{

    public static string[] firstNames = { "Robert", "Joeseph" , "John"};
    public static string[] lastNames = {"Clark", "Smith", "Doe" };



    public static int GetRandomFirstName()
    {
        return Random.Range(0, firstNames.Length);
    }
    public static int GetRandomLastName()
    {
        return Random.Range(0, lastNames.Length);
    }

 }
