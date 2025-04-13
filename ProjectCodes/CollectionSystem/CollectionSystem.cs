using UnityEngine;
using System.Collections.Generic;
using YanGameFrameWork.CoreCodes;
using YanGameFrameWork.AudioSystem;

namespace YanGameFrameWork.CollectionSystem
{
    public interface ICollectionItem
    {
        string Id { get; }           // 唯一标识符
        string Name { get; }         // 显示名称
        string Description { get; }  // 描述
        Sprite Icon { get; }         // 图标
        bool IsUnlocked { get; }     // 是否已解锁
        void Unlock();               // 解锁方法
    }

    public class CollectionSystem : Singleton<CollectionSystem>
    {
        private Dictionary<string, Dictionary<string, ICollectionItem>> _collections = new Dictionary<string, Dictionary<string, ICollectionItem>>();

        // 添加新的图鉴
        public void AddCollection(string collectionId)
        {
            if (!_collections.ContainsKey(collectionId))
            {
                _collections[collectionId] = new Dictionary<string, ICollectionItem>();
            }
        }

        // 添加图鉴项
        public void AddItem(string collectionId, ICollectionItem item)
        {
            if (!_collections.ContainsKey(collectionId))
            {
                AddCollection(collectionId);
            }
            _collections[collectionId][item.Id] = item;
        }

        public void RemoveCollection(string collectionId)
        {
            // SoundController.Instance.PlayOpenPanel();
        }

        // 解锁图鉴项
        public void UnlockItem(string collectionId, string itemId)
        {
            if (_collections.TryGetValue(collectionId, out var collection))
            {
                if (collection.TryGetValue(itemId, out var item))
                {
                    item.Unlock();
                }
            }
        }

        // 获取图鉴完成率
        public float GetCompletionRate(string collectionId)
        {
            if (!_collections.TryGetValue(collectionId, out var collection))
            {
                return 0f;
            }

            if (collection.Count == 0)
            {
                return 0f;
            }

            int unlockedCount = 0;
            foreach (var item in collection.Values)
            {
                if (item.IsUnlocked)
                {
                    unlockedCount++;
                }
            }

            return (float)unlockedCount / collection.Count;
        }

        // 获取指定图鉴的所有物品
        public Dictionary<string, ICollectionItem> GetCollectionItems(string collectionId)
        {
            return _collections.TryGetValue(collectionId, out var collection) ? collection : new Dictionary<string, ICollectionItem>();
        }

        // 获取指定图鉴的已解锁物品数量
        public int GetUnlockedCount(string collectionId)
        {
            if (!_collections.TryGetValue(collectionId, out var collection))
            {
                return 0;
            }

            int count = 0;
            foreach (var item in collection.Values)
            {
                if (item.IsUnlocked)
                {
                    count++;
                }
            }
            return count;
        }

        // 获取指定图鉴的总物品数量
        public int GetTotalCount(string collectionId)
        {
            return _collections.TryGetValue(collectionId, out var collection) ? collection.Count : 0;
        }
    }
}
