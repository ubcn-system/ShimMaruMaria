using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShimMaruMaria
{
    class UtilCls
    {
        public static String rtnConfigPath()
        {
            String rtnPath = IniRead.getIniData("Path", "path", "C:\\Shinmaru\\config\\conf.ini");

            return rtnPath;
        }

        public static String rtnToDay(String pattern)
        {
            String toDay = "";
            DateTime now = DateTime.Now;
            toDay = now.ToString(pattern); //"yyyyMMdd"

            return toDay;
        }


        public static String rtnDay(String pattern, int day)
        {
            String rtnDay = "";
            DateTime now = DateTime.Now.AddDays(day);
            rtnDay = now.ToString(pattern); //"yyyyMMdd"

            return rtnDay;
        }

        /**
         * 카드 매입사 코드
         */
        /*
           KKP	카카오Pay
HNC	하나카드
KEC	외환카드
FMC	해외마스터
KBC	국민카드
NPC	NHN페이코
FAC	해외아멕스
SSC	삼성카드
NAC	농협카드
SHC	신한카드
BCC	비씨카드
HDC	현대카드
LTC	롯데카드
FVC	해외비자
TMN	티머니
    */
        public static String rtnCardCode_MAE(String card)
        {
            String rtnCode = "";

            switch (card)
            {
                case "BCC": // 비씨카드
                    rtnCode = "01";
                    break;
                case "KBC": // 국민카드
                    rtnCode = "02";
                    break;
                case "HNC": // 하나카드
                case "KEC": //외환카드
                    rtnCode = "03";
                    break;
                case "SSC": //삼성카드
                    rtnCode = "04";
                    break;
                case "SHC": //신한카드
                    rtnCode = "05";
                    break;
                case "HDC": //현대카드
                    rtnCode = "09";
                    break;
                case "LTC": //롯데카드
                    rtnCode = "10";
                    break;
                case "NAC": //농협카드
                    rtnCode = "12";
                    break;
                default:
                    rtnCode = "  ";
                    break;
            }

            return rtnCode;
        }


        /**
         * 카드 표준 코드
         */
        public static String rtnCardCode_NOR(String card)
        {
            String rtnCode = "";

            switch (card)
            {
                case "BCC": // 비씨카드
                    rtnCode = "01";
                    break;
                case "KBC": // 국민카드
                    rtnCode = "02";
                    break;
                case "HNC": // 하나카드
                case "KEC": //외환카드
                    rtnCode = "15";
                    break;
                case "SSC": //삼성카드
                    rtnCode = "04";
                    break;
                case "SHC": //신한카드
                    rtnCode = "05";
                    break;
                case "HDC": //현대카드
                    rtnCode = "09";
                    break;
                case "LTC": //롯데카드
                    rtnCode = "10";
                    break;
                case "NAC": //농협카드
                    rtnCode = "12";
                    break;
                default:
                    rtnCode = "  ";
                    break;
            }

            return rtnCode;
        }//

        //신용, 선불 리턴
        public static String rtnCardType(String card)
        {
            String rtnCode = "";

            switch (card)
            {
                case "BCC": // 비씨카드                    
                case "KBC": // 국민카드                    
                case "HNC": // 하나카드
                case "KEC": //외환카드                    
                case "SSC": //삼성카드                    
                case "SHC": //신한카드                    
                case "HDC": //현대카드                    
                case "LTC": //롯데카드                    
                case "NAC": //농협카드
                    rtnCode = "0";  //신용카드
                    break;
                case "TMN": //티머니
                case "CSB": //캐시비
                case "MYB": //마이비
                    rtnCode = "1"; //선불카드
                    break;
                default:
                    rtnCode = " ";
                    break;
            }//

            return rtnCode;
        }

        /*
         * UrlEncode
         */
        public static String rtnUrlEncode(String str)
        {
            String rtnEncode = "";
            if (!"".Equals(str))
            {
                rtnEncode = HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            return rtnEncode;
        }

        /*
         * urlDecode
         */
        public static String rtnUrlDecode(String str)
        {
            String rtnDecode = "";
            if (!"".Equals(str))
            {
                rtnDecode = HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            return rtnDecode;
        }


        //문자열 길이 byte
        public static int getStringLength(string str)
        {
            string s = str;
            byte[] strBt = System.Text.Encoding.Default.GetBytes(s);

            return strBt.Length;
        }

        /*
         * 디렉토리 생성
         */
        public static bool createDir(String path, String folder)
        {
            bool rtnBool = true;
            DirectoryInfo di = new DirectoryInfo(path + "\\" + folder);

            try
            {
                if (di.Exists == true)
                {
                    di.Delete(true);
                }//
                di.Create();
                rtnBool = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                rtnBool = false;
            }

            return rtnBool;
        }//

        /*
         * 디렉토리 삭제(사용에 주의!!)
         */
        public static bool deleteDir(String path, String folder)
        {
            bool rtnBool = false;
            DirectoryInfo di = new DirectoryInfo(path + "\\" + folder);
            try
            {
                if (di.Exists == true)
                {
                    di.Delete(true);
                    rtnBool = true;
                }
                else
                {
                    rtnBool = false;
                }//
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                rtnBool = false;
            }

            return rtnBool;
        }



        /*
         *디렉토리의 파일 이름 리턴
         */
        public static String rtnFiles(String path)
        {
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(path);
            string fileName = "";
            String rtnFiles = null;

            // Copy the files and overwrite destination files if they already exist.
            foreach (string s in files)
            {
                // Use static Path methods to extract only the file name from the path.
                fileName = Path.GetFileName(s);
                sb.Append(fileName + ",");
            }

            rtnFiles = sb.ToString();

            if (rtnFiles.Length > 0)
            {
                rtnFiles = rtnFiles.Substring(0, rtnFiles.Length - 1);
            }
            return rtnFiles;
        }


        public static void fileCopy(String orgPath, String newPath)
        {
            File.Copy(orgPath, newPath, true);
            File.Move(orgPath, orgPath + "_");
        }
    }
}
