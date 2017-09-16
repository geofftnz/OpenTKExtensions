using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Filesystem
{
    /// <summary>
    /// Simple on-demand notification if file modification dates have changed in one or more directory.
    /// Useful for shader reloading etc.
    /// </summary>
    public class MultiPathFileSystemPoller
    {
        public List<string> Paths { get; } = new List<string>();
        public bool HasChanges { get { return previousHash != currentHash; } }

        public string FileSpec { get; set; } = "*.*";

        private long previousHash = 0;
        private long currentHash = 0;

        public MultiPathFileSystemPoller(params string[] paths)
        {
            this.Paths.AddRange(paths);
        }

        public MultiPathFileSystemPoller()
            : this(System.IO.Path.GetFullPath("."))
        {
        }

        public void Poll()
        {
            currentHash =
                Paths.SelectMany(p=> Directory.EnumerateFiles(p, FileSpec, SearchOption.AllDirectories))
                .Select(fn => Directory.GetLastWriteTimeUtc(fn))
                .Select(d => d.ToBinary() % 0x786789fc42a47)
                .Sum();

            if (previousHash == 0)
            {
                previousHash = currentHash;
            }
        }

        public void Reset()
        {
            previousHash = currentHash;
        }

    }
}
