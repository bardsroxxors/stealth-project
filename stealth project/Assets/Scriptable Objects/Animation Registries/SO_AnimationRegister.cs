using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Register", menuName = "Scriptable Object/Animation Register")]

public class SO_AnimationRegister : ScriptableObject
{
    public List<string> names;
    public List<Animation> animations;




    public Animation GetAnimation(string n)
    {
        if (names.Contains(n))
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == n)
                    return animations[i];
            }
        }
        return null;
    }

}

