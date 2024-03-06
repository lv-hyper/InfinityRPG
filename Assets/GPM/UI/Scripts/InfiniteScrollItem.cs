namespace Gpm.Ui
{
    using System;
    using UnityEngine;

    public class InfiniteScrollData
    {
    }


    public class InfiniteScrollItem : MonoBehaviour
    {
        protected bool activeItem;

        protected InfiniteScroll scroll = null;

        protected InfiniteScrollData scrollData = null;
        protected Action<InfiniteScrollData> selectCallback = null;
        protected Action<InfiniteScrollData, RectTransform> updateSizeCallback = null;

        internal int itemIndex = -1;
        internal int dataIndex = -1;

        public bool IsActiveItem
        {
            get
            {
                return activeItem;
            }
        }

        internal void Initalize(InfiniteScroll scroll, int itemIndex)
        {
            this.scroll = scroll;
            this.itemIndex = itemIndex;
        }

        internal void SetData(int dataIndex, bool notifyEvent = true)
        {
            this.scrollData = scroll.GetData(dataIndex);
            this.dataIndex = dataIndex;

            SetActive(true, notifyEvent);
        }

        internal void ClearData(bool notifyEvent = true)
        {
            this.scrollData = null;
            this.dataIndex = -1;

            SetActive(false, notifyEvent);
        }

        public void AddSelectCallback(Action<InfiniteScrollData> callback)
        {
            selectCallback += callback;
        }

        public void RemoveSelectCallback(Action<InfiniteScrollData> callback)
        {
            selectCallback -= callback;
        }

        public virtual void UpdateData(InfiniteScrollData scrollData)
        {
            this.scrollData = scrollData;
        }

        protected void OnSelect()
        {
            if (selectCallback != null)
            {
                selectCallback(scrollData);
            }
        }

        public virtual void SetActive(bool active, bool notifyEvent = true)
        {
            activeItem = active;

            gameObject.SetActive(activeItem);

            if (notifyEvent == true)
            {
                if (scroll != null)
                {
                    scroll.onChangeActiveItem.Invoke(dataIndex, activeItem);
                }
            }
        }

        public void SetSize(Vector2 sizeDelta)
        {
            RectTransform rectTransform = (RectTransform)transform;
            if(rectTransform.sizeDelta != sizeDelta)
            {
                rectTransform.sizeDelta = sizeDelta;
                OnUpdateItemSize();
            }
        }

        public void AddUpdateSizeCallback(Action<InfiniteScrollData, RectTransform> callback)
        {
            updateSizeCallback += callback;
        }

        public void RemoveUpdateSizeCallback(Action<InfiniteScrollData, RectTransform> callback)
        {
            updateSizeCallback -= callback;
        }

        protected void OnUpdateItemSize()
        {
            if (updateSizeCallback != null)
            {
                updateSizeCallback(scrollData, transform as RectTransform);
            }
        }
    }
}
