using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laiatech_wpf
{
    public class Profile
    {
        public String title = "";
        public int expo = 0;
        public int gain = 0;
        public int bright = 0;
        public int cont = 0;
        public int satur = 0;
        public int sharp = 0;
        public int gamma = 0;
        public int whiteBal = 0;
        public int focus = 0;
        public int zoom = 0;
        public int pan = 0;
        public int tilt = 0;

        public Profile()
        {

        }

        public Profile(String pStr)
        {
            char[] separator = { '*' };
            String[] words = pStr.Split(separator);

            if (words.Length != 13)
                return;

            title = words[0];
            expo = int.Parse(words[1]);
            gain = int.Parse(words[2]);
            bright = int.Parse(words[3]);
            cont = int.Parse(words[4]);
            satur = int.Parse(words[5]);
            sharp = int.Parse(words[6]);
            gamma = int.Parse(words[7]);
            whiteBal = int.Parse(words[8]);
            focus = int.Parse(words[9]);
            zoom = int.Parse(words[10]);
            pan = int.Parse(words[11]);
            tilt = int.Parse(words[12]);
        }

        public String toString()
        {
            return String.Format("{0}*{1}*{2}*{3}*{4}*{5}*{6}*{7}*{8}*{9}*{10}*{11}*{12}",
                title, expo, gain, bright, cont, satur, sharp, gamma, whiteBal, focus, zoom, pan, tilt);
        }
    }
}
