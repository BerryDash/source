using UnityEngine;

public static class Tools
{
    public static Sprite GetIconForUser(int user)
    {
        if (user == 1)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-1");
        }
        else if (user == 2)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-2");
        }
        else if (user == 4)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-3");
        } else
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_1");
        }
    }
}