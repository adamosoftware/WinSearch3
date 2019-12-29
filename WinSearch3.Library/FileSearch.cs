using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WinSearch3.Library
{
    public class FileSearch
    {
        public string Locations { get; set; }
        public string SearchFor { get; set; }
        public string Extensions { get; set; }
        public string Contents { get; set; }

        public async Task<IEnumerable<string>> ExecuteAsync(IProgress<string> progress =  null)
        {
            var locations = SplitAndTrim(Locations);
            var extensions = SplitAndTrim(Extensions);

            if (!extensions.Any()) extensions = new string[] { "*" };

            var results = new List<string>();

            foreach (var loc in locations)
            {
                await Task.Run(() =>
                {
                    ExecuteInner(loc, extensions, results, progress);
                });
            }

            return results;
        }

        private void ExecuteInner(string path, IEnumerable<string> extensions, List<string> results, IProgress<string> progress = null)
        {
            progress?.Report(path);

            if (IsFileSearch())
            {
                foreach (var ext in extensions)
                {
                    var files = TryGetFiles(path, ext);
                    foreach (var fileName in files)
                    {
                        if (IsMatch(fileName)) results.Add(fileName);
                    }
                }
            }

            var folders = TryGetDirectories(path);
            foreach (var subFolder in folders)
            {
                if (IsFolderSearch() && subFolder.Contains(SearchFolderName()))
                {
                    results.Add(subFolder);
                }

                ExecuteInner(subFolder, extensions, results, progress);
            }
        }

        private string SearchFolderName()
        {
            return (SearchFor.EndsWith("\\") || SearchFor.EndsWith("/")) ? SearchFor.Substring(0, SearchFor.Length - 1) : SearchFor;
        }

        private bool IsFolderSearch()
        {
            return !IsFileSearch();
        }

        private bool IsFileSearch()
        {
            return !SearchFor.EndsWith("\\") && !SearchFor.EndsWith("/");
        }

        private bool IsMatch(string fileName)
        {
            if (fileName.Contains(SearchFor)) return true;

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
