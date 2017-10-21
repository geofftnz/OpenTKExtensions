using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public interface IReloadableResource
    {
        bool TryReload(out string message);
    }
}
