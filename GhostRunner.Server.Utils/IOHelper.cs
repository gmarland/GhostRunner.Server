using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Utils
{
    public class IOHelper
    {
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }

        public static String GetPackageVersion(String packageDirectory)
        {
            if ((Directory.Exists(packageDirectory)) && (File.Exists(packageDirectory.TrimEnd(new char[] { '\\'}) + "\\package.json")))
            {
                String[] packageContent = File.ReadAllLines(packageDirectory.TrimEnd(new char[] { '\\'}) + "\\package.json");

                String[] versionLines = packageContent.Where(pc => pc.Trim().ToLower().Contains("\"version\"")).ToArray();

                if (versionLines.Length > 0)
                {
                    String[] versionParts = versionLines.First().Replace("\"", String.Empty).Split(new char[] { ':' });

                    if (versionParts.Length == 2) return versionParts.Last().TrimEnd(new char[] { ',' }).Trim();
                    else return String.Empty;
                }
                else return String.Empty;
            }
            else return String.Empty;
        }
    }
}
