using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace laiatech_wpf
{
    class IniFile
    {
        string Path; // File name

        [DllImport("kernel32")] // We include kernel32.dll and describe its function WritePrivateProfilesString
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")] // We connect kernel32.dll again, and now we describe the function GetPrivateProfileString
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        // Using the constructor, write down to the file and its name.
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }

        // We read the ini file and return the value of the specified key from the specified section.
        public string ReadINI(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }
        // We write to the ini file. Recording takes place in the selected section in the selected key.
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        // We remove the key from the selected section.
        public void DeleteKey(string Key, string Section = null)
        {
            Write(Section, Key, null);
        }
        // Delete the selected section
        public void DeleteSection(string Section = null)
        {
            Write(Section, null, null);
        }
        // Check if there is such a key in this section
        public bool KeyExists(string Key, string Section = null)
        {
            return ReadINI(Section, Key).Length > 0;
        }
    }
}
