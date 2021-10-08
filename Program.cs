using System;
using System.Collections.Generic;
using System.IO;

namespace SekaiDecryptor
{
    class Program
    {
        public const int head = 16;
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
        public static bool IfSekaiAsset(BinaryReader binaryReader)
        {
            long fileHead = binaryReader.ReadInt32();
            if (fileHead == head)
                return true;
            else
                return false;
        }
        public static void GetPaths(List<string> paths,string searchPath)
        {
            string[] withPath = Directory.GetDirectories(searchPath);
            foreach (var item in withPath)
            {
                paths.Add(item);
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
                    FileStream input = File.OpenRead(file);
                    if (input.Length <= 4) continue;
                    using (BinaryReader oldFile = new BinaryReader(input))
                    {
                        if (!IfSekaiAsset(oldFile)) { continue; }
                        string saveFile = file;
                        if (saveFile != originPath) saveFile = saveFile.Substring(originPath.Length);
                        saveFile = savePath + saveFile;
                        if (File.Exists(saveFile)) continue;
                        FileStream newFile = File.Create(saveFile);
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (j < 5) newFile.WriteByte((byte)~oldFile.ReadByte());
                                    else newFile.WriteByte(oldFile.ReadByte());
                                }
                            }
                            byte[] bytes = oldFile.ReadBytes((int)oldFile.BaseStream.Length - 128 - 4);
                            newFile.Write(bytes,0, bytes.Length);
                        }
                        Console.WriteLine(saveFile);
                    }
                }
            }
        }
    }
}
