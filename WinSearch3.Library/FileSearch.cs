using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WinSearch3.Library
{
    public class FileSearch
    {
        public string Locations { get; set; }
        public string SearchFilename { get; set; }
        public string Extensions { get; set; }
        public string Contents { get; set; }

        public TimeSpan Elapsed { get; private set; }
        public int FilesSearched { get; private set; }
        public int FoldersSearched { get; private set; }

        public async Task<IEnumerable<string>> ExecuteAsync(IProgress<string> progress =  null)
        {
            var locations = SplitAndTrim(Locations);
            var extensions = ApplyMaskPattern(SplitAndTrim(Extensions));

            if (!extensions.Any()) extensions = new string[] { "*" };

            var results = new List<string>();

            var sw = Stopwatch.StartNew();

            foreach (var loc in locations)
            {
                await Task.Run(async () =>
                {
                    await ExecuteInner(loc, extensions, results, progress);
                });
            }

            sw.Stop();
            Elapsed = sw.Elapsed;

            return results;
        }

        private IEnumerable<string> ApplyMaskPattern(IEnumerable<string> enumerable)
        {
            return enumerable.Select(s =>
            {
                string result = s;
                if (!result.StartsWith("*.")) result = "*." + result;
                return result;
            });
        }

        private async Task ExecuteInner(string path, IEnumerable<string> extensions, List<string> results, IProgress<string> progress = null)
        {
            progress?.Report(path);

            if (IsFileSearch())
            {
                foreach (var ext in extensions)
                {
                    var files = TryGetFiles(path, ext);
                    FilesSearched += files.Count();
                    foreach (var fileName in files)
                    {
                        if (await IsMatchAsync(fileName)) results.Add(fileName);
                    }
                }
            }

            var folders = TryGetDirectories(path);
            FoldersSearched += folders.Count();
            foreach (var subFolder in folders)
            {
                if (IsFolderSearch() && subFolder.Contains(SearchFolderName()))
                {
                    results.Add(subFolder);
                }

                await ExecuteInner(subFolder, extensions, results, progress);
            }
        }

        private string SearchFolderName()
        {
            return (SearchFilename.EndsWith("\\") || SearchFilename.EndsWith("/")) ? SearchFilename.Substring(0, SearchFilename.Length - 1) : SearchFilename;
        }

        private bool IsFolderSearch()
        {
            return !IsFileSearch();
        }

        private bool IsFileSearch()
        {
            return !SearchFilename.EndsWith("\\") && !SearchFilename.EndsWith("/");
        }

        private async Task<bool> IsMatchAsync(string fileName)
        {
            if (fileName.ToLower().Contains(SearchFilename.ToLower())) return true;

            if (!string.IsNullOrEmpty(Contents))
            {
                using (var file = File.OpenRead(fileName))
                {
                    using (var reader = new StreamReader(file))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = await reader.ReadLineAsync();
                            if (line.Contains(Contents)) return true;
                        }                        
                    }
                }
            }

            return false;
        }

        private IEnumerable<string> TryGetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch 
            {
                return Enumerable.Empty<string>();
            }
        }

        private IEnumerable<string> TryGetFiles(string path, string pattern)
        {
            try
            {
                return Directory.GetFiles(path, pattern);
            }
            catch 
            {
                return Enumerable.Empty<string>();
            }
        }

        private static IEnumerable<string> SplitAndTrim(string input)
        {
            return input?.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()) ?? Enumerable.Empty<string>();
        }
    }
}
