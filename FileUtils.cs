using Readability.Windows;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Readability
{
    public static class FileUtils
    {
        //public static bool SaveFile(FileAnalysisWindow analysisWindow)
        //{
        //    throw new NotImplementedException();
        //}

        public static bool IsValidFileName(string name, out string osDependentMessage)
        {
            osDependentMessage = "Invalid file name. Please make sure your entry ";

            //switch(Environment.OSVersion.Platform)
            //{
            //case PlatformID.Win32S:
            //case PlatformID.Win32Windows:
            //case PlatformID.Win32NT:
            //case PlatformID.WinCE:
            //case PlatformID.Xbox:
            if(name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                osDependentMessage += "does not contain any of the following characters: \\/:*?\"<>|";
                return false;
            }
            if(Regex.IsMatch(name, @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])$", RegexOptions.IgnoreCase))
            {
                osDependentMessage += "is not an OS-reserved name.";
                return false;
            }
            return true;
            //case PlatformID.Unix:
            //case PlatformID.MacOSX:
            //default:
            //    if(name.Contains(":") || name.Contains("/"))
            //    {
            //        osDependentMessage += "does not contain any of the following characters: /:";
            //        return false;
            //    }
            //    osDependentMessage = "";
            //    return true;
            //}
        }
    }
}
