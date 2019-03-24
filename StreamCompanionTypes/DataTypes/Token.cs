using StreamCompanionTypes.Enums;
using System;
using System.Diagnostics;
using System.Globalization;

namespace StreamCompanionTypes.DataTypes
{

    public class Token
    {
        public TokenType Type { get; set; }
        private object RawValue { get; set; }
        public object Value
        {
            get { return RawValue; }
            set
            {
                RawValue = value;

                if (RawValue is null)
                    FormatedValue = "";
                else
                    FormatedValue = string.IsNullOrEmpty(Format)
                        ? RawValue.ToString()
                        : string.Format(CultureInfo.InvariantCulture, Format, RawValue);
            }
        }

        private readonly object _defaultValue;
        public string Format { get; set; }
        public string FormatedValue { get; set; }
        /// <summary>
        /// Name of the plugin that created this token
        /// </summary>
        public string PluginName { get; set; }
        internal Token(object value, TokenType type = TokenType.Normal, string format = null, object defaultValue = null)
        {
            Debug.Assert(!(value is Token));

            Format = format;
            _defaultValue = defaultValue;
            Type = type;
            Value = value;
        }
        public void Reset()
        {
            Value = _defaultValue;
        }

        public Token Clone()
        {
            return (Token)this.MemberwiseClone();
        }
    }
    
}