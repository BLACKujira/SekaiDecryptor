using System;
using System.Collections.Generic;
using System.IO;

namespace SekaiDecryptor
{
    class Program
    {
        public static readonly byte[] head = { 0x10, 0x00, 0x00, 0x00, 0xAA, 0x91, 0x96, 0x8B, 0x86, 0x46, 0x53 };
        static void Main(string[] args)
        {
            string savePath, assetPath;
            Console.WriteLine("input asset path");
            assetPath = Console.ReadLine().Trim(' ','"' );
            Console.WriteLine("input save path");
            savePath = Console.ReadLine().Trim(' ', '"');
            List<string> paths = new List<string>();
            GetPaths(paths, assetPath);
            MainProcess(paths, savePath, assetPath);
        }
        public static bool IfSekaiAsset(byte[] file)
        {
            for (int i = 0; i < head.Length; i++)
            {
                if (file[i] != head[i]) return false;
            }
            return true;
        }
        public static void GetPaths(List<string> paths,string searchPath)
        {
            paths.Add(searchPath);
            string[] withPath = Directory.GetDirectories(searchPath);
            foreach (var item in withPath)
            {
                GetPaths(paths, item);
            }
        }
        public static void MainProcess(List<string> paths,string savePath,string originPath)
        {
            foreach (var path in paths)
            {
                string[] files = Directory.GetFiles(path);
                if(files.Length != 0&& path != originPath)
                {
                    string saveFilePath = path;
                    saveFilePath = saveFilePath.Substring(originPath.Length);
                    saveFilePath = savePath + saveFilePath;
                    Directory.CreateDirectory(saveFilePath);
                }
                foreach (var file in files)
                {
                    byte[] input = File.ReadAllBytes(file);
                    if (input.Length <= 4+128) continue;
                    {
                        if (!IfSekaiAsset(input)) { continue; }
                        string saveFile = file;
                        if (saveFile != originPath) saveFile = saveFile.Substring(originPath.Length);
                        saveFile = savePath + saveFile;
                        if (File.Exists(saveFile)) continue;
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (j < 5) input[4+i*8+j] = (byte)~input[4+i * 8 + j];
                                }
                            }
                            FileStream fileStream = File.OpenWrite(saveFile);
                            fileStream.Write(input, 4, input.Length - 4);
                            fileStream.Close();
                        }
                        Console.WriteLine(saveFile);
                    }
                }
            }
        }
    }
}
