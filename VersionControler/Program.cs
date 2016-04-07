using System;
using System.IO;
using System.Linq;

namespace VersionControler
{
    class Program
    {
        private static string[] usage = new[]
        {
            "VersionControler.exe FileToEdit lineNumber [replacementPattern]",
            "FileToEdit - Full path to file",
            "lineNumber - Line number to edit, first line being 0 ",
            "[replacementPattern] - string.Format target ({0} being version string) "
        };
        /// <summary>
        /// Usage:
        /// VersionControler.exe FileToEdit lineNumber [replacementPattern]
        /// FileToEdit - Full path to file
        /// lineNumber - Line number to edit, first line being 0 
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(string.Join(Environment.NewLine, usage));
                return;
            }
            string replacementPattern = "{0}=\"v{1}\";";
            bool custom = false;
            if (args.Length == 3)
            {
                replacementPattern = args[2];
                custom = true;
            }
            string filePath = args[0];
            int lineNumber = Convert.ToInt32(args[1]);

            if (File.Exists(filePath))
            {
                String[] lines = File.ReadAllLines(filePath);
                DateTime d = DateTime.Now;
                string version = d.ToString("yyMMdd.HH");
                if (!custom)
                {
                    var splited = lines[lineNumber].Split(new[] { '=' }, 2);
                    lines[lineNumber] = string.Format(replacementPattern, splited[0], version);
                }
                else
                {
                    lines[lineNumber] = string.Format(replacementPattern, version);
                }
                Console.WriteLine("Changed version to: " + version);

                File.WriteAllLines(filePath, lines);
            }
            else
                Console.WriteLine("File was not found.");
        }
    }
}
