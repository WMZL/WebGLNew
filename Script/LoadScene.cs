using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;

public class LoadScene : MonoBehaviour
{
    /// <summary>
    /// 加载进度
    /// </summary>
    private AsyncOperation async;
    private void Awake()
    {
        StartCoroutine(StartLoadScene());
        SceneManager.sceneLoaded += CallBack;
    }

    IEnumerator StartLoadScene()
    {
        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress >= 0.9)
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void CallBack(Scene id, LoadSceneMode lod)
    {
        LoadComplete lc = new LoadComplete();
        lc.cmd = (int)U3DSENGTOJSMSG.SCENELOADED;
        JsonData jd = JsonMapper.ToJson(lc);
        Debug.Log("场景加载成功" + jd.ToString());
        ReadJavaInterface.Hello(jd.ToString());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CallBack;
    }
}
