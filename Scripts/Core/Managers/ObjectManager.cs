using Scripts.Entities.Players;
using Scripts.Network;
using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Managers
{
    [DefaultExecutionOrder(-10)]
    public class ObjectManager : MonoBehaviour
    {
        private static ObjectManager _instance;
        public static ObjectManager Instance => _instance;
        [SerializeField] private SerializableDictionary<ObjectType, GameObject> _prefabs;
        private Dictionary<int, GameObject> _objects = new();
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
        public GameObject CreateObject<T>(int index, ObjectType type, Vector3 position, Quaternion quat, out T compo) where T : MonoBehaviour
        {
            GameObject obj = Instantiate(_prefabs.Get(type), position, quat);
            _objects.Add(index, obj);
            compo = obj.GetComponent<T>();
            return compo.gameObject;
        }
        public T CreateObject<T>(int index,GameObject prefab,Vector3 position,Quaternion quat)
        {
            GameObject obj = Instantiate(prefab, position, quat);
            _objects.Add(index,obj);
            return obj.GetComponent<T>();
        }
        public T CreateObject<T>(int index, ObjectType type)
        {
            GameObject obj = Instantiate(_prefabs.Get(type));
            _objects.Add(index, obj);
            return obj.GetComponent<T>();
        }
        public T GetObejct<T>(int index)
        {
            GameObject obj = _objects.GetValueOrDefault(index);
            return obj.GetComponent<T>();
        }
        public T GetObjctOrCreate<T>(int index,ObjectType type)
        {
            GameObject obj = _objects.GetValueOrDefault(index);
            if (obj == null)
                return CreateObject<T>(index, type);
            else
                return obj.GetComponent<T>();
        }
        public void RemoveObject(int index)
        {
            Destroy(_objects[index]);
            _objects.Remove(index);
        }
        public void DestroyAll()
        {
            foreach (var item in _objects)
                Destroy(item.Value.gameObject);
            _objects.Clear();
        }
    }
}
