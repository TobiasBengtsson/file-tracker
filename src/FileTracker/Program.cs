using CommandLine;
using System;
using System.IO;
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

            [Option('r', "recursive", Default = false, HelpText = "Include subfolders")]
            public bool Recursive { get; set; }

            [Option('o', "output", Default = null, HelpText = "Path to output file")]
            public string OutputFilePath { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<IndexOptions>(args)
                .MapResult(
                    (IndexOptions opts) => RunIndexAndReturnExitCode(opts),
                    errs => 1);
        }

        private static int RunIndexAndReturnExitCode(IndexOptions opts)
        {
            var files = opts.Recursive ?
                    Directory.EnumerateFiles(opts.FolderPath, "*", SearchOption.AllDirectories) :
                    Directory.EnumerateFiles(opts.FolderPath);

            var output = opts.OutputFilePath == null ? Console.Out : File.AppendText(opts.OutputFilePath);

            foreach (var file in files)
            {
                using (var stream = File.OpenRead(file))
                using (var md5 = MD5.Create())
                {
                    var hash = md5.ComputeHash(stream);
                    var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    output.WriteLine($"{hashString}\t{file}");
                }
            }

            output.Close();
            return 0;
        }
    }
}
