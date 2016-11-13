using System.IO;

namespace OpenSupportEngine.Helpers.Resources
{
    public static class ResourceOperations
    {
        public static bool SaveToFileSystem(ResourceDescriptor resource, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    using (var stream = resource.ResourceAssembly.GetManifestResourceStream(resource.FullResourcePath))
                    {
                        var buffer = new byte[8 * 1024];
                        var index = 0;
                        do
                        {
                            index = stream.Read(buffer, 0, buffer.Length);
                            fileStream.Write(buffer, 0, index);
                        } while (index > 0);
                    }
                }
            }
            catch
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return false;
            }

            return true;
        }
    }
}
