using NDesk.Options;
using System;

namespace RemoveOldFiles
{
    public class CommandLineOptions
    {
        public string RootDirectory { get; set; }
        private string RemovalDays { get; set; }
        public DateTime RemovalDate { get; set; }
        public Boolean CheckHiddenFiles { get; set; }
        public Boolean CheckReadOnly { get; set; }
        public Boolean CheckSystemFlag { get; set; }
        public Boolean VerboseDeletedItems { get; set; }
        private Boolean ShowHelpMenu { get; set; }
        public CommandLineOptions()
        {
            RootDirectory = null;
            RemovalDays = null;
            CheckHiddenFiles = false;
            CheckReadOnly = false;
            CheckSystemFlag = false;
            VerboseDeletedItems = false;
            ShowHelpMenu = false;
        }
        public CommandLineOptions(string directory, string checkDate, Boolean checkHidden = false, Boolean checkRead = false, Boolean checkSystem = false, Boolean verbose = false)
        {
            RootDirectory = directory;
            RemovalDays = checkDate;
            CheckHiddenFiles = checkHidden;
            CheckReadOnly = checkRead;
            CheckSystemFlag = checkSystem;
            VerboseDeletedItems = verbose;
            ShowHelpMenu = false;
        }

        public Boolean CommandLineOptionSet(string[] args)
        {
            var options = new OptionSet(){
                {"d|rootDir=","Sets root directory.", v => RootDirectory = v},
                {"n|day=","Sets the number of days from current\n date for files to be deleted. (Creation Date)", v => RemovalDays = v },
                {"force","Forces on for all files/folders.",  v => {
                    CheckHiddenFiles = v != null;
                    CheckReadOnly = v != null;
                    CheckSystemFlag = v != null;
                    }
                },
                {"h|forceHidden","Forces on for hidden folders/files.", v => CheckHiddenFiles = v != null},
                {"r|forceReadOnly","Forces on for read-only folders/files.", v => CheckReadOnly = v != null},
                {"s|forceSystemFlag","Forces on for system-flagged folders/files.", v => CheckSystemFlag = v != null},
                {"v|verbose","Logs all the deleted files and folder.", v => VerboseDeletedItems = v != null },
                {"help","Show this message and exit.", help => ShowHelpMenu = help != null},
            }; 
            options.Parse(args);
            if (Convert.ToInt32(this.RemovalDays) >= 0)
            {
                RemovalDate = convertToDateTime(this.RemovalDays);
            }
            if (this.ShowHelpMenu == true)
            {
                ShowHelp(options);
            }
            return this.ShowHelpMenu;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: [OPTIONS] + input");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        public static DateTime convertToDateTime(string days)
        {
            DateTime cleanedDateTime = new DateTime();
            cleanedDateTime = DateTime.Now.AddDays(Convert.ToInt32(days) * -1);

            return cleanedDateTime;
        }
    }
}
