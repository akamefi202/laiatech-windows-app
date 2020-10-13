using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laiatech_wpf
{
    public class ProfileManager
    {
        public List<Profile> profileList = new List<Profile>();
        public Profile selectedProfile = null;
        public String iniFilePath = "config.ini";

        public ProfileManager()
        {
            readIniFile();
        }

        public void addProfile(Profile p)
        {
            profileList.Add(p);
            writeIniFile();
        }

        public void updateProfile(Profile p)
        {
            int selectedIndex = -1;
            for (int i = 0; i < profileList.Count; i ++)
            {
                if (profileList[i].title == p.title)
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                profileList[selectedIndex] = p;
                selectedProfile = p;
            }

            writeIniFile();
        }

        public void readIniFile()
        {
            if (!File.Exists(iniFilePath))
                File.Create(iniFilePath);

            String[] lines = File.ReadAllLines(iniFilePath);

            foreach (String line in lines)
            {
                Profile pp = new Profile(line);
                profileList.Add(pp);
            }
        }

        public void writeIniFile()
        {
            if (!File.Exists(iniFilePath))
                File.Create(iniFilePath);

            List<String> lines = new List<String>();

            foreach (Profile p in profileList)
                lines.Add(p.toString());

            File.WriteAllLines(iniFilePath, lines);
        }
    }
}
