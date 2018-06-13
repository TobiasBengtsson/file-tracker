using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FileTracker
{
    class Program
    {
        [Verb("index", HelpText = "Indexes files in folder")]
        private class IndexOptions
        {
            [Option('p', "path", Default = ".", HelpText = "Path of folder to index")]
            public string FolderPath { get; set; }

            [Option('o', "output", Default = null, HelpText = "Path to output file")]
            public string OutputFilePath { get; set; }

            [Option('r', "recursive", Default = false, HelpText = "Include subfolders")]
            public bool Recursive { get; set; }
        }

        [Verb("clean", HelpText = "Cleans files from folder")]
        private class CleanOptions
        {
            [Option('d', "dryRun", Default = false, HelpText = "Perform a dry run, printing out the files that would be deleted")]
            public bool DryRun { get; set; }

            [Option('p', "path", Default = ".", HelpText = "Path of folder to clean")]
            public string FolderPath { get; set; }

            [Option('i', "input", Required = true, HelpText = "The path of the file containing the index")]
            public string InputFilePath { get; set; }

            [Option('r', "recursive", Default = false, HelpText = "Include subfolders")]
            public bool Recursive { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<IndexOptions, CleanOptions>(args)
                .MapResult(
                    (IndexOptions opts) => RunIndexAndReturnExitCode(opts),
                    (CleanOptions opts) => RunCleanAndReturnExitCode(opts),
                    errs => 1);
        }

        private static int RunIndexAndReturnExitCode(IndexOptions opts)
        {
            if (!Directory.Exists(opts.FolderPath))
            {
                Console.Error.WriteLine($"Directory '{opts.FolderPath}' not found.");
                return 1;
            }

            var files = GetFilePaths(opts.FolderPath, opts.Recursive);

            var output = opts.OutputFilePath == null ? Console.Out : File.AppendText(opts.OutputFilePath);

            foreach (var file in files)
            {
                output.WriteLine($"{GetFileHash(file)}\t{file}");
            }

            output.Close();
            return 0;
        }

        private static int RunCleanAndReturnExitCode(CleanOptions opts)
        {
            if (!File.Exists(opts.InputFilePath))
            {
                Console.Error.WriteLine($"File '{opts.InputFilePath}' not found.");
                return 1;
            }

            if (!Directory.Exists(opts.FolderPath))
            {
                Console.Error.WriteLine($"Directory '{opts.FolderPath}' not found.");
                return 1;
            }

            var indexedFilesHashes = GetFileHashesFromIndexFile(opts.InputFilePath);

            var files = GetFilePaths(opts.FolderPath, opts.Recursive);
            foreach (var file in files.Where(f => indexedFilesHashes.Contains(GetFileHash(f))))
            {
                if (opts.DryRun)
                    Console.WriteLine(file);
                else
                    File.Delete(file);
            }

            return 0;
        }

        private static HashSet<string> GetFileHashesFromIndexFile(string inputFilePath) =>
            File.ReadLines(inputFilePath).Select(line => line.Split('\t')[0]).ToHashSet();

        private static IEnumerable<string> GetFilePaths(string folderPath, bool recursive) =>
            recursive ?
                Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories) :
                Directory.EnumerateFiles(folderPath);

        private static string GetFileHash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
