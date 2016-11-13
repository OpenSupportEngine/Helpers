using System.Reflection;

namespace OpenSupportEngine.Helpers.Resources
{
    public sealed class ResourceDescriptor
    {
        public Assembly ResourceAssembly { get; set; }
        public string FullResourcePath { get; set; }
    }
}
