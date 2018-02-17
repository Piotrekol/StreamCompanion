using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    public class UpdateContainer
    {
        public string Version { get; set; }
        public string DownloadPageUrl { get; set; }
        public string ExeDownloadUrl { get; set; }
        public int ExpectedExeSizeInBytes { get; set; }
        public double ExpectedExeSizeInMbytes => ExpectedExeSizeInBytes > 0 ? ExpectedExeSizeInBytes / 1024d : 0d;
        public Dictionary<string, string> Changelog { get; set; }

        public string GetChangelog(bool rtf)
        {
            if (Changelog == null) return "There was a problem while retrieving changelog";
            var ret = string.Empty;
            if (rtf)
            {
                foreach (var e in Changelog)
                {
                    var version = e.Key;
                    var changelog = e.Value;
                    ret += @"{\rtf1\ansi";
                    ret += string.Format(@"\b Version: {0}\b0 \line ",version);
                    ret += changelog.Replace("\n",@" \line ").Replace("\t",@"\tab")+ @" \line \line ";
                }
                ret += "}";

            }
            else
            {
             
                foreach (var e in Changelog)
                {
                    var version = e.Key;
                    var changelog = e.Value;
                    ret += version + "\n" + changelog + "\n\n";
                }
            }
            return ret;
        }
    }
}