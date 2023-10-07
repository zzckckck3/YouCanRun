using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private Coroutine nowCoroutine = null;
    private IEnumerator coroutineInstance = null;
    public Coroutine GetCoroutine()
    {
        return this.nowCoroutine;
    }
    public IEnumerator GetcoroutineInstance()
    {
        return this.coroutineInstance;
    }
    public void SetCoroutineInstance(IEnumerator coroutineInstance)
    {
        this.coroutineInstance = coroutineInstance;
    }

    public void StartStoredCoroutine()
    {
        if (coroutineInstance != null)
        {
            nowCoroutine = StartCoroutine(coroutineInstance);
        }
    }
    public void NowStopCoroutine()
    {
        if(nowCoroutine != null)
        {
            StopCoroutine(nowCoroutine);
            nowCoroutine = null;
            coroutineInstance = null;
        }
    }
}
