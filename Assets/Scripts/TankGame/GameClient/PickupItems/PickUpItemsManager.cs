﻿using System;
using Game.Units.Components;
using SHLibrary;
using SHLibrary.Utils;
using UnityEngine;
using System.Collections.Generic;
using SHLibrary.Logging;
using PlayerData;

namespace Game.PickupItems
{
    public class PickUpItemsManager : UnityBehaviourBase
    {
        [SerializeField]
        private PickupItem _itemPrefab;

        private readonly Dictionary<long, PickupItem> _droppedItems = new Dictionary<long, PickupItem>();

        /// <summary>
        /// Calls when unit pick up dropped item(UnitId, ItemInstance).
        /// </summary>
        public event Action<int, PickupItem> UnitPicked;

        public void DropItem(GameItemType itemType, long worldId, Vector3 pos)
        {
            var item = CreateItemInstance(pos);

            item.Initialize(worldId, itemType);
            item.Interacted += OnInteracted;

            _droppedItems.Add(worldId, item);
        }

        internal void DestroyAllItems()
        {
            foreach (var item in _droppedItems.Values)
            {
                item.Interacted -= OnInteracted;

                Destroy(item.gameObject);
            }
            _droppedItems.Clear();
        }

        public void DestroyItem(long worldId)
        {
            PickupItem item;
            if (_droppedItems.TryGetValue(worldId, out item))
            {
                item.Interacted -= OnInteracted;
                _droppedItems.Remove(worldId);

                Destroy(item.gameObject);
            }
            else
            {
                DevLogger.Warning(string.Format("drop item with world id {0}" +
                    " already destroyed", worldId));
            }
        }

        private void OnInteracted(PickupItem item, Collider collider)
        {
            var unit = collider.gameObject.GetComponent<TankHealtComponent>();
            if (unit != null)
            {
                UnitPicked.SafeRaise(unit.UnitID, item);
                DestroyItem(item.WorldId);
            }
        }
        
        private PickupItem CreateItemInstance(Vector3 position)
        {
            return Instantiate(_itemPrefab, position, Quaternion.identity, transform);
        }
    }
}
