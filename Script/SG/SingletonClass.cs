using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonClass<T>
{

    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = System.Activator.CreateInstance<T>();
            }

            return m_Instance;
        }
    }
}
