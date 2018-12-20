using StreamCompanionTypes.Enums;
using System;
using System.Diagnostics;
using System.Globalization;

namespace StreamCompanionTypes.DataTypes
{

    public class Token
    {
        public TokenType Type { get; set; }
        public object Value { get; set; }
        public Token(object value, TokenType type = TokenType.Normal)
        {
            Debug.Assert(!(value is Token));
            Debug.Assert(!(value is TokenWithFormat));

            Value = value;
            Type = type;
        }
        public Token Clone()
        {
            return (Token)this.MemberwiseClone();
        }
    }

    public class TokenWithFormat : Token
    {
        public TokenWithFormat(object value, TokenType type = TokenType.Normal, string format = null) : base(value, type)
        {
            Format = format;
            Value = base.Value;
        }
        private object _value { get; set; }

        public new object Value
        {
            get { return _value; }
            set
            {
                _value = value;

                if (_value is null)
                    FormatedValue = "";
                else
                    FormatedValue = string.IsNullOrEmpty(Format)
                        ? _value.ToString()
                        : string.Format(CultureInfo.InvariantCulture, Format, _value);
            }
        }

        public string Format { get; set; }
        public string FormatedValue { get; set; }
    }
}