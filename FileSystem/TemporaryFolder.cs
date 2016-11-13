using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSupportEngine.Helpers.FileSystem
{
    public sealed class TemporaryFolder : IDisposable
    {
        public const string LockFileName = "lockfile";

        public static string FolderPrefix { get; set; }
        private static string TempFolderPrefix
        {
            get
            {
                return string.Format("{0}-TEMP.", FolderPrefix);
            }
        }
        public string DirectoryPath { get; private set; }
        public bool Disposed { get; private set; }

        private static List<TemporaryFolder> currentFolders =
            new List<TemporaryFolder>();
        private static object currentFoldersLock =
            new object();

        private FileStream lockStream;

        public TemporaryFolder()
        {
            if (string.IsNullOrEmpty(FolderPrefix))
                throw new NullReferenceException();

            do
            {
                var folderName = TempFolderPrefix + Path.GetRandomFileName();
                DirectoryPath = Path.Combine(Path.GetTempPath(), folderName);
            } while (Directory.Exists(DirectoryPath));

            Directory.CreateDirectory(DirectoryPath);
            lockStream = File.Open(
                Path.Combine(DirectoryPath, LockFileName),
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None
                );

            InvokeOnCurrentFoldersList(list => list.Add(this));
        }

        ~TemporaryFolder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            InvokeOnCurrentFoldersList(list => list.Remove(this));
            Disposed = true;

            if (disposing)
            {
                lockStream.Dispose();
            }
            RemoveFolderRecursively(DirectoryPath);
        }

        public static bool CleanUp()
        {
            if (!string.IsNullOrEmpty(FolderPrefix))
            {
                var path = Path.GetTempPath();
                var filter = string.Format("{0}*", TempFolderPrefix);

                CleanUpCurrentFolders();

                var dirs = Directory.EnumerateDirectories(path, filter);
                foreach (var dir in dirs)
                {
                    if (!RemoveFolderRecursively(dir))
                        return false;
                }

                return true;
            }

            return false;
        }

        private static void CleanUpCurrentFolders()
        {
            InvokeOnCurrentFoldersList(list =>
            {
                currentFolders.ForEach(f => f.Dispose());
                currentFolders.Clear();
            });
        }

        private static void InvokeOnCurrentFoldersList(Action<List<TemporaryFolder>> action)
        {
            lock (currentFoldersLock)
            {
                action(currentFolders);
            }
        }

        private static bool RemoveFolderRecursively(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
