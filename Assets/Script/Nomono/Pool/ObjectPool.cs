using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>//多个池子组合而成
{
    public ObjectPool() { } //默认无参构造函数

    //资源目录
    public string ResourceDir = "";

    Dictionary<string, SubPool> m_pools = new Dictionary<string, SubPool>();

    //取对象
    public GameObject Spawn(string name) //通过对象名字寻找池子
    {
        if (!m_pools.ContainsKey(name))
            RegisterNewPool(name); //创建新的池子
        return m_pools[name].Spawn(); //在池子里面取对象
    }

    //回收对象
    public void Unspawn(GameObject go)
    {
        SubPool pool = null;
        foreach (SubPool temp_pool in m_pools.Values) //查找该对象所在的池子
            if (temp_pool.ContainsObj(go))
            {
                pool = temp_pool;
                break;
            }
        if (pool != null) pool.Unspawn(go);
    }

    //回收该池子的所有对象
    public void UnspawnAll()
    {
        foreach (SubPool temp_pool in m_pools.Values)
            temp_pool.UnspawnAll();
    }

    //创建新的子池子
    private void RegisterNewPool(string name)
    {
        //得到预设
        string path = "";
        if (string.IsNullOrEmpty(ResourceDir))
            path = name;
        else
            path = ResourceDir + "/" + name;
        //加载预设
        GameObject prefab = Resources.Load<GameObject>(path);
        //创建子对象池
        SubPool pool = new SubPool(prefab);
        m_pools.Add(name, pool);

    }
}