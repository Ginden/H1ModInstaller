using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using DiscUtils;
using DiscUtils.Iso9660;

namespace Heroes1ModInstallator
{
    class IsoExtractor
    {
        static public void ExtractIso(string isoPath, string targetDir)
        {

            using (FileStream isoStream = File.Open(isoPath, FileMode.Open, System.IO.FileAccess.Read))
            {
                CDReader cd = new CDReader(isoStream, true);
                Stack<string> dirs = new Stack<string>(20);
                string root = @"\";
                dirs.Push(root);
                while (dirs.Count > 0)
                {

                    string currentDir = dirs.Pop();
                    Debug.WriteLine($"Scanning directory {currentDir}");
                    string tempPathDirectory = Pathy.Combine(targetDir, currentDir);
                    Directory.CreateDirectory(tempPathDirectory);
                    string[] subDirs = cd.GetDirectories(currentDir);
                    try
                    {
                        string[] files = cd.GetFiles(currentDir);
                        foreach (string file in files)
                        {
                            string normalizedFileName = file.Remove(file.Length - 2);
                            string onIsoPath = Pathy.Combine(currentDir, file);
                            string newPath = Pathy.Combine(targetDir, normalizedFileName);
                            Debug.WriteLine($"Copying from {onIsoPath} to {newPath}");
                            using (var fileStream = File.Create(newPath))
                            {
                                using (var gameFileStream = cd.OpenFile(file, FileMode.Open))
                                {
                                    gameFileStream.CopyTo(fileStream);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine(err);
                        Debug.WriteLine($"Error ({err.Message}) in {currentDir}");
                        throw;
                    }

                    foreach (string str in subDirs)
                        dirs.Push(str);
                }
            }
        }
    }
}
