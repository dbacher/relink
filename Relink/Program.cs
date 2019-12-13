using SymbolicLinkSupport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Relink
{
    class Program
    {
        static void Main(string[] args)
        {
            // Find all links            
            foreach (var arg in args)
            {
                foreach (var link in GetDirectoryLinks(arg))
                {
                    DoDirectoryCopy(link);
                }

                foreach (var link in GetFileLinks(arg))
                {
                    DoFileCopy(link);
                }
            }
        }

        private static void DoFileCopy(string link)
        {
            Console.WriteLine($"---{link}");

            var file = new FileInfo(link);
            var target = file.GetSymbolicLinkTarget();
            var tempName = GetRandomName(link);

            Console.WriteLine($" -> {link} to {target}");

            DirectoryHelpers.CopyFile(link, tempName);

            Console.WriteLine($" Deleting {link}");
            File.Delete(link);

            Console.WriteLine($" Renaming {tempName} to {link}");
            File.Move(tempName, link);
        }

        public static void DoDirectoryCopy(string link)
        {
            var dirInfo = new DirectoryInfo(link);
            var target = dirInfo.GetSymbolicLinkTarget();
            var tempName = GetRandomName(link);

            Console.WriteLine($"Found {link} ({target})");
            Console.WriteLine($"  => Copying {target} to {tempName}");

            DirectoryHelpers.CopyDirectory(target, tempName);
            Console.WriteLine();

            Console.WriteLine($"  => Deleting {link}");
            Directory.Delete(link);

            Console.WriteLine($"  => Moving {tempName} to {link}");
            Directory.Move(tempName, link);
        }


        static readonly Random rng = new Random();

        static string GetRandomName(string x)
        {
            var sb = new StringBuilder(x);

            const string allowed = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            sb.Append("-");
            for (int i = 0; i < 5; i++)
            {
                sb.Append(allowed[rng.Next(allowed.Length - 1)]);
            }

            return sb.ToString();
        }

        static IEnumerable<string> GetDirectoryLinks(string path)
        {
            var allDirs = Directory.EnumerateDirectories(path);
            foreach (var currentDir in allDirs)
            {
                var isLink = false;
                try
                {
                    var dirInfo = new DirectoryInfo(currentDir);

                    isLink = (dirInfo.IsSymbolicLink() && dirInfo.IsSymbolicLinkValid());
                }
                catch (UnauthorizedAccessException uae)
                {
                    Console.WriteLine($"*** Couldn't access {currentDir} - {uae.ToString()}");
                }

                if (isLink)
                    yield return currentDir;
            }
        }

        static IEnumerable<string> GetFileLinks(string path)
        {
            var allFiles = Directory.EnumerateFiles(path);
            foreach (var currentFile in allFiles)
            {
                var isLink = false;
                try
                {
                    var fileInfo = new FileInfo(currentFile);

                    isLink = (fileInfo.IsSymbolicLink() && fileInfo.IsSymbolicLinkValid());
                }
                catch (UnauthorizedAccessException uae)
                {
                    Console.WriteLine($"*** Couldn't access {currentFile} - {uae.ToString()}");
                }

                if (isLink)
                    yield return currentFile;
            }
        }
    }
}
