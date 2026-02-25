using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimMaruMaria
{
    class IniRead
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);


        public static String getIniData(String section, String key, String filePath)
        {
            StringBuilder sb = new StringBuilder(1024);

            GetPrivateProfileString(section, key, String.Empty, sb, 1024, filePath);

            return sb.ToString();
        }

        public static void setIniData(String section, String key, String val, String filePath)
        {
            WritePrivateProfileString(section, key, val, filePath);
        }
        /*
         IniRead.setIniData("POSINFO", "OPERCD", "230", "config\\eConfig.ini");
            IniRead.setIniData("POSINFO", "RESTCD", "S000570", "config\\eConfig.ini");
            IniRead.setIniData("POSINFO", "SHOPCD", "0001", "config\\eConfig.ini");
            IniRead.setIniData("POSINFO", "VANCD", "03", "config\\eConfig.ini");
            IniRead.setIniData("ECON", "MAC", "00155DEDCB13", "config\\eConfig.ini");
        */


        public static void setIniFile(String path, String fileName, String today, String iniData)
        {
            String[] iniArr = iniData.Split(';');

            try
            {
                if (iniArr.Length > 0 && iniArr.Length <= 8)
                {
                    Console.WriteLine(iniArr[0]); //tid

                    IniRead.setIniData("POSINFO", "OPERCD", iniArr[1], path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "RESTCD", iniArr[2], path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "SHOPCD", iniArr[3], path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "POSNO", iniArr[4], path +  "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "VANCD", iniArr[5], path +  "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "SVRURL", iniArr[6], path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("POSINFO", "POSGRCD", iniArr[7], path + "\\" + fileName + "_" + iniArr[0] + ".ini");

                    IniRead.setIniData("ECON", "MAC", "00155DEDCB13", path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("ECON", "POSAPP", "not exist", path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("ECON", "MODULE", "1, 0, 0, 5", path + "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("ECON", "HDDSN", "D4D4EF02", path +  "\\" + fileName + "_" + iniArr[0] + ".ini");
                    IniRead.setIniData("ECON", "CHKSUM", "File Not Exist", path + "\\" + fileName + "_" + iniArr[0] + ".ini");

                    IniRead.setIniData("ECON", "TERMINAL_ID", iniArr[0], path +  "\\" + fileName + "_" + iniArr[0] + ".ini");

                }
                else
                {
                    Console.WriteLine("ini 배열길이 8넘음:");
                    Console.WriteLine(iniArr.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
