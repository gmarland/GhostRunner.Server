using log4net;
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
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void DeleteDirectory(string targetDirectory)
        {
            string[] files = Directory.GetFiles(targetDirectory);
            string[] dirs = Directory.GetDirectories(targetDirectory);

            foreach (string file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    _log.Info("DeleteDirectory(): Error deleting file " + file, ex);
                }
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            try
            {
                Directory.Delete(targetDirectory, false);
            }
            catch (Exception ex)
            {
                _log.Info("DeleteDirectory(): Error deleting directory " + targetDirectory, ex);
            }
        }

        public static Boolean CopyDirectory(String sourcePath, String destinationPath)
        {
            _log.Debug("Copying from: " + sourcePath);
            _log.Debug("Copying to: " + destinationPath);

            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }
                catch (Exception ex)
                {
                    _log.Error("CopyDirectory(): Error creating directory at" + dirPath.Replace(sourcePath, destinationPath), ex);

                    DeleteDirectory(destinationPath);

                    return false;
                }
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
                catch (Exception ex)
                {
                    _log.Error("CopyDirectory(): Error copying file to " + newPath.Replace(sourcePath, destinationPath), ex);

                    DeleteDirectory(destinationPath);

                    return false;
                }
            }

            return true;
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
