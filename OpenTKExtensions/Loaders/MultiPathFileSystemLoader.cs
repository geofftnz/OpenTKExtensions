using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OpenTKExtensions.Loaders
{
    public class MultiPathFileSystemLoader : ShaderLoaderBase, IShaderLoader
    {
        public List<string> SearchPaths { get; private set; }

        public MultiPathFileSystemLoader()
            : this(".")
        {
        }

        public MultiPathFileSystemLoader(string searchPaths)
        {
            SearchPaths = new List<string>();

            SearchPaths.AddRange(searchPaths.Split(';'));
        }

        protected override string GetContent(string name)
        {
            foreach (var filepath in GetFilePaths(name))
            {
                if (File.Exists(filepath))
                    return File.ReadAllText(filepath);
            }
            throw new FileNotFoundException("MultiPathFileSystemLoader could not find file", string.Join(";", GetFilePaths(name)));
        }

        private IEnumerable<string> GetFilePaths(string fileName)
        {
            foreach (var path in SearchPaths)
                yield return Path.Combine(path, fileName);
        }

    }
}
