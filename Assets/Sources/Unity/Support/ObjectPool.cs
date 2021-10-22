using System.Collections.Generic;
using Sources.Unity.Support.Source.Unity.Support;
using UnityEngine;

namespace Sources.Unity.Support {
    public sealed class ObjectPool : SingletonMonoBehaviour<ObjectPool> {
        public enum StartupPoolMode {
            Awake,
            Start
        }

        [System.Serializable]
        public class StartupPool {
            public int Size;
            public GameObject Prefab;
        }

        List<GameObject> tempList = new List<GameObject>();

        Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

        public StartupPoolMode startupPoolMode;
        public StartupPool[] StartupPools;

        private bool startupPoolsCreated;

        private void Awake() {
            if (startupPoolMode == StartupPoolMode.Awake)
                CreateStartupPools();
        }

        private void Start() {
            if (startupPoolMode == StartupPoolMode.Start)
                CreateStartupPools();
        }

        private void CreateStartupPools() {
            if (!Instance.startupPoolsCreated) {
                Instance.startupPoolsCreated = true;
                StartupPool[] pools = Instance.StartupPools;
                if (pools == null || pools.Length <= 0) {
                    return;
                }

                foreach (StartupPool t in pools) {
                    CreatePool(t.Prefab, t.Size);
                }
            }
        }

        public void CreatePool(GameObject prefab, int initialPoolSize) {
            if (prefab != null && !Instance.pooledObjects.ContainsKey(prefab)) {
                var list = new List<GameObject>();
                Instance.pooledObjects.Add(prefab, list);

                if (initialPoolSize > 0) {
                    bool active = prefab.activeSelf;
                    prefab.SetActive(false);
                    Transform parent = Instance.transform;
                    while (list.Count < initialPoolSize) {
                        var obj = Instantiate(prefab, parent, false);
                        list.Add(obj);
                    }

                    prefab.SetActive(active);
                }
            }
        }

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
            List<GameObject> list;
            Transform trans;
            GameObject obj;
            if (Instance.pooledObjects.TryGetValue(prefab, out list)) {
                obj = null;
                if (list.Count > 0) {
                    while (obj == null && list.Count > 0) {
                        obj = list[0];
                        list.RemoveAt(0);
                    }

                    if (obj != null) {
                        trans = obj.transform;
                        trans.SetParent(parent);
                        trans.localPosition = position;
                        trans.localRotation = rotation;
                        obj.SetActive(true);
                        Instance.spawnedObjects.Add(obj, prefab);
                        return obj;
                    }
                }

                obj = Instantiate(prefab);
                trans = obj.transform;
                trans.parent = parent;
                trans.localPosition = position;
                trans.localRotation = rotation;
                Instance.spawnedObjects.Add(obj, prefab);
                return obj;
            }

            obj = Instantiate(prefab);
            trans = obj.GetComponent<Transform>();
            trans.SetParent(parent, false);
            trans.localPosition = position;
            trans.localRotation = rotation;
            return obj;
        }

        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position) {
            return Spawn(prefab, parent, position, Quaternion.identity);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
            return Spawn(prefab, null, position, rotation);
        }

        public GameObject Spawn(GameObject prefab, Transform parent) {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position) {
            return Spawn(prefab, null, position, Quaternion.identity);
        }

        public GameObject Spawn(GameObject prefab) {
            return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
        }

        public void Recycle(GameObject obj) {
            GameObject prefab;
            if (Instance.spawnedObjects.TryGetValue(obj, out prefab)) {
                Recycle(obj, prefab);
            } else {
                Destroy(obj);
            }
        }

        void Recycle(GameObject obj, GameObject prefab) {
            Instance.pooledObjects[prefab].Add(obj);
            Instance.spawnedObjects.Remove(obj);
            obj.transform.SetParent(Instance.transform);
            obj.SetActive(false);
        }

        public void RecycleAll(GameObject prefab) {
            foreach (var item in Instance.spawnedObjects) {
                if (item.Value == prefab) {
                    tempList.Add(item.Key);
                }
            }

            for (int i = 0; i < tempList.Count; ++i) {
                Recycle(tempList[i]);
            }

            tempList.Clear();
        }

        public void RecycleAll() {
            tempList.AddRange(Instance.spawnedObjects.Keys);
            for (int i = 0; i < tempList.Count; ++i) {
                Recycle(tempList[i]);
            }

            tempList.Clear();
        }

        public bool IsSpawned(GameObject obj) {
            return Instance.spawnedObjects.ContainsKey(obj);
        }

        public int CountPooled<T>(T prefab) where T : Component {
            return CountPooled(prefab.gameObject);
        }

        public int CountPooled(GameObject prefab) {
            List<GameObject> list;
            if (Instance.pooledObjects.TryGetValue(prefab, out list)) {
                return list.Count;
            }

            return 0;
        }

        public int CountSpawned(GameObject prefab) {
            int count = 0;
            foreach (var instancePrefab in Instance.spawnedObjects.Values) {
                if (prefab == instancePrefab) {
                    ++count;
                }
            }

            return count;
        }

        public int CountAllPooled() {
            int count = 0;
            foreach (var list in Instance.pooledObjects.Values) {
                count += list.Count;
            }

            return count;
        }

        public List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList) {
            if (list == null) {
                list = new List<GameObject>();
            }

            if (!appendList) {
                list.Clear();
            }

            List<GameObject> pooled;
            if (Instance.pooledObjects.TryGetValue(prefab, out pooled)) {
                list.AddRange(pooled);
            }

            return list;
        }

        public List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList) {
            if (list == null) {
                list = new List<GameObject>();
            }

            if (!appendList) {
                list.Clear();
            }

            foreach (var item in Instance.spawnedObjects) {
                if (item.Value == prefab) {
                    list.Add(item.Key);
                }
            }

            return list;
        }

        public void DestroyPooled(GameObject prefab) {
            List<GameObject> pooled;
            if (Instance.pooledObjects.TryGetValue(prefab, out pooled)) {
                for (int i = 0; i < pooled.Count; ++i) {
                    GameObject.Destroy(pooled[i]);
                }

                pooled.Clear();
            }
        }

        public void DestroyAll(GameObject prefab) {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }
    }
}
