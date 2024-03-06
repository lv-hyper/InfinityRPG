namespace Gpm.Ui
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class InfiniteScroll : MonoBehaviour, IItemContainer, IMoveScroll
    {
        public enum MoveToType
        {
            MOVE_TO_TOP = 0,
            MOVE_TO_CENTER,
            MOVE_TO_BOTTOM
        }

        public InfiniteScrollItem               itemPrefab              = null;
        public int                              padding                 = 0;
        public int                              space                   = 0;
        public bool                             dynamicItemSize         = false;

        public ScrollLayout                     layout                  = new ScrollLayout();

        protected bool                          isInitialize            = false;
        protected ScrollRect                    scrollRect              = null;
        protected RectTransform                 content                 = null;
        protected RectTransform                 viewport                = null;
        protected bool                          isVertical              = false;
        protected Vector2                       anchorMin               = Vector2.zero;
        protected Vector2                       anchorMax               = Vector2.zero;
        protected List<InfiniteScrollData>      dataList                = new List<InfiniteScrollData>();
        protected float                         defaultItemSize         = 0.0f;
        protected int                           needItemNumber          = -1;
        protected int                           madeItemNumber          = 0;
        protected List<InfiniteScrollItem>      items                   = new List<InfiniteScrollItem>();
        protected float                         firstItemPosition       = 0.0f;
        protected int                           selectDataIndex         = -1;
        protected Action<InfiniteScrollData>    selectCallback          = null;
        protected float                         sizeInterpolationValue  = 0.0001f; // 0.01%
        protected List<float>                   itemSizeList            = new List<float>();
        protected float                         minItemSize             = 0.0f;
        protected bool                          processing              = false;
        protected bool                          isDirty = true;


        protected float                         contentsPostion         = 0;

        private int                             firstDataIndex      = 0;
        private int                             lastDataIndex       = 0;
        private bool                            isStartLine           = true;
        private bool                            isEndLine             = true;

        private bool                            changeValue             = false;

        public ChangeValueEvent                 onChangeValue           = new ChangeValueEvent();
        public ItemActiveEvent                  onChangeActiveItem      = new ItemActiveEvent();

        public StateChangeEvent                 onStartLine           = new StateChangeEvent();
        public StateChangeEvent                 onEndLine            = new StateChangeEvent();
        
        public Vector2 GetScrollPostion()
        {
            return content.anchoredPosition;
        }

        public void SetScrollPostion(Vector2 postion)
        {
            content.anchoredPosition = postion;
        }

        public bool IsMoveToFirstData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            float viewportSize = GetViewportSize();
            float contentSize = GetContentSize();
            float position = GetContentPosition();

            return IsMoveToFirstData(position, viewportSize, contentSize);
        }

        private bool IsMoveToFirstData(float position, float viewportSize, float contentSize)
        {
            bool isShow = false;

            if (viewportSize > contentSize)
            {
                isShow = true;
            }
            else
            {
                float interpolation = contentSize * sizeInterpolationValue;
                if (-position > -interpolation)
                {
                    isShow = true;
                }
            }

            return isShow;
        }

        public bool IsMoveToLastData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            float viewportSize = GetViewportSize();
            float contentSize = GetContentSize();
            float position = GetContentPosition();

            return IsMoveToLastData(position, viewportSize, contentSize);
        }

        private bool IsMoveToLastData(float position, float viewportSize, float contentSize)
        {
            bool isShow = false;

            if (viewportSize > contentSize)
            {
                isShow = true;
            }
            else
            {
                float interpolation = contentSize * sizeInterpolationValue;

                if (-(viewportSize + position - contentSize) <= interpolation)
                {
                    isShow = true;
                }
            }

            return isShow;
        }

        public void ResizeScrollView()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            CheckNeedMoreItem();
        }

        public void MoveTo(InfiniteScrollData data, MoveToType moveToType, float time = 0)
        {
            MoveTo(GetDataIndex(data), moveToType, time);
        }

        public void MoveTo(int dataIndex, MoveToType moveToType, float time = 0)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(dataIndex) == true)
            {
                if (time > 0)
                {
                    Control.MoveTo(this, dataIndex, moveToType, time);
                }
                else
                {
                    Vector2 prevPosition = content.anchoredPosition;
                    float move = 0.0f;

                    if (isVertical == true)
                    {
                        move = GetMovePosition(dataIndex, viewport.rect.height, content.rect.height, moveToType);
                        content.anchoredPosition = new Vector2(prevPosition.x, move);
                    }
                    else
                    {
                        move = GetMovePosition(dataIndex, viewport.rect.width, content.rect.width, moveToType);
                        content.anchoredPosition = new Vector2(-move, prevPosition.y);
                    }
                }
            }
        }

        public Vector2 GetMovePosition(int dataIndex, MoveToType moveToType)
        {
            Vector2 position = content.anchoredPosition;

            float move = GetMovePosition(dataIndex, isVertical, moveToType);
            if (isVertical == true)
            {
                position.y = move;
            }
            else
            {
                position.x = -move;
            }

            return position;
        }

        internal float GetMovePosition(int dataIndex, bool isVertical, MoveToType moveToType)
        {
            if (isVertical == true)
            {
                return GetMovePosition(dataIndex, viewport.rect.height, content.rect.height, moveToType);
            }
            else
            {
                return GetMovePosition(dataIndex, viewport.rect.width, content.rect.width, moveToType);
            }
        }

        internal float GetMovePosition(int dataIndex, float viewportSize, float contentSize, MoveToType moveToType)
        {
            float move = 0.0f;
            float moveItemSize = GetItemSize(dataIndex);
            float passingItemSize = GetItemSizeSumToIndex(dataIndex);

            move = passingItemSize + padding;

            switch (moveToType)
            {
                case MoveToType.MOVE_TO_CENTER:
                    {
                        move -= viewportSize * 0.5f - moveItemSize * 0.5f;
                        break;
                    }
                case MoveToType.MOVE_TO_BOTTOM:
                    {
                        move -= viewportSize - moveItemSize;
                        break;
                    }
            }

            move = Mathf.Clamp(move, 0.0f, contentSize - viewportSize);
            move = Math.Max(0.0f, move);

            return move;
        }

        public void MoveToFirstData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (isVertical == true)
            {
                scrollRect.normalizedPosition = Vector2.one;
            }
            else
            {
                scrollRect.normalizedPosition = Vector2.zero;
            }
        }

        public void MoveToLastData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (isVertical == true)
            {
                scrollRect.normalizedPosition = Vector2.zero;
            }
            else
            {
                scrollRect.normalizedPosition = Vector2.one;
            }
        }

        public void AddSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback += callback;
        }

        public void RemoveSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback -= callback;
        }

        public void AddScrollValueChangedLisnter(UnityAction<Vector2> listener)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            scrollRect.onValueChanged.AddListener(listener);
        }

        public void InsertData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            dataList.Add(data);

            CreateNeedItem(data);

            float itemSize = 0;
            if (dynamicItemSize == true)
            {
                itemSize = minItemSize;
            }
            else
            {
                itemSize = defaultItemSize;
            }
            itemSizeList.Add(itemSize);

            ResizeContent();

            UpdateShowItem();

            isDirty = true;
        }

        public void RemoveData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            int dataIndex = GetDataIndex(data);

            RemoveData(dataIndex);

            isDirty = true;
        }

        public void RemoveData(int dataIndex)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(dataIndex) == true)
            {
                selectDataIndex = -1;
                dataList.RemoveAt(dataIndex);
                itemSizeList.RemoveAt(dataIndex);
                ResizeContent();
                UpdateShowItem(true);
            }
        }

        public void ClearData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectDataIndex = -1;
            dataList.Clear();
            itemSizeList.Clear();

            ClearItems();

            ResizeContent();
            UpdateShowItem(true);
        }

        public void Clear()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            dataList.Clear();
            itemSizeList.Clear();
            ResizeContent();

            ClearItems();

            selectDataIndex = -1;
        }

        public int GetDataCount()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.Count;
        }

        public InfiniteScrollData GetData(int index)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(index) == true)
            {
                return dataList[index];
            }
            else
            {
                return null;
            }
        }

        public void UpdateData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            int dataIndex = GetDataIndex(data);
            if (IsValidDataIndex(dataIndex) == true)
            {
                if (IsShowDataIndex(dataIndex) == true)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.UpdateData(data);
                    }
                }
            }
        }

        public void UpdateAllData()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            UpdateShowItem(true);
        }

        public int GetDataIndex(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.FindIndex(p =>
            {
                return p.Equals(data);
            });
        }

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (isInitialize == false)
            {
                scrollRect = GetComponent<ScrollRect>();
                content = scrollRect.content;
                viewport = scrollRect.viewport;
                isVertical = scrollRect.vertical;

                layout.Initialize(this, isVertical);

                if (isVertical == true)
                {
                    anchorMin = new Vector2(content.anchorMin.x, 1.0f);
                    anchorMax = new Vector2(content.anchorMax.x, 1.0f);

                    content.anchorMin = anchorMin;
                    content.anchorMax = anchorMax;
                    content.pivot = new Vector2(0.5f, 1.0f);
                }
                else
                {
                    anchorMin = new Vector2(0, content.anchorMin.y);
                    anchorMax = new Vector2(0, content.anchorMax.y);

                    content.anchorMin = anchorMin;
                    content.anchorMax = anchorMax;
                    content.pivot = new Vector2(0.0f, 0.5f);
                }

                dataList.Clear();
                itemSizeList.Clear();

                scrollRect.onValueChanged.AddListener(OnValueChanged);

                isInitialize = true;
                isDirty = true;
            }
        }

        private void OnSelectItem(InfiniteScrollData data)
        {
            int dataIndex = GetDataIndex(data);
            if (IsValidDataIndex(dataIndex) == true)
            {
                selectDataIndex = dataIndex;

                if (selectCallback != null)
                {
                    selectCallback(data);
                }
            }
        }

        private bool IsValidDataIndex(int index)
        {
            return (index >= 0 && index < dataList.Count) ? true : false;
        }

        private void CreateNeedItem(InfiniteScrollData data)
        {
            if (madeItemNumber > itemSizeList.Count)
            {
                return;
            }
            if (dynamicItemSize == true)
            {
                float itemSizeSum = 0;
                float needItemSize = 0;
                if (isVertical == true)
                {
                    needItemSize = viewport.rect.height * 2;
                }
                else
                {
                    needItemSize = viewport.rect.width * 2;
                }

                if (layout.IsGrid() == true)
                {
                    int lineCount = layout.GetLineCount();
                    for (int lineIndex = 0; lineIndex < lineCount; ++lineIndex)
                    {
                        itemSizeSum += layout.GetLineSize(lineIndex);

                        if (lineIndex + 1 < lineCount)
                        {
                            itemSizeSum += space;
                        }

                        if (itemSizeSum > needItemSize)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    for (int sizeIndex = 0; sizeIndex < itemSizeList.Count; ++sizeIndex)
                    {
                        itemSizeSum += itemSizeList[sizeIndex];

                        if (sizeIndex + 1 < itemSizeList.Count)
                        {
                            itemSizeSum += space;
                        }

                        if (itemSizeSum > needItemSize)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                if (madeItemNumber > 0 &&
                    madeItemNumber == needItemNumber)
                {
                    return;
                }

                if (madeItemNumber > dataList.Count)
                {
                    return;
                }
            }

            CreateItem(data);
        }

        private InfiniteScrollItem CreateItem(InfiniteScrollData data)
        {   
            InfiniteScrollItem  item            = Instantiate(itemPrefab);
            RectTransform       itemTransform   = (RectTransform)item.transform;
            
            itemTransform.anchorMin = anchorMin;
            itemTransform.anchorMax = anchorMax;
            itemTransform.pivot                 = content.pivot;
            
            if (isVertical == true)
            {
                itemTransform.sizeDelta = new Vector2(0, itemTransform.sizeDelta.y);
            }
            else
            {
                itemTransform.sizeDelta = new Vector2(itemTransform.sizeDelta.x, 0);
            }
            
            itemTransform.SetParent(content, false);
            
            if (madeItemNumber == 0)
            {
                InitializeItemInformation(itemTransform);
            }

            ++madeItemNumber;

            items.Add(item);
            item.Initalize(this, items.Count - 1);
            item.SetActive(false, false);
            item.AddSelectCallback(OnSelectItem);

            if (dynamicItemSize == true)
            {
                item.AddUpdateSizeCallback(OnUpdateItemSize);
            }

            return item;
        }

        private void InitializeItemInformation(RectTransform itemTransform)
        {
            if (isVertical == true)
            {
                defaultItemSize     = itemTransform.rect.height;
            }
            else
            {
                defaultItemSize     = itemTransform.rect.width;
            }

            SetFirstItemPosition(defaultItemSize, itemTransform.pivot);

            minItemSize     = defaultItemSize;
            needItemNumber  = GetNeedItemNumber();

            items.Clear();
        }

        private void CheckNeedMoreItem()
        {
            int itemNumber  = GetNeedItemNumber();

            if (needItemNumber < itemNumber)
            {
                int gap = itemNumber - needItemNumber;
                needItemNumber = itemNumber;

                if (dataList.Count > 0)
                {
                    int firstDataIndex  = GetShowFirstDataIndex() + madeItemNumber;
                    int dataIndex       = 0;

                    for (int count = 0; count < gap; ++count)
                    {
                        dataIndex = firstDataIndex + count;
                        if (IsValidDataIndex(dataIndex) == true)
                        {
                            CreateNeedItem(dataList[dataIndex]);
                        }
                        else
                        {
                            CreateNeedItem(dataList[0]);
                        }
                    }
                }
                
                UpdateShowItem(true);
            }
        }

        private void ClearItems()
        {
            for (int index = 0; index < items.Count; ++index)
            {
                items[index].ClearData(false);
            }
        }
        
        private void ResizeContent()
        {
            Vector2 currentSize = content.sizeDelta;
            float   size        = GetContentSize();

            if (isVertical == true)
            {
                content.sizeDelta = new Vector2(currentSize.x, size);
            }
            else
            {
                content.sizeDelta = new Vector2(size, currentSize.y);
            }

            isDirty = true;
        }

        private float GetContentSize()
        {
            float itemTotalSize = GetItemSizeSum(itemSizeList.Count);

            return itemTotalSize + padding * 2.0f;
        }

        private float GetViewportSize()
        {
            float size = 0;
            if (isVertical == true)
            {
                size = viewport.rect.height;
            }
            else
            {
                size = viewport.rect.width;
            }

            return size;
        }

        private float GetGetContentSize()
        {
            float size = 0;
            if (isVertical == true)
            {
                size = content.rect.height;
            }
            else
            {
                size = content.rect.width;
            }

            return size;
        }

        private float GetContentPosition()
        {
            float position = 0;
            if (isVertical == true)
            {
                position = content.anchoredPosition.y;
            }
            else
            {
                position = -content.anchoredPosition.x;
            }

            return position;
        }

        private void UpdateShowItem(bool forceUpdateData = false)
        {
            if (forceUpdateData == false &&
                processing == true)
            {
                return;
            }

            float firstPosition = -GetContentPosition();

            float viewSize = GetViewportSize();
            if (contentsPostion != firstPosition)
            {
                contentsPostion = firstPosition;
                isDirty = true;
            }

            if (isDirty == false)
            {
                return;
            }
            
            processing = true;

            int prevFirstDataIndex = firstDataIndex;
            int prevLastDataIndex = lastDataIndex;

            firstDataIndex = GetShowFirstDataIndex();
            
            int lineIndex = 0;

            float position = firstPosition;
            float lineSize = layout.GetLineSize(lineIndex);

            for (int dataIndex = 0; dataIndex < firstDataIndex; ++dataIndex)
            {
                int dataLineIndex = layout.GetLineIndex(dataIndex);
                if (lineIndex != dataLineIndex)
                {
                    position += lineSize + space;

                    lineIndex = dataLineIndex;
                    lineSize = layout.GetLineSize(lineIndex);
                }
            }

            if (prevFirstDataIndex != firstDataIndex)
            {
                for (int dataIndex = prevFirstDataIndex; dataIndex < firstDataIndex; dataIndex++)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.ClearData();
                    }
                }

                changeValue = true;
            }
            
            for (int dataIndex = firstDataIndex; dataIndex < dataList.Count; ++dataIndex)
            {
                int dataLineIndex = layout.GetLineIndex(dataIndex);
                if (lineIndex != dataLineIndex)
                {   
                    position += lineSize + space;

                    lineIndex = dataLineIndex;
                    lineSize = layout.GetLineSize(lineIndex);                    
                }

                if (viewSize < position)
                {
                    break;
                }

                InfiniteScrollItem item = PullItemByDataIndex(dataIndex);
 
                bool needUpdateData = false;

                if (item.IsActiveItem == false ||
                    item.dataIndex != dataIndex)
                {
                    item.SetData(dataIndex);

                    needUpdateData = true;

                    changeValue = true;
                }

                if (needUpdateData == true || forceUpdateData == true)
                {
                    item.UpdateData(dataList[dataIndex]);
                }

                RectTransform itemTransform = (RectTransform)item.transform;
                layout.SetItemSizeAndPosition(itemTransform, dataIndex);

                lastDataIndex = dataIndex;
            }
            
            if (prevLastDataIndex != lastDataIndex)
            {
                for (int dataIndex = lastDataIndex + 1; dataIndex <= prevLastDataIndex; dataIndex++)
                {
                    InfiniteScrollItem item = GetItemByDataIndex(dataIndex);
                    if (item != null)
                    {
                        item.ClearData();
                    }
                }

                changeValue = true;
            }

            if (changeValue == true)
            {
                onChangeValue.Invoke(firstDataIndex, lastDataIndex, isStartLine, isEndLine);
                changeValue = false;
            }

            processing = false;
        }

        private InfiniteScrollItem PullItemByDataIndex(int dataIndex)
        {
            InfiniteScrollItem item = null;

            int itemIndex = GetItemIndexByDataIndex(dataIndex, true);
            if (itemIndex == -1)
            {
                item = CreateItem(dataList[dataIndex]);
            }
            else
            {
                item = items[itemIndex];
            }

            return item;
        }

        private InfiniteScrollItem GetItemByDataIndex(int dataIndex)
        {
            int itemIndex = GetItemIndexByDataIndex(dataIndex, false);
            if (itemIndex != -1)
            {
                return items[itemIndex];
            }

            return null;
        }

        private int GetItemIndexByDataIndex(int dataIndex, bool findEmptyIndex = false)
        {
            int emptyIndex = -1;
            for (int index = 0; index < items.Count; ++index)
            {
                if (items[index].dataIndex == dataIndex)
                {
                    return index;
                }

                if (findEmptyIndex == true)
                {
                    if (emptyIndex == -1 &&
                     items[index].dataIndex == -1)
                    {
                        emptyIndex = index;
                    }
                }
            }

            return emptyIndex;
        }
     
        private bool IsShowDataIndex(int dataIndex)
        {
            if (dataIndex >= firstDataIndex && dataIndex <= lastDataIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private int GetShowFirstDataIndex()
        {
            int     index           = -1;
            float   contentPosition = GetContentPosition();
            float   itemSizeSum     = 0.0f;
          
            int lineCount = layout.GetLineCount();
            for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
            {
                itemSizeSum += layout.GetLineSize(lineIndex);

                if (itemSizeSum >= contentPosition)
                {
                    index = layout.GetLineFirstItemIndex(lineIndex);
                    break;
                }

                itemSizeSum += space;
            }

            if (index < 0)
            {
                index = 0;
            }

            return index;
        }    
        
        private void OnValueChanged(Vector2 value)
        {
            bool prevIsStartLine = isStartLine;
            isStartLine = IsMoveToFirstData();
            if (prevIsStartLine != isStartLine)
            {
                onStartLine.Invoke(isStartLine);

                changeValue = true;
            }

            bool prevIsEndLine = isEndLine;
            isEndLine = IsMoveToLastData();
            if (prevIsEndLine != isEndLine)
            {
                onEndLine.Invoke(isEndLine);

                changeValue = true;
            }

            UpdateShowItem();            
        }

        private void OnUpdateItemSize(InfiniteScrollData data, RectTransform itemTransform)
        {
            int dataIndex = GetDataIndex(data);

            if (IsValidDataIndex(dataIndex) == true)
            {
                float size = 0.0f;

                if (isVertical == true)
                {
                    size = itemTransform.rect.height;
                }
                else
                {
                    size = itemTransform.rect.width;
                }

                if (itemSizeList[dataIndex] == size)
                {
                    return;
                }

                if (dataIndex == 0)
                {
                    SetFirstItemPosition(size, itemTransform.pivot);
                }

                itemSizeList[dataIndex] = size;

                ResizeContent();

                if (minItemSize == 0 ||
                    minItemSize > size)
                {
                    minItemSize = size;
                    CheckNeedMoreItem();
                }
                else
                {
                    UpdateShowItem();
                }
            }
        }
         
        private float GetItemSizeSum(int toIndex)
        {
            if (toIndex >= itemSizeList.Count)
            {
                toIndex = itemSizeList.Count - 1;
            }

            float sizeSum = 0.0f;

            int lineCount = layout.GetLineCount(toIndex);
            if (dynamicItemSize == true)
            {
                for (int lineIdx = 0; lineIdx < lineCount; lineIdx++)
                {
                    sizeSum += layout.GetLineSize(lineIdx);
                }
            }
            else
            {
                sizeSum = defaultItemSize * lineCount;
            }

            if (lineCount > 0)
            {
                int spaceCount = lineCount;

                int maxLineCount = layout.GetLineCount();
                if (lineCount == maxLineCount)
                {
                    spaceCount--;
                }

                sizeSum = sizeSum + space * spaceCount;
            }

            return sizeSum;
        }

        public float GetItemSizeSumToIndex(int toIndex)
        {
            if (toIndex >= itemSizeList.Count)
            {
                toIndex = itemSizeList.Count - 1;
            }

            float sizeSum = 0.0f;

            int lineCount = layout.GetLineCount(toIndex) - 1;
            if (dynamicItemSize == true)
            {
                for (int lineIdx = 0; lineIdx < lineCount; lineIdx++)
                {
                    sizeSum += layout.GetLineSize(lineIdx);
                }
            }
            else
            {
                sizeSum = defaultItemSize * lineCount;
            }

            if (lineCount > 0)
            {
                int spaceCount = lineCount;

                int maxLineCount = layout.GetLineCount();
                if (lineCount == maxLineCount)
                {
                    spaceCount--;
                }

                sizeSum = sizeSum + space * spaceCount;
            }

            return sizeSum;
        }

        public float GetItemSize(int dataIndex)
        {
            float size = minItemSize;

            if (dynamicItemSize == true)
            {
                if (dataIndex < itemSizeList.Count)
                {
                    size = itemSizeList[dataIndex];
                }
            }
            else
            {
                size = defaultItemSize;
            }

            return size;
        }

        public float GetItemPostion(int dataIndex)
        {
            float firstItemPosition = GetFirstItemPosition();
            float passingItemSize = GetItemSizeSumToIndex(dataIndex);

            if (isVertical == true)
            {
                return firstItemPosition - passingItemSize;
            }
            else
            {
                return firstItemPosition + passingItemSize;
            }
        }

        public int GetItemCount()
        {
            return itemSizeList.Count;
        }

        public bool IsDynamicItemSize()
        {
            return dynamicItemSize;
        }

        private void SetFirstItemPosition(float itemSize, Vector2 pivot)
        {
            if (isVertical == true)
            {
                firstItemPosition = itemSize * pivot.y - itemSize - padding;
            }
            else
            {
                firstItemPosition = itemSize * pivot.x + padding;
            }
        }

        public float GetFirstItemPosition()
        {
            return firstItemPosition;
        }

        private int GetNeedItemNumber()
        {
            int needItemNumber = 0;

            float itemSize = 0.0f;
            if (dynamicItemSize == true)
            {
                itemSize = minItemSize;
            }
            else
            {
                itemSize = defaultItemSize;
            }

            if (itemSize > 0)
            {
                if (isVertical == true)
                {
                    needItemNumber = (int)(viewport.rect.height / itemSize);
                }
                else
                {
                    needItemNumber = (int)(viewport.rect.width / itemSize);
                }
                needItemNumber += 1;

                if(layout.IsGrid() == true)
                {
                   needItemNumber = needItemNumber * layout.values.Count;
                }

                needItemNumber += 2;
            }

            return needItemNumber;
        }

        private void OnValidate()
        {
            layout.SetDefaults();
        }

        [Serializable]
        public class ChangeValueEvent : UnityEvent<int, int, bool, bool>
        {
            public ChangeValueEvent()
            {
            }
        }

        [Serializable]
        public class ItemActiveEvent : UnityEvent<int, bool>
        {
            public ItemActiveEvent()
            {
            }
        }

        [Serializable]
        public class StateChangeEvent : UnityEvent<bool>
        {
            public StateChangeEvent()
            {
            }
        }

        public static class Control
        {
            public static void MoveTo(InfiniteScroll scroll, int dataIndex, MoveToType moveToType, float time = 0)
            {
                ScrollMoveTo spring = scroll.gameObject.GetComponent<ScrollMoveTo>();
                if (spring == null)
                {
                    spring = scroll.gameObject.AddComponent<ScrollMoveTo>();
                }

                spring.dataIndex = dataIndex;
                spring.moveToType = moveToType;
                spring.time = time;
                spring.curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

                spring.autoDestory = true;

                spring.Play();
            }
        }
    }
}
