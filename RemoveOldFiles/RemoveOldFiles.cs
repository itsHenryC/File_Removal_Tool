using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace RemoveOldFiles
{
    class RemoveOldFiles
    {
        static void Main(string[] args)
        {
            Boolean adminStatus = CheckRunAsAdmin();
            if (adminStatus)
            {
                CommandLineOptions settings = new CommandLineOptions();
                Boolean helpMenu = settings.CommandLineOptionSet(args);
                if (!helpMenu)
                {
                    int deletedFoldersCount = 0;
                    int deletedFilesCount = 0;
                    int failedFoldersCount = 0;
                    int failedFilesCount = 0;

                    try
                    {
                        if (!System.IO.Directory.Exists(settings.RootDirectory))
                        {
                            Console.WriteLine("ERROR: The Directory: \"{0}\" does not exist.", settings.RootDirectory);
                            return;
                        }
                        List<string> allFiles = FileTraversal.TraverseFiles(settings.RootDirectory);
                        FileTraversal.DeleteFile(allFiles, settings, ref deletedFilesCount, ref failedFilesCount);

                        List<string> allDirectories = DirectoryTraversal.TraverseFolders(settings.RootDirectory);
                        DirectoryTraversal.deleteDirectories(allDirectories, settings, ref deletedFoldersCount, ref failedFoldersCount);

                        Console.WriteLine("Successfully removed:");
                        Console.WriteLine("  Files:   " + deletedFilesCount);
                        Console.WriteLine("  Folders: " + deletedFoldersCount);
                        if (failedFilesCount > 0 || failedFoldersCount > 0)
                        {
                            Console.WriteLine("Failed to remove:");
                            if (failedFilesCount > 0)
                                Console.WriteLine("  Files:   " + failedFilesCount);
                            if (failedFoldersCount > 0)
                                Console.WriteLine("  Folders: " + failedFoldersCount);
                        }
                    }
                    catch (System.IO.PathTooLongException)
                    {
                        Console.WriteLine("The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Executable must be run as Administrator.");
            }
        }

        static Boolean CheckRunAsAdmin()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
          .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
