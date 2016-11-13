using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenSupportEngine.Helpers.Resources
{
    public sealed class ResourceDescriptor
    {
        public Assembly ResourceAssembly { get; set; }
        public string FullResourcePath { get; set; }
    }
}
