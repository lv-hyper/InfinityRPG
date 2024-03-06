namespace Gpm.Ui
{
    using System;

    using System.Collections.Generic;
    using UnityEngine;

    public interface IItemContainer
    {
        float GetItemSize(int toIndex);

        float GetItemPostion(int toIndex);

        int GetItemCount();

        bool IsDynamicItemSize();
    }

    [Serializable]
    public class ScrollLayout
    {
        [Serializable]
        public class LayoutValue
        {
            public enum ValueType
            {
                DEFAULT,
                RATE,
            }

            [HideInInspector]
            public ValueType valueType;
            public float value;
        }


        private IItemContainer container;

        private bool isVertical;

        public List<LayoutValue> values = new List<LayoutValue>();

        public void Initialize(IItemContainer container, bool isVertical)
        {
            this.container = container;
            this.isVertical = isVertical;
        }

        public void SetDefaults()
        {
            foreach (var layoutValue in values)
            {
                if (layoutValue.valueType == LayoutValue.ValueType.DEFAULT)
                {
                    layoutValue.valueType = LayoutValue.ValueType.RATE;
                    layoutValue.value = 1;
                }
            }
        }

        public int GetLineIndex(int index)
        {
            if (container.GetItemCount() == 0)
            {
                return 0;
            }

            if (index >= container.GetItemCount())
            {
                index = container.GetItemCount() - 1;
            }

            if (IsGrid() == true)
            {
                // Calculate grid line
                return index / values.Count;
            }
            return index;
        }

        public int GetLineCount(int index)
        {
            if (container.GetItemCount() == 0)
            {
                return 0;
            }

            if (index >= container.GetItemCount())
            {
                index = container.GetItemCount() - 1;
            }
            return GetLineIndex(index) + 1;
        }

        public int GetLineCount()
        {
            return GetLineCount(container.GetItemCount() - 1);
        }

        private float GetLineSizeFromIndex(int index)
        {
            if (container.GetItemCount() == 0)
            {
                return 0;
            }

            if (index >= container.GetItemCount())
            {
                index = container.GetItemCount() - 1;
            }

            if (IsGrid() == true)
            {
                return GetLineSize(GetLineIndex(index));
            }

            return container.GetItemSize(index);
        }


        public int GetLineFirstItemIndex(int lineIndex)
        {
            int lineCount = GetLineCount();

            if (lineIndex >= lineCount)
            {
                lineIndex = lineCount - 1;
            }

            int firstItemIndex = lineIndex;
            if (IsGrid() == true)
            {
                firstItemIndex = firstItemIndex * values.Count;
            }
            return firstItemIndex;
        }

        public float GetLineSize(int lineIndex)
        {
            int lineCount = GetLineCount();
            if (lineCount == 0)
            {
                return 0;
            }

            if (lineIndex >= lineCount)
            {
                lineIndex = lineCount - 1;
            }

            if (container.IsDynamicItemSize() == true)
            {
                int itemCount = container.GetItemCount();
                if (itemCount > 0)
                {
                    if (IsGrid() == true)
                    {
                        int firstItemIndex = GetLineFirstItemIndex(lineIndex);
                        if (firstItemIndex < itemCount)
                        {
                            float lineSize = container.GetItemSize(firstItemIndex);

                            for (int gridIdx = 1; gridIdx < values.Count; gridIdx++)
                            {
                                int index = firstItemIndex + gridIdx;
                                if (index < itemCount)
                                {
                                    float gridSize = container.GetItemSize(index);
                                    if (lineSize < gridSize)
                                    {
                                        lineSize = gridSize;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            return lineSize;
                        }
                    }
                }
            }

            return container.GetItemSize(lineIndex);
        }

        public void SetItemSizeAndPosition(RectTransform rectTransform, int dataIndex)
        {
            float itemPosition = container.GetItemPostion(dataIndex);
            float size = container.GetItemSize(dataIndex);
            
            Vector2 currentSize = rectTransform.sizeDelta;

            if (isVertical == true)
            {
                if (IsGrid() == true)
                {
                    float inlineMaxSize = 0;
                    foreach (var layoutValue in values)
                    {
                        inlineMaxSize += layoutValue.value;
                    }

                    int inlineIndex = dataIndex % values.Count;

                    float inlinePos = 0;
                    float inlineSize = values[inlineIndex].value;

                    if (inlineMaxSize > 0 &&
                        inlineSize > 0)
                    {
                        for (int index = 0; index < inlineIndex; index++)
                        {
                            inlinePos += values[index].value;
                        }
                        inlinePos = inlinePos / inlineMaxSize;
                        inlineSize = inlineSize / inlineMaxSize;

                        rectTransform.anchorMin = new Vector2(inlinePos, 1.0f);
                        rectTransform.anchorMax = new Vector2(inlinePos + inlineSize, 1.0f);
                    }
                    else
                    {
                        rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                        rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    }
                    currentSize = rectTransform.sizeDelta;
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                }

                rectTransform.sizeDelta = new Vector2(currentSize.x, size);
                rectTransform.anchoredPosition = new Vector2(0, itemPosition);

            }
            else
            {
                if (IsGrid() == true)
                {
                    float inlineMaxSize = 0;
                    foreach (var layoutValue in values)
                    {
                        inlineMaxSize += layoutValue.value;
                    }

                    int inlineIndex = dataIndex % values.Count;

                    float inlinePos = 0;
                    float inlineSize = values[inlineIndex].value;

                    if (inlineMaxSize > 0 &&
                        inlineSize > 0)
                    {
                        for (int index = 0; index < inlineIndex; index++)
                        {
                            inlinePos += values[index].value;
                        }
                        inlinePos = (inlineMaxSize - inlinePos) / inlineMaxSize;
                        inlineSize = inlineSize / inlineMaxSize;

                        rectTransform.anchorMin = new Vector2(0, inlinePos - inlineSize);
                        rectTransform.anchorMax = new Vector2(0, inlinePos);
                    }
                    else
                    {
                        rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                        rectTransform.anchorMax = new Vector2(0.0f, 0.0f);
                    }
                    currentSize = rectTransform.sizeDelta;
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                }

                rectTransform.sizeDelta = new Vector2(size, currentSize.y);
                rectTransform.anchoredPosition = new Vector2(itemPosition, 0);
            }
        }

        public bool IsGrid()
        {
            if (values.Count > 1)
            {
                return true;
            }

            return false;
        }
    }
}