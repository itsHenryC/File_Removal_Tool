using System;
using System.Collections.Generic;
using System.IO;
namespace RemoveOldFiles
{
    public class FileTraversal
    {
        public static List<string> TraverseFiles(string root)
        {
            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>();

            dirs.Push(root);

            List<string> allFiles = new List<string>();
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

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

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

                foreach (string file in files)
                {
                        allFiles.Add(file);                  
                }
                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
            return allFiles;
        }

        public static void DeleteFile(List<string> allFiles, CommandLineOptions settings, ref int deletedCounter, ref int failedDeletedCounter)
        {
            if (allFiles == null)
            { return; }

            foreach (var file in allFiles)
            {
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                    Boolean isTimeOld, isFileHidden, isFileReadOnly, isFileSysFlagged, deleteFile;

                    isTimeOld = fileInfo.LastAccessTime < settings.RemovalDate;
                    if (!isTimeOld)
                    {
                        return;
                    }

                    isFileHidden = (hasFlag(fileInfo, FileAttributes.Hidden));
                    isFileReadOnly = (fileInfo.IsReadOnly);
                    isFileSysFlagged = (hasFlag(fileInfo, FileAttributes.System));
                    deleteFile = (!isFileHidden || settings.CheckHiddenFiles);
                    deleteFile &= (!isFileReadOnly || settings.CheckReadOnly);
                    deleteFile &= (!isFileSysFlagged || settings.CheckSystemFlag);
                    if (deleteFile)
                    {
                        if (settings.VerboseDeletedItems == true)
                        {
                            Console.WriteLine(file);
                        }
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                        deletedCounter += 1;
                    }
                }
                catch (Exception e)
                {
                    failedDeletedCounter += 1;
                    if (settings.VerboseDeletedItems == true)
                    {
                        Console.WriteLine("ERROR: Unable to delete file, reason: {0}", e.Message);
                    }
                }
            }
        }

        public static Boolean hasFlag(FileInfo fileInfo, FileAttributes flag)
        {
            return (fileInfo.Attributes & flag) > 0;
        }
    }
}
