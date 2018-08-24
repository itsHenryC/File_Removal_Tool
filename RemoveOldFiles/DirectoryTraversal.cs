using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveOldFiles
{
    public class DirectoryTraversal
    {
        public static List<string> TraverseFolders(string root)
        {
            // Data structure to hold names of subfolders to be
            // examined for files.
            List<string> allDirectories = new List<string>();
            Stack<string> dirs = new Stack<string>();

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.

                foreach (string str in subDirs)
                {
                    dirs.Push(str);
                    allDirectories.Add(str);
                }                
            }
            return allDirectories;
        }


        public static void deleteDirectories(List<string> Directories, CommandLineOptions settings, ref int deletedCounter, ref int failedDeletedCounter)
        {
            if (Directories == null)
            { return; }
            var sortedDirectories = new SortedList<int, List<string>>();

            foreach (var folder in Directories)
            {
                var pathCount = folder.Split('\\').Length;
                if (!sortedDirectories.ContainsKey(pathCount))
                {
                    sortedDirectories.Add(pathCount, new List<string>());
                }
                sortedDirectories[pathCount].Add(folder);
            }

            Boolean isFolderHidden, isFolderSysFlagged;

            foreach (var directoriesPaths in sortedDirectories.Reverse())
            {
                foreach (var directories in directoriesPaths.Value)
                {
                    if (IsDirectoryEmpty(directories))
                    {
                        try
                        {
                            DirectoryInfo folderInfo = new DirectoryInfo(directories);
                            isFolderHidden = (hasFlag(folderInfo, FileAttributes.Hidden));
                            isFolderSysFlagged = (hasFlag(folderInfo, FileAttributes.System));
                            Boolean deleteFile;
                            deleteFile = (!isFolderHidden || settings.CheckHiddenFiles);
                            deleteFile &= (!isFolderSysFlagged || settings.CheckSystemFlag);

                            if (deleteFile)
                            {
                                if (settings.VerboseDeletedItems == true)
                                {
                                    Console.WriteLine(directories);
                                }
                                Directory.Delete(directories);
                                deletedCounter += 1;
                            }
                        }
                        catch
                        {
                            failedDeletedCounter += 1;
                            if (settings.VerboseDeletedItems == true)
                            {
                                Console.WriteLine("ERROR: Unable to delete folder.");
                            }
                        }
                    }
                }
            }
        }

        static bool IsDirectoryEmpty(string path)
        {
            string[] dirs = System.IO.Directory.GetDirectories(path); string[] files = System.IO.Directory.GetFiles(path);
            return dirs.Length == 0 && files.Length == 0;
        }
  
        static Boolean hasFlag(DirectoryInfo folderInfo, FileAttributes flag)
        {
            return (folderInfo.Attributes & flag) > 0;
        }
    }
}
