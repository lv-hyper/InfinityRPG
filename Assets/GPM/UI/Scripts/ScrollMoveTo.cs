namespace Gpm.Ui
{
    using UnityEngine;

    using MoveToType = InfiniteScroll.MoveToType;

    public interface IMoveScroll
    {
        Vector2 GetScrollPostion();

        void SetScrollPostion(Vector2 postion);

        Vector2 GetMovePosition(int dataIndex, MoveToType moveToType);
    }

    [DisallowMultipleComponent]
    public class ScrollMoveTo : MonoBehaviour
    {
        private IMoveScroll scroll;

        private void OnEnable()
        {
            if (scroll == null)
            {
                scroll = GetComponent<IMoveScroll>();
            }
        }

        public int dataIndex;
        public MoveToType moveToType;
        public float time = 0.3f;

        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public bool autoDestory = false;

        private float rate = 0;
        
        private Vector2 start;

        public void Play()
        {
            rate = 0;
            enabled = true;
        }

        public void Update()
        {
            if (scroll == null)
            {
                return;
            }

            if (rate == 0)
            {
                start = scroll.GetScrollPostion();
            }

            if (time > 0)
            {
                rate += Time.deltaTime / time;
            }
            else
            {
                rate = 1;
            }

            Vector2 end = scroll.GetMovePosition(dataIndex, moveToType);
            if (rate < 1)
            {
                scroll.SetScrollPostion(Vector2.Lerp(start, end, curve.Evaluate(rate)));                
            }
            else
            {
                scroll.SetScrollPostion(end);
                rate = 0;

                if (autoDestory == true)
                {
                    Destroy(this);
                }
                enabled = false;
            }
        }

    }
}