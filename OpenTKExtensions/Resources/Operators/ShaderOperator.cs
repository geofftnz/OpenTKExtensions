using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    /// <summary>
    /// A Shader Operator. (Not sure at this stage whether this should be a resource or component... maybe a component since it encapsulates resources.
    /// 
    /// Basically a shader program combined with a GBuffer, making an encapsulated part of a rendering pipeline.
    /// 
    /// By default it hosts its own output textures, which can then be retrieved to pass as inputs to a subsequent stage.
    /// However textures can also be supplied to write to.
    /// 
    /// The internal shader program can be reloaded on demand.
    /// 
    /// </summary>
    public class ShaderOperatorResource : ResourceBase, IResource, IReloadableResource
    {
        //private ReloadableResource<ShaderProgram> Shader;
        //private GBuffer OutputBuffer;

        public ShaderOperatorResource(string name) : base(name)
        {

        }

        public void Load()
        {
            //Shader.Load();
        }

        public bool TryReload(out string message)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
