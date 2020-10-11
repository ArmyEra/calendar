using System;
using System.Collections.Generic;
using Calendar.DatePanel.Utils;
using Core;
using Extensions;
using UnityEngine;
using Utils;
using EventType = Core.EventType;

namespace Calendar.DatePanel
{
    public class DateContainer : MonoBehaviour
    {
        private static bool _initializedOnStart;
        private static Vector2 _itemSpace = Vector2.zero;
        private static Vector2 _itemSize = Vector2.zero;
        
        [Header("Настройки сетки")]
        [SerializeField] private Vector2 gridMargin;
        [SerializeField] private Vector2 cellOffset;
        [SerializeField] private GameObject dateItemTemplate;

        private RectTransform _rectTransform;
        private readonly List<GameObject> _dayObjects = new List<GameObject>();
        
        /// <summary>
        /// Инициализирует кнопки-даты 
        /// </summary>
        public void Initialize(in DateTime monthDate)
        {
            InitializeOnStart();
            monthDate.GetBoundIndexes(out var startIndex, out var finalIndex);
            SetItems(monthDate.FirstMonthDate(), startIndex, finalIndex);
        }

        private void InitializeOnStart()
        {
            if(_initializedOnStart)
                return;

            _rectTransform = GetComponent<RectTransform>();
            GetItemSize();
            
            _initializedOnStart = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetItemSize()
        {
            var gridSize = _rectTransform.rect.size;
            gridSize -= gridMargin;
            
            _itemSpace = new Vector2(gridSize.x / Params.GridSize.Column, gridSize.y / Params.GridSize.Row);
            var itemsSize = _itemSpace - cellOffset;
            var minValue = Mathf.Min(itemsSize.x, itemsSize.y);
            _itemSize = new Vector2(minValue, minValue);
        }

        private void SetItems(DateTime startDate, GridCell startIndex, GridCell finalIndex)
        {
            var startTotalIndex = startIndex.GetTotalIndex(Params.GridSize);
            var finalTotalIndex = finalIndex.GetTotalIndex(Params.GridSize);

            var startPosition = _itemSpace / 2f;
            var dayIncrement = 0;
            for (var index = startTotalIndex; index <= finalTotalIndex; index++, dayIncrement++)
            {
                var cell = GridCell.FromTotalIndex(index, in Params.GridSize);
                var dateItemSetUpData = new DateItemSetUpData
                {
                    Date = startDate.AddDays(dayIncrement),
                    Size = _itemSize,
                    Position = new Vector2(startPosition.x + (_itemSpace.x * cell.Column), -(startPosition.y + (_itemSpace.y * cell.Row)))
                };

                var dateItemGameObject = Instantiate(dateItemTemplate, transform);
                var dateItem = dateItemGameObject.transform.GetComponent<DateItem>();
                dateItem.SetUp(in dateItemSetUpData);
                
                _dayObjects.Add(dateItemGameObject);
            }
        }

        /// <summary>
        /// Разрушает все кнопки даты
        /// </summary>
        public void DestroyItems(int monthIncrement)
        {
            for(var i = 0; i < _dayObjects.Count; i++)
                Destroy(_dayObjects[i]);
            
            _dayObjects.Clear();
            
            EventManager.RaiseEvent(EventType.OnMonthChanged, monthIncrement);
        }
    }
}