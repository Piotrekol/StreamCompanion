using StreamCompanionTypes.Enums;
using System.Collections.Generic;

namespace StreamCompanionTypes.DataTypes
{
    public class Tokens : Dictionary<string, Token>
    {
        public Tokens(string groupName="Default")
        {
            GroupName = groupName;
        }

        public string GroupName { get; set; }

    }
}