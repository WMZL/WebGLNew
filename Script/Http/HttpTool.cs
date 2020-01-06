
//
//  HttpTool.cs
//  EndlessRunner
//
//  Created by jiabl on 09/06/2017.
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HttpRequestEventHandle(string __request);

/// <summary>
/// Http tool.
/// 当需要请求http的时候调用Request方法
/// 例子：
/// HttpTool.Instance.Request("www.baidu.com",new HttpRequestEventHandle(myfunc))
/// void myfunc(string str) {do something]
/// </summary>
public class HttpTool : MonoBehaviour
{
    /// <summary>
    /// http请求
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="fa">Fa.</param>
    /// 
    public void Request(string __url, HttpRequestEventHandle __eventHandle)
    {
        StartCoroutine(httpRequest(__url, __eventHandle));
    }
    /// <summary>
    /// Request the specified __url, __eventHandleForComplete and __eventHandleForError.
    /// </summary>
    /// <param name="__url">URL.</param>
    /// <param name="__eventHandleForComplete">Event handle for complete.</param>
    /// <param name="__eventHandleForError">Event handle for error.</param>
    public void Request(string __url, HttpRequestEventHandle __eventHandleForComplete, HttpRequestEventHandle __eventHandleForError)
    {
        StartCoroutine(httpRequest(__url, __eventHandleForComplete, __eventHandleForError));
    }


    /// <summary>
    /// Https the request.
    /// </summary>
    /// <returns>The request.</returns>
    /// <param name="url">URL.</param>
    public IEnumerator httpRequest(string __url, HttpRequestEventHandle __eventHandle)
    {
        WWW __request = new WWW(__url);
        yield return __request;

        if (string.IsNullOrEmpty(__request.error))
        {
            onHttpRequestCompleted(__request.text, __eventHandle);
        }
        else
        {
            Debug.Log("<====== http request error! =======> info: url=>" + __url + " request error: " + __request.error);
        }
    }
    /// <summary>
    /// Https the request.
    /// </summary>
    /// <returns>The request.</returns>
    /// <param name="__url">URL.</param>
    /// <param name="__eventHandleForComplete">Event handle for complete.</param>
    /// <param name="__eventHandleForError">Event handle for error.</param>
    public IEnumerator httpRequest(string __url, HttpRequestEventHandle __eventHandleForComplete, HttpRequestEventHandle __eventHandleForError)
    {
        WWW __request = new WWW(__url);
        yield return __request;

        if (string.IsNullOrEmpty(__request.error))
        {
            onHttpRequestCompleted(__request.text, __eventHandleForComplete);
        }
        else
        {
            onHttpRequestFailed(__request.text, __eventHandleForError);
            Debug.Log("<====== http request error! =======> info: url=>" + __url + " request error: " + __request.error);
        }
    }

    /// <summary>
    /// http请求后的回调方法.
    /// </summary>
    /// <param name="__requestText">Request text.</param>
    /// <param name="__eventHandle">Event handle.</param>
    private void onHttpRequestCompleted(string __requestText, HttpRequestEventHandle __eventHandle)
    {
        __eventHandle(__requestText);
    }
    /// <summary>
    /// http请求失败的回调方法
    /// </summary>
    /// <param name="__requestText">Request text.</param>
    /// <param name="__eventHandle">Event handle.</param>
    private void onHttpRequestFailed(string __requestText, HttpRequestEventHandle __eventHandle)
    {
        __eventHandle(__requestText);
    }
}