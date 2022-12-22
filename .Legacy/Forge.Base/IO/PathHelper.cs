/* *********************************************************************
 * Date: 13 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Logging;
using System;
using System.IO;

namespace Forge.IO
{

    /// <summary>
    /// Helper method(s)
    /// </summary>
    public static class PathHelper
    {

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(PathHelper));

        /// <summary>
        /// Performs the folder security test.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>True, if the security test passed, otherwise False.</returns>
        /// <example>
        /// <code>
        /// if (!PathHelper.PerformFolderSecurityTest(value))
        /// {
        ///     throw new SecurityException(string.Format("Provided folder must have read, write and delete rights. Folder: {0}", value));
        /// }
        /// </code>
        /// </example>
        public static bool PerformFolderSecurityTest(String path)
        {
            if (path == null)
            {
                ThrowHelper.ThrowArgumentNullException("path");
            }

            DirectoryInfo di = new DirectoryInfo(path);
            bool result = true;
            try
            {
                if (!di.Exists)
                {
                    di.Create();
                }
                FileInfo testFile = new FileInfo(Path.Combine(path, "test.txt"));
                if (testFile.Exists)
                {
                    testFile.Delete();
                }
                testFile.Create().Dispose();
                testFile.Delete();
            }
            catch (Exception ex)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(ex.Message, ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Determines whether the path is an absolute path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the path is absolute path, otherwise <c>false</c>.
        /// </returns>
        public static bool IsAbsolutePath(string path)
        {
            if (path == null)
            {
                ThrowHelper.ThrowArgumentNullException("path");
            }
            return path.Length > 1 && path.Substring(1, 1).Equals(":");
        }

        /// <summary>
        /// Cutoffs the backslash from path end.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// Path with backslash at the end
        /// </returns>
        /// <example>
        ///   <code>
        /// string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// string systemFolderWithBackslash = string.Format("{0}\\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        /// Assert.IsTrue(systemFolder.Equals(PathHelper.CutoffBackslashFromPathEnd(systemFolder)));
        /// Assert.IsTrue(systemFolder.Equals(PathHelper.CutoffBackslashFromPathEnd(systemFolderWithBackslash)));
        ///   </code>
        ///   </example>
        public static string CutoffBackslashFromPathEnd(string path)
        {
            if (path == null)
            {
                ThrowHelper.ThrowArgumentNullException("path");
            }

            string result = path;
            if (path.EndsWith("\\"))
            {
                result = path.Substring(0, path.Length - 1);
            }
            return result;
        }

        /// <summary>Cutoffs the last entry from path.</summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string CutoffLastEntryFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                ThrowHelper.ThrowArgumentNullException("path");
            }

            string result = "";
            string[] entries = path.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            if (entries.Length == 0)
            {
                result = @"\";
            }
            else if (entries.Length == 1)
            {
                result = path;
            }
            else
            {
                result = entries[0];
                for (int i = 1; i < entries.Length - 1; i++)
                {
                    result = Path.Combine(result, entries[i]);
                }
                if (result.EndsWith(":")) result = string.Format("{0}\\", result);
            }

            return result;
        }

        /// <summary>
        /// Resolves the environment special folder.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The resolved path</returns>
        /// <example>
        /// <code>
        /// string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// string path = string.Format("${0}\\test.docx", Environment.SpecialFolder.MyDocuments.ToString());
        ///
        /// string expectedResult = string.Format("{0}\\test.docx", systemFolder);
        /// Assert.IsTrue(expectedResult.Equals(PathHelper.ResolveEnvironmentSpecialFolder(path)));
        /// </code>
        /// </example>
        public static string ResolveEnvironmentSpecialFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                ThrowHelper.ThrowArgumentNullException("path");
            }

            string folder = path;
            foreach (string specialFolder in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                string searchPattern = string.Format("${0}", specialFolder);
                if (path.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase))
                {
                    folder = folder.Substring(searchPattern.Length, folder.Length - searchPattern.Length);
                    if (folder.StartsWith("\\"))
                    {
                        folder = folder.Substring(1);
                    }
                    folder = Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolder)), folder);
                    break;
                }
            }

            return folder;
        }

    }

}
