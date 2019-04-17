using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{

    //public Enums.Mood currentMood;
    public int firstNameIndex = -1, lastNameIndex = -1;
    public virtual void CopyFrom(Person p)
    {
        firstNameIndex = p.firstNameIndex;
        lastNameIndex = p.lastNameIndex;
    }

}
