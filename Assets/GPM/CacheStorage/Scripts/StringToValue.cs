using System;
using UnityEngine;

namespace Gpm.CacheStorage
{
    [Serializable]
    public struct StringToValue<T>
    {
        [SerializeField]
        private string text;

        private T value;

        private bool converted;

        public string GetText()
        {
            return text;
        }

        public T GetValue()
        {
            if (converted == false)
            {
                return ConvertValue();
            }

            return value;
        }

        private T ConvertValue()
        {
            try
            {
                value = (T)Convert.ChangeType(text, typeof(T));
                converted = true;
            }
            catch
            {
                SetText("");
            }

            return value;
        }

        public StringToValue(string text = "")
        {
            this.text = text;

            this.value = default(T);

            this.converted = false;
        }

        public StringToValue(T value)
        {
            this.text = value.ToString();

            this.value = value;

            this.converted = true;
        }

        public void SetText(string text)
        {
            if (text.Equals(this.text) == false)
            {
                this.text = text;

                this.value = default(T);

                this.converted = false;
            }
        }

        public void SetValue(T value)
        {
            this.text = value.ToString();

            this.value = value;

            this.converted = true;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                if (converted == false)
                {
                    ConvertValue();
                }

                return converted;
            }

            return false;
        }

        public static implicit operator T(StringToValue<T> data)
        {
            return data.GetValue();
        }

        public static implicit operator string(StringToValue<T> data)
        {
            return data.text;
        }

        public static implicit operator StringToValue<T>(string value)
        {
            return new StringToValue<T>(value);
        }

        public static implicit operator StringToValue<T>(T value)
        {
            return new StringToValue<T>(value);
        }

        public override string ToString()
        {
            return text;
        }
    }
}