using StreamCompanion.Common;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClickCounter
{
    public class KeyEntry
    {
        [Browsable(false)]
        public int Code { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        private string _tokenName;
        [IgnoreDataMember]
        public string TokenName => _tokenName ??= $"key-{Name}".ToLowerInvariant().RemoveWhitespace();
        [IgnoreDataMember]
        [Browsable(false)]
        public bool Pressed;
    }
}
