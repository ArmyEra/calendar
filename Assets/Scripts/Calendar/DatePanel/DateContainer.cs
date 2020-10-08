using System;
using Calendar.DatePanel.Utils;
using Extensions;
using UnityEngine;
using Utils;

namespace Calendar.DatePanel
{
    public class DateContainer : MonoBehaviour
    {
        private static bool _itemRectInitialized;
        private static Vector2 _itemSpace = Vector2.zero;
        private static Vector2 _itemSize = Vector2.zero;
        
        [Header("Настройки сетки")]
        [SerializeField] private Vector2 gridMargin;
        [SerializeField] private Vector2 cellOffset;
        [SerializeField] private GameObject dateItemTemplate;

        private RectTransform _rectTransform;
        
        public void Initialize(in DateTime monthDate)
        {
            _rectTransform = GetComponent<RectTransform>();
            monthDate.GetBoundIndexes(out var startIndex, out var finalIndex);

            if (!_itemRectInitialized)
                GetItemSize();
            
            SetItems(monthDate, startIndex, finalIndex);
            // print(startIndex.GetTotalIndex(Params.GridSize));
            // print(finalIndex.GetTotalIndex(Params.GridSize));
        }

        private void GetItemSize()
        {
            var gridSize = _rectTransform.rect.size;
            gridSize -= gridMargin;
            
            _itemSpace = new Vector2(gridSize.x / Params.GridSize.Column, gridSize.y / Params.GridSize.Row);
            var itemsSize = _itemSpace - cellOffset;
            var minValue = Mathf.Min(itemsSize.x, itemsSize.y);
            _itemSize = new Vector2(minValue, minValue);
        }

        private void SetItems(DateTime now, GridCell startIndex, GridCell finalIndex)
        {
            var startDate = now.FirstMonthDate();
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
            }
        }
    }
}