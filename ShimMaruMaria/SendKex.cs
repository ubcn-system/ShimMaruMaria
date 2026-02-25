using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShimMaruMaria
{
    class SendKex
    {
        private ILog logger = null;
        private XmlParse xp = null;
        private MariaDB db = null;
        private String comp_seq = "";
        private String van_cd = "";
        private String casher_no = "";
        
        private String path = "";
        private String file = "";
        private String ext = "";

        public SendKex()
        {
            Type newMethod = MethodBase.GetCurrentMethod().DeclaringType;
            logger = LogManager.GetLogger(newMethod);

            comp_seq = IniRead.getIniData("Shop", "comp_seq", UtilCls.rtnConfigPath() + "\\conf.ini");
            van_cd = IniRead.getIniData("Shop", "van_cd", UtilCls.rtnConfigPath() + "\\conf.ini");
            casher_no = IniRead.getIniData("Shop", "casher_no", UtilCls.rtnConfigPath() + "\\conf.ini");

            path = IniRead.getIniData("eRest", "path", UtilCls.rtnConfigPath() + "\\conf.ini");
            file = IniRead.getIniData("eRest", "filenm", UtilCls.rtnConfigPath() + "\\conf.ini");
            ext = IniRead.getIniData("eRest", "ext", UtilCls.rtnConfigPath() + "\\conf.ini");

            xp = new XmlParse();
            db = new MariaDB();
        }//

        public void ERestTest()
        {
            String tRec = null;
            ERESTLib.PosAPI erest = new ERESTLib.PosAPIClass();
            int API_RET = erest.ERestApiInit(out tRec);
            logger.Info(API_RET);
            API_RET = erest.ERestApiRecvMstPos("C:\\eRest\\MOD\\MST\\pos.mst","20260219000000");

            logger.Info(API_RET);

            if (tRec != null)
            {
                logger.Info(tRec);
            }
        }




        private void sendErestData(ERESTLib.PosAPI erest,String rtnDataArr,String title)
        {
            int i = 0;
            int j = 0;
            String[] sendLineArr = null;
            String[] sendArr = null;
            sendLineArr = rtnDataArr.Split('=');


            //전송전 모듈 초기화 필요 각 파라미터는 조은시스템 또는 도로공사에서 받아야 한다.
            int API_RET = 0;
            //API_RET = erest.ERestApiSendSaleData("", "", "");

            logger.Info(API_RET);

            logger.Info(sendLineArr.Length);
            for (i = 0; i <sendLineArr.Length-1; i++)
            {
                sendArr = sendLineArr[i].Split('_');

                logger.Info(String.Format("{0}:{1}:{2}:{3}", title, i, sendArr.Length,sendLineArr[i]));
                logger.Info(sendArr[0]);  //transaction_no
                logger.Info(sendArr[1]);  //terminal_id
                logger.Info(sendArr[2]);  //header
                logger.Info(sendArr[3]);  //body
                logger.Info(sendArr[4]);  //card

                /*
                for (j = 0; j < sendArr.Length; j++)
                {
                    logger.Info(String.Format("{0}:{1}:{2}", title, j, sendArr[j]));
                }*/

                //API_RET = erest.ERestApiSendSaleData("", "", "");

                //거래내역 업데이트 필요-----------------------
            }//
        }

        public void SendData_ERestClose(String terminal_id, String data, ERESTLib.PosAPI erest)
        {
            int API_RET = 0;

            logger.Info(String.Format("정산 Data[{0}]:[{1}]", terminal_id, data));

            API_RET = erest.ERestApiSendDayCloseData(data);

            logger.Info("응답:" + API_RET.ToString());
        }

        public String makeiniFile(String today, String type)
        {
            //20260223
            int i = 0;
            String year = today.Substring(0, 4);
            String month = today.Substring(4, 2);
            String day = today.Substring(6, 2);
            String iniPath = path + "\\" + year + "\\" + month + "\\" + day;
            String rtnIniFile = "";
            bool rtnBool = false;
            //ini 디렉토리 생성
            rtnBool = UtilCls.createDir(path, year);
            if (rtnBool == true)
            {
                rtnBool = UtilCls.createDir(path + "\\" + year, month);
                if (rtnBool == true)
                {
                    rtnBool = UtilCls.createDir(path + "\\" + year+"\\"+month,day);
                }
                else 
                {
                    logger.Error(String.Format("iniPATH:{0} 생성불가",iniPath));                    
                }//
            }else {
                logger.Error(String.Format("iniPATH:{0} 생성불가",path+"\\"+year));                
            }//
            if (rtnBool == false)
            {
                return rtnIniFile;
            }//
            //ini 파일 생성 
            String rtnQuery = "";   

            if ("SINGLE".Equals(type))
            {
                rtnQuery = xp.rtnQuery("eRest_TID_POS_SINGLE");
            }
            else if("MULTI".Equals(type))
            {
                rtnQuery = xp.rtnQuery("eRest_TID_POS_MULTI");
            }
            else if ("JUNGSAN".Equals(type))
            {
                rtnQuery = xp.rtnQuery("eRest_jungSanTID");
            }

            String arrTerminalID = db.eRestSalesINI_TID(rtnQuery, comp_seq, today);

            String[] arr_iniTID = arrTerminalID.Split('|');
            for (i = 0; i < arr_iniTID.Length; i++)
            {
                if (!"".Equals(arr_iniTID[i]))
                {
                    IniRead.setIniFile(iniPath, file, today, arr_iniTID[i]);
                }//
            }

            rtnIniFile = UtilCls.rtnFiles(iniPath);

            return rtnIniFile;
        }



        public void SendSingleHistory(String today)
        {
            String year = today.Substring(0, 4);
            String month = today.Substring(4, 2);
            String day = today.Substring(6, 2);
            String iniPath = path + "\\" + year + "\\" + month + "\\" + day;
            String eConfigPath = path + "\\" + file + "." + ext;
            int i = 0;
            int API_RET = 0;
            String tRec = null;
            String rtnQuery = "";
            String rtnDataArr = "";
            String appType = "";
            String listType = "";
            String rtnIniFile = this.makeiniFile(today, "SINGLE");
            String terminal_id = "";
            ERESTLib.PosAPI erest = new ERESTLib.PosAPIClass();

            if (rtnIniFile.Length > 0)
            {
                String[] orgIniFile = rtnIniFile.Split(',');

                for (i = 0; i < orgIniFile.Length; i++)
                {
                    UtilCls.fileCopy(iniPath + "\\" + orgIniFile[i], eConfigPath);
                    terminal_id = IniRead.getIniData("ECON", "TERMINAL_ID", eConfigPath);

                    API_RET = erest.ERestApiInit(out tRec);  //초기화

                    logger.Info("INI DATA:" + tRec);

                    appType = "APP";
                    listType = "single";
                    rtnQuery = xp.rtnQuery("ERestSales_POS_Single_APP");
                    rtnDataArr = db.eRestSales_POS_list(rtnQuery, terminal_id, listType, today, comp_seq, casher_no, van_cd, appType);
                    logger.Info("SINGLE 레코드:" + rtnDataArr.Replace("|", "\n").Replace("=", "\n"));
                    sendErestData(erest, rtnDataArr, "승인(싱글)");

                    appType = "CNL";
                    listType = "single";
                    rtnQuery = xp.rtnQuery("ERestSales_POS_Single_CNL");
                    rtnDataArr = db.eRestSales_POS_list(rtnQuery, terminal_id, listType, today, comp_seq, casher_no, van_cd, appType);
                    logger.Info("SINGLE 레코드 취소:" + rtnDataArr.Replace("|", "\n").Replace("=", "\n"));
                    sendErestData(erest, rtnDataArr, "승인(취소)");
                }// for
            }
        }
        
        public void SendMultiHistory(String today)
        {
            String year = today.Substring(0, 4);
            String month = today.Substring(4, 2);
            String day = today.Substring(6, 2);
            String iniPath = path + "\\" + year + "\\" + month + "\\" + day;
            String eConfigPath = path + "\\" + file + "." + ext;
            int i = 0;
            int API_RET = 0;
            String tRec = null;
            String rtnQuery = "";
            String rtnDataArr = "";
            String appType = "";
            String listType = "";
            String rtnIniFile = this.makeiniFile(today, "MULTI");
            String terminal_id = "";
            ERESTLib.PosAPI erest = new ERESTLib.PosAPIClass();
            
            if (rtnIniFile.Length > 0)
            {
                String[] orgIniFile = rtnIniFile.Split(',');
                for (i = 0; i < orgIniFile.Length; i++)
                {
                    UtilCls.fileCopy(iniPath + "\\" + orgIniFile[i], eConfigPath);
                    terminal_id = IniRead.getIniData("ECON", "TERMINAL_ID", eConfigPath);

                    API_RET = erest.ERestApiInit(out tRec);  //초기화
                    logger.Info("INI DATA:" + tRec);

                    appType = "APP";
                    rtnQuery = xp.rtnQuery("ERestSales_POS_Multi_APP");
                    listType = "multi";
                    rtnDataArr = db.eRestSales_POS_list(rtnQuery, terminal_id, listType, today, comp_seq, casher_no, van_cd, appType);
                    //logger.Info("MULTI 레코드:" + rtnDataArr.Replace("|", "\n").Replace("=", "\n"));
                    sendErestData(erest, rtnDataArr, "승인(멀티)");

                    appType = "APP";
                    rtnQuery = xp.rtnQuery("ERestSales_POS_Multi_CNL");
                    listType = "multi";
                    rtnDataArr = db.eRestSales_POS_list(rtnQuery, terminal_id, listType, today, comp_seq, casher_no, van_cd, appType);
                    //logger.Info("MULTI 레코드:" + rtnDataArr.Replace("|", "\n").Replace("=", "\n"));
                    sendErestData(erest, rtnDataArr, "취소(멀티)");
                   
                }// for
            }
        }// 

        public void SendDayClose(String yesterday)
        {
            String year = yesterday.Substring(0, 4);
            String month = yesterday.Substring(4, 2);
            String day = yesterday.Substring(6, 2);
            String iniPath = path + "\\" + year + "\\" + month + "\\" + day;
            String eConfigPath = path + "\\" + file + "." + ext;
            int i = 0;
            int API_RET = 0;
            String tRec = null;
            String rtnIniFile = this.makeiniFile(yesterday, "JUNGSAN");
            String terminal_id = "";
            String rtnQuery = "";
            String rtnData = "";
            ERESTLib.PosAPI erest = new ERESTLib.PosAPIClass();

            if (rtnIniFile.Length > 0)
            {
                String[] orgIniFile = rtnIniFile.Split(',');
                for (i = 0; i < orgIniFile.Length; i++)
                {
                    UtilCls.fileCopy(iniPath + "\\" + orgIniFile[i], eConfigPath);
                    terminal_id = IniRead.getIniData("ECON", "TERMINAL_ID", eConfigPath);

                    API_RET = erest.ERestApiInit(out tRec);  //초기화
                    logger.Info("INI DATA:" + tRec);

                    if (!"".Equals(tRec))
                    {
                        rtnQuery = xp.rtnQuery("jungSanDay_Erest");
                        rtnData = db.eRestSales_Close_list(rtnQuery, comp_seq, yesterday, terminal_id, casher_no);
                    }
                    else
                    {
                        logger.Info(terminal_id + " ini 데이타 누락");
                    }//

                    //SendData_ERestClose(terminal_id, rtnData, erest); //마감데이타 전송
                }
            }//

        }

        

    }
}
