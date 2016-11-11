using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Filesystem
{
    /// <summary>
    /// Simple on-demand notification if file modification dates have changed in a directory.
    /// Useful for shader reloading etc.
    /// </summary>
    public class FileSystemPoller
    {
        public string Path { get; set; }
        public bool HasChanges { get { return previousHash != currentHash; } }

        private long previousHash = 0;
        private long currentHash = 0;

        public FileSystemPoller(string path)
        {
            this.Path = path;
        }
        public FileSystemPoller()
            : this(System.IO.Path.GetFullPath("."))
        {
        }

        public void Poll()
        {
            currentHash =
                Directory
                .EnumerateFiles(Path, @"*.*", SearchOption.AllDirectories)
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
