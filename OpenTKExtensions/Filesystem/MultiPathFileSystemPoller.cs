using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        public long PollIntervalMilliseconds { get; set; } = 2000;

        public string FileSpec { get; set; } = "*.*";

        private long previousHash = 0;
        private long currentHash = 0;
        private Stopwatch updateTimer = Stopwatch.StartNew();
        private long lastPollTimeMilliseconds = 0;

        public MultiPathFileSystemPoller(params string[] paths)
        {
            Paths.AddRange(paths);
        }

        public MultiPathFileSystemPoller()
            : this(Path.GetFullPath("."))
        {
        }

        public bool Poll()
        {
            if (updateTimer.ElapsedMilliseconds - lastPollTimeMilliseconds > PollIntervalMilliseconds)
            {
                lastPollTimeMilliseconds = updateTimer.ElapsedMilliseconds;
                PollImmediate();
                return HasChanges;
            }
            return false;            
        }

        public void PollImmediate()
        {

            currentHash =
                Paths.SelectMany(p => Directory.EnumerateFiles(p, FileSpec, SearchOption.AllDirectories))
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
