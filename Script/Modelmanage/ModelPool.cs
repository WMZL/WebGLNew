using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPool : ISGGamePool<GameObject>
{
    public ModelPool(string name) : base(name) { }

    public override bool ClearObject(GameObject o)
    {
        if (o != null)
        {
            o.SetActive(false);
            GameObject.Destroy(o);
        }

        return true;
    }

    public override GameObject InitilizeObject(GameObject o)
    {
        if (o != null)
        {
            return GameObject.Instantiate(o);
        }
        return null;
    }

    public override bool ResetObject(GameObject o)
    {
        if (o != null)
        {
            ModelInfo m = o.GetComponent<ModelInfo>();
            if (m != null)
            {
                m.ClearData();
            }
            o.SetActive(false);
            return true;
        }
        return false;
    }

    protected override void SetPoolParent(GameObject t)
    {
        t.transform.SetParent(m_PoolParent.transform);
    }
}
