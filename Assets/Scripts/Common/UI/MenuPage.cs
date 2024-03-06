using System;

namespace Common.UI
{
    public class MenuPage : MenuComponent
    {
        public override void OpenChild(string menuName)
        {
            throw new NotSupportedException();
        }
        public override void Open()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public override string Name => gameObject.name;
    }
}