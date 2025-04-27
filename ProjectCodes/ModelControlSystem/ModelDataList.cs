/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-27
 * Description: 本类是自己封装了一层dictionary,用于在Inspector中显示model的数据
 * 不过要想显示具体的数据，还需要model的具体类的各个字段加上[SerializeField]
 *
 ****************************************************************************/



using System;
using System.Collections.Generic;


namespace YanGameFrameWork.ModelControlSystem
{

    /// <summary>
    /// 用于在Inspector中显示和编辑模块
    /// </summary>
    [System.Serializable]
    public class ModelDataList : IDictionary<Type, YanModelBase>
    {
        private Dictionary<Type, YanModelBase> _modules = new Dictionary<Type, YanModelBase>();

        public YanModelBase this[Type key] { get => _modules[key]; set => _modules[key] = value; }

        public ICollection<Type> Keys => _modules.Keys;

        public ICollection<YanModelBase> Values => _modules.Values;

        public int Count => _modules.Count;

        public bool IsReadOnly => false;

        public void Add(Type key, YanModelBase value)
        {
            _modules.Add(key, value);
        }

        public bool ContainsKey(Type key)
        {
            return _modules.ContainsKey(key);
        }

        public bool Remove(Type key)
        {
            return _modules.Remove(key);
        }

        public bool TryGetValue(Type key, out YanModelBase value)
        {
            return _modules.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<Type, YanModelBase> item)
        {
            _modules.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _modules.Clear();
        }

        public bool Contains(KeyValuePair<Type, YanModelBase> item)
        {
            return _modules.ContainsKey(item.Key) && _modules[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<Type, YanModelBase>[] array, int arrayIndex)
        {
            ((IDictionary<Type, YanModelBase>)_modules).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<Type, YanModelBase> item)
        {
            return _modules.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<Type, YanModelBase>> GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _modules.GetEnumerator();
        }
    }
}