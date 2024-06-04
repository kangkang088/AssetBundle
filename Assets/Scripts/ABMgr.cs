using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABMgr : SingletonAutoMono<ABMgr> {
    //AB包管理器目的：让外部更方便的进行资源加载
    //主包
    private AssetBundle mainAB = null;
    //依赖包信息
    private AssetBundleManifest manifest = null;
    //存储加载过的AB包
    private Dictionary<string,AssetBundle> abDic = new Dictionary<string,AssetBundle>();

    //AB包存放路径
    private string PathUrl {
        get {
            return Application.streamingAssetsPath + "/";
        }
    }
    //主包名
    private string MainABName {
        get {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }
    //加载AB包和依赖包
    public void LoadDependencies(string abName) {
        //加载AB包
        //获取依赖包相关信息
        AssetBundle ab = null;
        if(mainAB == null) {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        string[] strs = manifest.GetAllDependencies(abName);
        for(int i = 0;i < strs.Length;i++) {
            if(!abDic.ContainsKey(strs[i])) {
                abDic.Add(strs[i],AssetBundle.LoadFromFile(PathUrl + strs[i]));
            }
        }
        //要使用的AB包
        if(!abDic.ContainsKey(abName)) {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName,ab);
        }
    }
    //同步加载
    public Object LoadRes(string abName,string resName) {
        LoadDependencies(abName);
        //加载资源
        Object obj = abDic[abName].LoadAsset(resName);
        if(obj is GameObject)
            return Instantiate(obj);
        return obj;
    }
    public Object LoadRes(string abName,string resName,System.Type type) {
        LoadDependencies(abName);
        //加载资源
        Object obj = abDic[abName].LoadAsset(resName,type);
        if(obj is GameObject)
            return Instantiate(obj);
        return obj;
    }
    public T LoadRes<T>(string abName,string resName) where T : Object {
        LoadDependencies(abName);
        //加载资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        if(obj is GameObject)
            return Instantiate(obj);
        return obj;
    }
    //异步加载
    //AB包没有使用异步加载，只是AB包中加载资源时使用异步加载
    public void LoadResAsync(string abName,string resName,UnityAction<Object> callback) {
        StartCoroutine(ReadllyLoadResAsync(abName,resName,callback));
    }
    private IEnumerator ReadllyLoadResAsync(string abName,string resName,UnityAction<Object> callback) {
        LoadDependencies(abName);
        //加载资源
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;
        if(abr.asset is GameObject) {
            callback(Instantiate(abr.asset));
        }
        else {
            callback(abr.asset);
        }
    }
    public void LoadResAsync(string abName,string resName,System.Type type,UnityAction<Object> callback) {
        StartCoroutine(ReadllyLoadResAsync(abName,resName,type,callback));
    }
    private IEnumerator ReadllyLoadResAsync(string abName,string resName,System.Type type,UnityAction<Object> callback) {
        LoadDependencies(abName);
        //加载资源
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName,type);
        yield return abr;
        if(abr.asset is GameObject) {
            callback(Instantiate(abr.asset));
        }
        else {
            callback(abr.asset);
        }
    }
    public void LoadResAsync<T>(string abName,string resName,UnityAction<T> callback) where T:Object {
        StartCoroutine(ReadllyLoadResAsync<T>(abName,resName,callback));
    }
    private IEnumerator ReadllyLoadResAsync<T>(string abName,string resName,UnityAction<T> callback) where T:Object {
        LoadDependencies(abName);
        //加载资源
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        if(abr.asset is GameObject) {
            callback(Instantiate(abr.asset) as T);
        }
        else {
            callback(abr.asset as T);
        }
    }
    //单个包卸载
    public void UnLoad(string abName) {
        if(abDic.ContainsKey(abName)) {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }
    //所有包卸载
    public void ClearAB() {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
