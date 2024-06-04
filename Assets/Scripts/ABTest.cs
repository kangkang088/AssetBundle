using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ABTest : MonoBehaviour {
    public Image image;
    void Start() {
        ABMgr.GetInstance().LoadResAsync<GameObject>("model","Cube",(obj)=> {
            obj.transform.position = Vector3.up; 
        });

        ////1.加载AB包
        //AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/model");


        ////AssetBundle ab2 = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/head");

        ////利用主包，获取依赖信息
        ////1.加载主包
        //AssetBundle abMain = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/PC");
        ////2.加载主包中的固定文件
        //AssetBundleManifest abManifest = abMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ////3.从固定文件中得到想要知道的AB包的依赖信息
        //string[] strs = abManifest.GetAllDependencies("model");
        //for(int i = 0;i < strs.Length;i++) {
        //    Debug.Log(strs[i]);
        //    AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + strs[i]);
        //}


        ////2.加载AB包资源
        ////GameObject obj = ab.LoadAsset<GameObject>("Cube");
        //GameObject obj = ab.LoadAsset("Cube",typeof(GameObject)) as GameObject;
        //Instantiate(obj);

        //ab.Unload(false);
        //AssetBundle.UnloadAllAssetBundles(false);

        //加载一个圆
        //GameObject obj2 = ab.LoadAsset<GameObject>("Sphere");
        //Instantiate(obj2,Vector3.one,Quaternion.identity);
        //GameObject obj3 = ab.LoadAsset<GameObject>("Sphere");
        //Instantiate(obj3);

        //StartCoroutine(LoadABRes("head","23_11100001"));
    }
    IEnumerator LoadABRes(string ABName,string resName) {
        //第一步 加载AB包
        AssetBundleCreateRequest ab = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + ABName);
        yield return ab;
        //第二部 加载资源
        AssetBundleRequest abr = ab.assetBundle.LoadAssetAsync<Sprite>(resName);
        yield return abr;
        image.sprite = abr.asset as Sprite;
    }
    void Update() {

    }
}
