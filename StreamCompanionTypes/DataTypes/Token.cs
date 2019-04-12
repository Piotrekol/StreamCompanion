using StreamCompanionTypes.Enums;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace StreamCompanionTypes.DataTypes
{

    public class Token
    {
        public TokenType Type { get; set; }
        private object RawValue { get; set; }
        public bool FormatIsValid { get; private set; }
        public object Value
        {
            get { return RawValue; }
            set
            {
                RawValue = value;
                if (!FormatIsValid)
                    return;

                if (RawValue is null)
                    FormatedValue = "";
                else
                {
                    var formatIsValid = FormatIsValid;
                    FormatedValue = string.IsNullOrEmpty(Format)
                        ? RawValue.ToString()
                        : TryFormat(CultureInfo.InvariantCulture, Format, out formatIsValid, RawValue);

                    if (FormatedValue.StartsWith("Eval:"))
                    {
                        var result = Evaluate(FormatedValue.Substring(5, FormatedValue.Length - 5));
                        FormatedValue = result.ToString(CultureInfo.InvariantCulture);
                    }

                    FormatIsValid = formatIsValid;
                }
            }
        }

        private readonly object _defaultValue;
        private string _format;

        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                FormatIsValid = true;
                Value = Value;
            }
        }

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

        private static string TryFormat(IFormatProvider formatProvider, string format, out bool valid, params object[] args)
        {
            var result = "INVALID FORMAT";
            try
            {
                result = string.Format(formatProvider, format, args);
                valid = true;
            }
            catch (FormatException)
            {
                valid = false;
            }

            return result;
        }
        public double Evaluate(string expression)
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("expression", typeof(string), expression);
                DataRow row = table.NewRow();
                table.Rows.Add(row);
                return double.Parse(((string)row["expression"]).Replace(',', '.'), CultureInfo.InvariantCulture);
            }
            catch
            {
                FormatIsValid = false;
                return Double.NaN;
            }
        }
    }

}