using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Pooler : Singleton<Pooler>
{
    class PooledObject
    {
        public GameObject[] objects { get; set; }
        public int currIdx { get; set; }
    }

    private Dictionary<string, PooledObject> _pooledRefMapping;

    private void Awake()
    {
        InitializeSingleton();
        _pooledRefMapping = new Dictionary<string, PooledObject>();
    }
    /// <summary>
    /// Store and clone a particular prefab to pool
    /// </summary>
    /// <param name="pooledRef"></param>
    /// <param name="count"></param>
    public void PoolGameObject(GameObject pooledRef, int count)
    {
        if (pooledRef == null) 
        {
            Debug.LogError("Pooler cannot instantiate object without Projectile component.");
            return;
        }
        _pooledRefMapping[pooledRef.name] = new PooledObject();
        _pooledRefMapping[pooledRef.name].objects = new GameObject[count];
        _pooledRefMapping[pooledRef.name].currIdx = 0;

        // Pool GameObjects
        for (int i = 0; i < count; i++)
        {
            _pooledRefMapping[pooledRef.name].objects[i] = Instantiate(pooledRef);
            _pooledRefMapping[pooledRef.name].objects[i].SetActive(false);
            _pooledRefMapping[pooledRef.name].objects[i].transform.SetParent(transform, true);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject Pool(GameObject go)
    {
        return Pool(go.name);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject Pool(string goName)
    {
        if (!_pooledRefMapping.ContainsKey(goName))
        {
            return null;
        }
        int idx = _pooledRefMapping[goName].currIdx;
        Debug.Log(_pooledRefMapping[goName].objects.Length);
        _pooledRefMapping[goName].currIdx = (idx + 1) % _pooledRefMapping[goName].objects.Length;
        _pooledRefMapping[goName].objects[idx].transform.SetParent(transform, true);
        return _pooledRefMapping[goName].objects[idx];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="globalPosition"></param>
    /// <returns></returns>
    public GameObject Spawn(GameObject go, Vector3 globalPosition)
    {
        go = Pool(go);
        if (go == null)
        {
            return null;
        }
        go.transform.position = globalPosition;
        go.SetActive(true);
        return go;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject NextObjectToPool(GameObject go)
    {
        return NextObjectToPool(go.name);
    }
    public GameObject NextObjectToPool(string goName)
    {
        return _pooledRefMapping[goName].objects[_pooledRefMapping[goName].currIdx];
    }
    public bool HasObjectInPool(GameObject go)
    {
        return _pooledRefMapping.ContainsKey(go.name);
    }
    public bool HasObjectInPool(string goName)
    {
        return _pooledRefMapping.ContainsKey(goName);
    }
}
