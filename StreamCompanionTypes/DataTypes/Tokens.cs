using StreamCompanionTypes.Enums;
using System;
using System.Collections.Generic;

namespace StreamCompanionTypes.DataTypes
{
    public class Tokens : Dictionary<string, Token>
    {
        public Tokens(string groupName = "Default")
        {
            GroupName = groupName;
        }

        public string GroupName { get; set; }

        /// <summary>
        /// Stores all created tokens<para/>
        /// DO NOT add or remove any of the values of this dictionary
        /// </summary>
        public static Dictionary<string, Token> AllTokens { get; set; } = new Dictionary<string, Token>();

        public static event EventHandler AllTokensChanged;
        /// <summary>
        /// Returns existing token instance with updated value or creates new instance it if it doesn't exist
        /// </summary>
        internal static Token SetToken(string pluginName, string tokenName, object value, TokenType type = TokenType.Normal, string format = null, object defaultValue = null)
        {
            Token token;

            if (AllTokens.ContainsKey(tokenName))
            {
                token = AllTokens[tokenName];
                token.Value = value;
            }
            else
            {
                token = new Token(value, type, format, defaultValue);
                token.PluginName = pluginName;
                AllTokens[tokenName] = token;

                if ((type & TokenType.Live) != 0)
                    OutputPattern.AddLiveToken(tokenName);

                AllTokensChanged?.Invoke(null, EventArgs.Empty);
            }

            return token;
        }

        public static TokenSetter CreateTokenSetter(string pluginName)
        {
            return (tokenName, value, type, format, defaultValue) => SetToken(pluginName, tokenName, value, type, format, defaultValue);
        }

        /// <summary>
        /// <inheritdoc cref="Tokens.SetToken"/>
        /// </summary>
        public delegate Token TokenSetter(string tokenName, object value, TokenType type = TokenType.Normal,
            string format = null, object defaultValue = null);
    }
}