using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShimMaruMaria
{
    class MariaDB
    {
        private String id = "";
        private String pwd = "";
        private MySqlConnection conn = null;
        private ILog logger = null;
        private String mariaDBStr = "";
        private String mariaStr = "";
        
        public MariaDB() 
        {
            Type nowMethod = MethodBase.GetCurrentMethod().DeclaringType;
            logger = LogManager.GetLogger(nowMethod);

            mariaDBStr = IniRead.getIniData("DB", "conn", UtilCls.rtnConfigPath() + "\\conf.ini");
            id = IniRead.getIniData("DB", "id", UtilCls.rtnConfigPath() + "\\conf.ini");
            pwd = IniRead.getIniData("DB", "pwd", UtilCls.rtnConfigPath() + "\\conf.ini");

            mariaStr = String.Format(mariaDBStr, id, pwd);

            conn = new MySqlConnection(mariaStr);
        }

        ~MariaDB() {
            if (conn != null)
            {
                conn = null;
                logger.Info("MariaDB 해제");
            }
        }

        public bool ConnTest()
        {
            string connStr = "Server=192.168.100.113;Port=3306;Database=vmms;User ID=vmms;Password=ubcn0504";
            logger.Info("연결시도");
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Info("실패:"+ex.ToString());
                return false;
         
            }
        }

        public String eRestSalesINI_TID(String query, String comp_seq, String date)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //logger.Info(mariaStr);
                //logger.Info(query);

                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.Parameters.Add(new MySqlParameter("@company_seq", comp_seq));
                cmd.Parameters.Add(new MySqlParameter("@tran_date", date));

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    sb.Append(dr["TERMINAL_ID"].ToString() + ";");
                    sb.Append(dr["OPER_CD"].ToString()+";");
                    sb.Append(dr["REST_CD"].ToString()+";");
                    sb.Append(dr["SHOP_CD"].ToString() + ";");
                    sb.Append(dr["POS_NO"].ToString()+";");                    
                    sb.Append(dr["VANCD"].ToString()+";");
                    sb.Append(dr["SVRURL"].ToString()+";");
                    sb.Append(dr["POSGR_CD"].ToString() + "|");
                }//

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return sb.ToString();
        }


        public String eRestSales_JungSanTID(String query, String comp_seq, String date)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //logger.Info(mariaStr);
                //logger.Info(query);

                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.Parameters.Add(new MySqlParameter("@company_seq", comp_seq));
                cmd.Parameters.Add(new MySqlParameter("@tran_date", date));

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    sb.Append(dr["TERMINAL_ID"].ToString() + ";");
                }//
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return sb.ToString();
        }


        //도로공사 일마감 데이타
        public String eRestSales_Close_list(String query, String comp_seq, String date, String terminal_id, String casher_no)
        {
            String rtnData = "";
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.Parameters.Add(new MySqlParameter("@terminal_id", terminal_id));
                cmd.Parameters.Add(new MySqlParameter("@company_seq", comp_seq));
                cmd.Parameters.Add(new MySqlParameter("@tran_date", date));

                MySqlDataReader dr = cmd.ExecuteReader();

                rtnData = this.rtnSendErestJUNGSAN(dr, date, casher_no);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return rtnData;
        }

        //도로공사 정산 데이타 생성
        private String rtnSendErestJUNGSAN(MySqlDataReader dr, String dt, String casher_no)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                {
                    /*
                    dr["MIN_TRAN_SEQ"]  --자판기 일련번호 시작
                    dr["MAX_TRAN_SEQ"],  --자판기 일련번호 마침
                    dr["MIN_TRAN_TIME"],  --자판기 거래 시작시간
                    dr["MAX_TRAN_TIME"],  --자판기 마침 시작시간  
                    dr["TOTAL_CNT"],  -- 총거래건수
                    dr["TOTAL_AMT"],  --총거래금액 
                    dr["TOT_CARD_CNT"], --카드+선불거래건수
                    dr["TOT_CARD_AMT"], --카드+선불거래금액
                    dr["TOT_CASH_CNT"], --현금거래 건수
                    dr["TOT_CASH_AMT"]  --현금거래 금액
                    */
                    sb.Append(dt + ";");  // 영업일자 항목구분자( ; )
                    sb.Append("0" + ";"); // 정산종류(0: 총정산, 1:캐셔별 정산)  항목구분자( ; )
                    sb.Append(dt + dr["MIN_TRAN_TIME"].ToString().Substring(0, 4) + ";"); // 개설년월일시분(YYYYMMDDhhmm)   항목구분자( ; )
                    sb.Append(dt + dr["MAX_TRAN_TIME"].ToString().Substring(0, 4) + ";"); // 정산년월일시분(YYYYMMDDhhmm) 항목구분자( ; )
                    sb.Append(casher_no + ";"); // 캐셔 ID  항목구분자( ; )

                    sb.Append(String.Format("{0:D8}", Int32.Parse(dr["MIN_TRAN_SEQ"].ToString())) + ";");  // 시작영수증번호 항목구분자( ; )
                    sb.Append(String.Format("{0:D8}", Int32.Parse(dr["MAX_TRAN_SEQ"].ToString())) + ";");  // 종료영수증번호 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOTAL_CNT"].ToString())) + ";");  // 총판매역 건수  항목구분자( ; )
                    sb.Append(String.Format("{0:D14}",Int32.Parse(dr["TOTAL_AMT"].ToString())) + ";");  // 총판매역 총 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["CARD_CNL_CNT"].ToString())) + ";"); //  반품 건수 항목구분자( ; ) 

                    if (Int32.Parse(dr["CARD_CNL_AMT"].ToString()) > 0)
                    {
                        sb.Append(String.Format("{0:D13}", Int32.Parse(dr["CARD_CNL_AMT"].ToString()) * -1) + ";"); //반품총금액 (총 금액이 양수일 경우 음수, 음수일 경우 양수로 변환 전송)  항목구분자( ; )
                    }
                    else
                    {
                        sb.Append(String.Format("{0:D14}", Int32.Parse(dr["CARD_CNL_AMT"].ToString()) ) + ";"); //반품총금액 (총 금액이 양수일 경우 음수, 음수일 경우 양수로 변환 전송)  항목구분자( ; )
                    }//
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOTAL_CNT"].ToString())) + ";");  //    총매출 건수  항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", Int32.Parse(dr["TOTAL_AMT"].ToString())) + ";");  //    총매출 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOTAL_CNT"].ToString()) - Int32.Parse(dr["CARD_CNL_CNT"].ToString())) + ";");  //  순매출 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", Int32.Parse(dr["TOTAL_AMT"].ToString()) - Int32.Parse(dr["CARD_CNL_AMT"].ToString())) + ";");  //  순매출 금액 항목구분자( ; )

                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOT_CARD_CNT"].ToString())) + ";"); //신용/선불카드 일시불 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", Int32.Parse(dr["TOT_CARD_AMT"].ToString())) + ";"); //신용/선불카드 일시불 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";"); //신용/선불카드 할부 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", 0) + ";"); //신용/선불카드 할부 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["CARD_CNL_CNT"].ToString())) + ";"); // 신용/선불카드 반품일시불 건수 항목구분자( ; )

                    sb.Append(String.Format("{0:D11}", Int32.Parse(dr["CARD_CNL_AMT"].ToString())) + ";");// 신용/선불카드 반품일시불 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");// 신용/선불카드 반품할부 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", 0) + ";");// 신용/선불카드 반품할부 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");//  임의등록 신용/선불 건수  항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", 0) + ";");//   임의등록 신용/선불 금액   항목구분자( ; )

                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOT_CASH_CNT"].ToString())) + ";"); // 현금매출 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", Int32.Parse(dr["TOT_CASH_AMT"].ToString())) + ";"); // 현금매출 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");//   상품권총판매 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", 0) + ";");//   상품권총판매 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");// 상품권총환불 건수 항목구분자( ; )

                    sb.Append(String.Format("{0:D11}", 0) + ";");// 상품권총환불 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");// 상품권매출 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", 0) + ";");//  상품권매출 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOT_CASH_CNT"].ToString())) + ";");//  총현금매출 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D11}", Int32.Parse(dr["TOT_CASH_AMT"].ToString())) + ";");//  총현금매출 금액 항목구분자( ; )

                    sb.Append(String.Format("{0:D4}", 0) + ";");// 선수입금 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", 0) + ";");// 선구입금 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");//  수수료이익액 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", 0) + ";");// 수수료이익액 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", 0) + ";");// 미포함 부가세 금액 건수 항목구분자( ; )

                    sb.Append(String.Format("{0:D11}", 0) + ";");// 미포함 부가세 금액 금액 항목구분자( ; )
                    sb.Append(String.Format("{0:D4}", Int32.Parse(dr["TOTAL_CNT"].ToString()) - Int32.Parse(dr["CARD_CNL_CNT"].ToString()) ) + ";");// 실매출 건수 항목구분자( ; )
                    sb.Append(String.Format("{0:D14}", Int32.Parse(dr["TOTAL_AMT"].ToString()) - Int32.Parse(dr["CARD_CNL_AMT"].ToString())) + ";");//  실매출 금액 항목구분자( ; )                    
                    sb.Append(String.Format("{0:D4}", 0) + ";");// 미전송 매출 개수 항목구분자( ; )
                    sb.Append(casher_no + ";");//  마감 매니저 ID 항목구분자( ; )
                    sb.Append("1" + ";"); //  POS 마감 구분(0: 미마감, 1: 마감)  항목구분자( ; )

                }//while

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }
            return sb.ToString();
        }



        public String eRestSales_POS_list(String query, String terminal_id,String type, String date, String comp_seq, String casher_no, String vanCD,String appType)
        {
            String rtnData = "";
            try
            {
                //logger.Info(mariaStr);
                //logger.Info(query);

                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandTimeout = 1200;
                cmd.Parameters.Add(new MySqlParameter("@company_seq", comp_seq));
                cmd.Parameters.Add(new MySqlParameter("@tran_date", date));
                cmd.Parameters.Add(new MySqlParameter("@terminal_id", terminal_id));

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    logger.Info("데이타 있슴");
                }

                if ("single".Equals(type))
                {
                    rtnData = rtnSendDataSingleErest(dr, casher_no, vanCD, appType);
                }
                else if ("multi".Equals(type))
                {
                    rtnData = rtnSendDataMultiErest(dr, casher_no, vanCD, appType);
                }

                /*
                String recTransactionNo = null;
                String recTerminalId = null;

                if (dr.HasRows)
                {
                    logger.Info("데이타 있슴");
                }

                while (dr.Read())
                {

                    recTransactionNo = dr["TRANSACTION_NO"].ToString();
                    recTerminalId = dr["TERMINAL_ID"].ToString();

                    sb.Append(recTransactionNo + ";");
                    sb.Append(recTerminalId + ";");
                }//
                */
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return rtnData;
        }//


        private String rtnSendDataSingleErest(MySqlDataReader dr, String casher_no, String vanCD,String appType)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                StringBuilder sendHeaderSB = new StringBuilder();
                StringBuilder sendDetailSB = new StringBuilder();
                StringBuilder sendCardSB = new StringBuilder();

                int RowCount = 1000;
                
                String[] prevTranNo = new String[RowCount];
                String recTransaction_NO = null;
                String recTerminal_ID = null;
                String recCard_NO = null;
                String recPurchase_Name = null;
                bool chkItemCnt = false;
                int cnlNum = 1;
                String amtFormat = "{0:D14}";
                String cntFormat = "{0:D15}";
                if ("CNL".Equals(appType))
                {
                    cnlNum = -1;
                    cntFormat = "{0:D14}";
                    amtFormat = "{0:D13}";
                }//

                int i = 0;
                while (dr.Read())
                {
                    recTransaction_NO = dr["TRANSACTION_NO"].ToString();
                    recTerminal_ID = dr["TERMINAL_ID"].ToString();
                    recCard_NO = dr["Card_NO"].ToString();
                    
                    if (i > 0 && prevTranNo[i - 1].Equals(recTransaction_NO))
                    {
                        chkItemCnt = true;
                        logger.Debug("장바구니: " + recTransaction_NO);
                    }
                    else
                    {
                        chkItemCnt = false;
                        logger.Debug("장바구니 아님: " + recTransaction_NO);
                    }//

                    //S_SETSALEHDR
                    sendHeaderSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //기준영업일
                    sendHeaderSB.Append(String.Format("{0:D8}", Int32.Parse(dr["TRAN_SEQ"].ToString())) + ";"); //영수증번호 
                    sendHeaderSB.Append("0;"); //판매구분(0:매출, 1:반품)
                    sendHeaderSB.Append(dr["TRANSACTION_DATE"].ToString() + dr["TRANSACTION_TIME"].ToString() + ";"); //실판매일시
                    sendHeaderSB.Append(String.Format("{0:D2}", Int32.Parse(dr["ITEM_COUNT"].ToString())) + ";"); //총 판매상품건수 
                    sendHeaderSB.Append(String.Format("{0:D2}", Int32.Parse(dr["ITEM_COUNT"].ToString())) + ";"); //총전표건수
                    sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //총판매금액
                    
                    if (!"".Equals(dr["CASH_APPROVAL_DATE"].ToString()))
                    {
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //현금판매금액
                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //카드판매금액
                    }
                    else
                    {
                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //현금판매금액
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["CARD_AMOUNT"].ToString()) * cnlNum) + ";"); //카드판매금액                        
                    }//
                    sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //상품권판매금액
                    sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //보관/외상주유액(반품매출일 경우 부호 변환)
                    sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //선수입금액(반품매출일 경우 부호 변환)
                    sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //수수료이익액(반품매출일 경우 부호 변환)
                    sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //실매출액(부가세 포함) (반품매출일 경우 부호 변환)
                    sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["VAT"].ToString()) * cnlNum) + ";"); //부가세액(반품매출일 경우 부호 변환)
                    sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["SUPPLY"].ToString()) * cnlNum) + ";"); //순매출액(부가세 제외 금액) (반품매출일 경우 부호 변환)
                    
                    sendHeaderSB.Append("0;"); //현금영수증 발행 구분(0:미발행, 1:발행)
                    sendHeaderSB.Append(" ".PadRight(32) + ";"); //원거래 영수증번호(영업일자(8) + 운영업체코드(3) + 휴게소코드(7)
                    sendHeaderSB.Append(casher_no + ";"); //사원번호
                    sendHeaderSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)

                    if (chkItemCnt == false)
                    {
                        sendDetailSB.Append("01;"); //라인번호
                    }

                    sendDetailSB.Append(dr["POS_NO"].ToString() + ";"); //매장코드(상품 마스터의 매장코드)
                    sendDetailSB.Append(dr["PRODUCT_CODE"].ToString() + ";"); //판매상품Seq
                    sendDetailSB.Append(dr["PRODUCT_BARCODE"].ToString() + ";"); //바코드(소스코드)
                    sendDetailSB.Append(String.Format(cntFormat, 1 * cnlNum) + ";"); //판매수량(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //판매금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append("00;"); //노즐번호 - Default: ‘00’
                    sendDetailSB.Append(String.Format("{0:D15}", Int32.Parse(dr["AMOUNT"].ToString())) + ";"); //판매단가
                    sendDetailSB.Append(String.Format("{0:D15}", 0) + ";"); //수수료단가   
                    sendDetailSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //선수입금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //수수료금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //보관/외상매출액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //실매출액(부가세 포함) (반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["VAT"].ToString()) * cnlNum) + ";"); //부가세액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["SUPPLY"].ToString()) * cnlNum) + ";"); //순매출액(부가세 제외 금액) (반품매출일 경우 부호 변환)
                    sendDetailSB.Append(" ".PadLeft(14) + ";"); //주유완료일시(YYYYMMDDhhmmss) – Default: ‘0’ or ‘ ‘
                    sendDetailSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)

                    if (!"".Equals(recCard_NO)) // 신용카드 매출
                    {
                        recPurchase_Name = dr["PURCHASE_NAME"].ToString();

                        sendCardSB.Append("01;"); //  Slip 구분자(‘01’: 카드)
                        sendCardSB.Append(String.Format(amtFormat, Int32.Parse(dr["CARD_AMOUNT"].ToString()) * cnlNum) + ";"); //승인금액(반품매출일 경우 부호 변환)
                        sendCardSB.Append(UtilCls.rtnCardCode_MAE(dr["PURCHASE_CODE"].ToString()).PadRight(4) + ";"); //매입사코드 4byte
                        sendCardSB.Append(recPurchase_Name + " ".PadRight(16 - UtilCls.getStringLength(recPurchase_Name)) + ";"); //매입사명(카드사 마스터)  : 16byte
                        sendCardSB.Append(dr["APPROVAL_NO"].ToString() + ";"); //승인번호
                        sendCardSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //승인일자(YYYYMMDD)
                        sendCardSB.Append(dr["TRANSACTION_TIME"].ToString() + ";"); //승인시간(hhmmss)
                        sendCardSB.Append(UtilCls.rtnCardType(dr["PURCHASE_CODE"].ToString()) + ";"); //(0:신용카드, 1:선불카드, 2:보너스적립카드, 3:보너스할인카드  
                        sendCardSB.Append("1" + ";"); //자동이체구분(0:자동이체, 1:전표매입)
                        sendCardSB.Append(UtilCls.rtnCardCode_NOR(dr["PURCHASE_CODE"].ToString()).PadRight(3) + ";"); //표준카드사코드(카드사 마스터) 3byte
                        sendCardSB.Append(vanCD + ";"); //VAN사 코드(POS 환경정보)  --> POS 셋팅값 --?                        
                    }
                    else
                    {
                        sendCardSB.Append("02;"); //  Slip 구분자(‘02’: 현금)
                        sendCardSB.Append(String.Format(amtFormat, Int32.Parse(dr["CASH_AMOUNT"].ToString()) * cnlNum) + ";"); //승인금액(반품매출일 경우 부호 변환)
                        sendCardSB.Append(dr["CASH_APPROVAL_NO"].ToString().PadRight(8) + ";"); //승인번호
                        sendCardSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //승인일자(YYYYMMDD)
                        sendCardSB.Append(dr["TRANSACTION_TIME"].ToString() + ";"); //승인시간(hhmmss)
                        sendCardSB.Append(vanCD + ";"); //VAN사 코드(POS 환경정보)  --> POS 셋팅값 --?                        
                    }//

                    prevTranNo[i] = recTransaction_NO;

                    i++;

                    

                    sb.Append(recTransaction_NO + "_");
                    sb.Append(recTerminal_ID + "_");
                    sb.Append(sendHeaderSB.ToString() + "_");
                    sb.Append(sendDetailSB.ToString() + "_");
                    sb.Append(sendCardSB.ToString() + "=");

                    //초기화
                    sendHeaderSB.Length = 0;
                    sendDetailSB.Length = 0;
                    sendCardSB.Length = 0;

                }//while

                logger.Info(i);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }//

            return sb.ToString();
        }


        private String rtnSendDataMultiErest(MySqlDataReader dr, String casher_no, String vanCD, String appType)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                //장바구니 거래는 header 1건 + detail + card 여러건 
                StringBuilder sendHeaderSB = new StringBuilder();
                StringBuilder sendDetailSB = new StringBuilder();
                StringBuilder sendCardSB = new StringBuilder();

                int RowSize = 1000; //충분히 큰수
                String[] prevTranNo = new String[RowSize];
                String recTransactionNo = "";
                String recTerminalID = "";
                String recCard_NO = "";
                String recPurchase_Name = "";
                int cnlNum = 1;
                String amtFormat = "{0:D14}";
                String cntFormat = "{0:D15}";
                if ("CNL".Equals(appType))
                {
                    cnlNum = -1;
                    cntFormat = "{0:D14}";
                    amtFormat = "{0:D13}";
                }//

                bool chkItemCnt = false;
                int iRecItemCnt = 0;
                int i = 0;
                int iLineNum = 0;
                while(dr.Read()) {
                    recTransactionNo = dr["TRANSACTION_NO"].ToString();
                    recTerminalID = dr["TERMINAL_ID"].ToString();
                    recCard_NO = dr["CARD_NO"].ToString();
                    recPurchase_Name = dr["PURCHASE_NAME"].ToString();
                    iRecItemCnt = Int32.Parse(dr["ITEM_COUNT"].ToString());
                    if (i > 0 && prevTranNo[i - 1].Equals(recTransactionNo))
                    {
                        chkItemCnt = true;
                        logger.Info("장바구니: " + recTransactionNo);
                        iLineNum++;
                    }
                    else
                    {
                        chkItemCnt = false;
                        logger.Info("장바구니 변경: " + recTransactionNo);
                        iLineNum = 0;
                    }//

                    if (chkItemCnt == false && iLineNum == 0)
                    {
                        //Header 만들기
                        //sendHeader
                        sendHeaderSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //영업일
                        sendHeaderSB.Append(String.Format("{0:D8}", Int32.Parse(dr["TRAN_SEQ"].ToString())) + ";"); //영수증번호 
                        sendHeaderSB.Append("0;"); //판매구분(0:매출, 1:반품)
                        sendHeaderSB.Append(dr["TRANSACTION_DATE"].ToString() + dr["TRANSACTION_TIME"].ToString() + ";"); //실판매일시
                        sendHeaderSB.Append(String.Format("{0:D2}", Int32.Parse(dr["ITEM_COUNT"].ToString())) + ";"); //총 판매상품건수 
                        sendHeaderSB.Append(String.Format("{0:D2}", 1) + ";"); //총전표건수
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_AMOUNT"].ToString()) * cnlNum) + ";"); //총판매금액
                        
                        if (!"".Equals(dr["CASH_APPROVAL_DATE"].ToString()))
                        {
                            sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_AMOUNT"].ToString()) * cnlNum) + ";"); //현금판매금액
                            sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //카드판매금액
                        }
                        else
                        {
                            sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //현금판매금액
                            sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_AMOUNT"].ToString()) * cnlNum) + ";"); //카드판매금액
                        }//

                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //상품권판매금액
                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //보관/외상주유액(반품매출일 경우 부호 변환)
                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //선수입금액(반품매출일 경우 부호 변환)
                        sendHeaderSB.Append(String.Format("{0:D14}", 0) + ";"); //수수료이익액(반품매출일 경우 부호 변환)
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_AMOUNT"].ToString())*cnlNum) + ";"); //실매출액(부가세 포함) (반품매출일 경우 부호 변환)
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_VAT"].ToString())*cnlNum) + ";"); //부가세액(반품매출일 경우 부호 변환)
                        sendHeaderSB.Append(String.Format(amtFormat, Int32.Parse(dr["TOT_SUPPLY"].ToString())*cnlNum) + ";"); //순매출액(부가세 제외 금액) (반품매출일 경우 부호 변환)

                        sendHeaderSB.Append("0;"); //현금영수증 발행 구분(0:미발행, 1:발행)
                        sendHeaderSB.Append(" ".PadRight(32) + ";"); //원거래 영수증번호(영업일자(8) + 운영업체코드(3) + 휴게소코드(7) + 포스그룹코드(2) + 포스번호(4) + 영수증번호(8))
                        sendHeaderSB.Append(casher_no + ";"); //사원번호                        
                        sendHeaderSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)

                        sb.Append(recTransactionNo + "_");
                        sb.Append(recTerminalID + "_");
                        sb.Append(sendHeaderSB.ToString() + "_");
                    }

                    sendDetailSB.Append(String.Format("{0:D2}", (iLineNum + 1)) + ";"); //라인번호                    
                    sendDetailSB.Append(dr["POS_NO"].ToString() + ";"); //매장코드(상품 마스터의 매장코드)
                    sendDetailSB.Append(dr["PRODUCT_CODE"].ToString() + ";"); //판매상품Seq
                    sendDetailSB.Append(dr["PRODUCT_BARCODE"].ToString() + ";"); //바코드(소스코드)
                    sendDetailSB.Append(String.Format(cntFormat, 1*cnlNum) + ";"); //판매수량(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString())*cnlNum) + ";"); //판매금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append("00;"); //노즐번호 - Default: ‘00’
                    sendDetailSB.Append(String.Format("{0:D15}", Int32.Parse(dr["AMOUNT"].ToString())) + ";"); //판매단가
                    sendDetailSB.Append(String.Format("{0:D15}", 0) + ";"); //수수료단가                    
                    sendDetailSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //선수입금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //수수료금액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format("{0:D14}", 0) + ";"); //보관/외상매출액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //실매출액(부가세 포함) (반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["VAT"].ToString()) * cnlNum) + ";"); //부가세액(반품매출일 경우 부호 변환)
                    sendDetailSB.Append(String.Format(amtFormat, Int32.Parse(dr["SUPPLY"].ToString()) * cnlNum) + ";"); //순매출액(부가세 제외 금액) (반품매출일 경우 부호 변환)

                    sendDetailSB.Append(" ".PadLeft(14) + ";"); //주유완료일시(YYYYMMDDhhmmss) – Default: ‘0’ or ‘ ‘
                    sendDetailSB.Append("0;"); //주유처리구분(0:일반,1:외상,2:보관주유,4:검량,5:자가소비)

                    if ((iLineNum + 1) == iRecItemCnt)
                    {

                        if (!"".Equals(recCard_NO)) // 신용카드 매출
                        {
                            recPurchase_Name = dr["PURCHASE_NAME"].ToString();

                            sendCardSB.Append("01;"); //  Slip 구분자(‘01’: 카드)
                            sendCardSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //승인금액(반품매출일 경우 부호 변환)
                            sendCardSB.Append(UtilCls.rtnCardCode_MAE(dr["PURCHASE_CODE"].ToString()).PadRight(4) + ";"); //매입사코드 4byte
                            sendCardSB.Append(recPurchase_Name + " ".PadRight(16 - UtilCls.getStringLength(recPurchase_Name)) + ";"); //매입사명(카드사 마스터)  : 16byte
                            sendCardSB.Append(dr["APPROVAL_NO"].ToString() + ";"); //승인번호
                            sendCardSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //승인일자(YYYYMMDD)
                            sendCardSB.Append(dr["TRANSACTION_TIME"].ToString() + ";"); //승인시간(hhmmss)
                            sendCardSB.Append(UtilCls.rtnCardType(dr["PURCHASE_CODE"].ToString()) + ";"); //(0:신용카드, 1:선불카드, 2:보너스적립카드, 3:보너스할인카드  
                            sendCardSB.Append("1" + ";"); //자동이체구분(0:자동이체, 1:전표매입)
                            sendCardSB.Append(UtilCls.rtnCardCode_NOR(dr["PURCHASE_CODE"].ToString()).PadRight(3) + ";"); //표준카드사코드(카드사 마스터) 3byte
                            sendCardSB.Append(vanCD + ";"); //VAN사 코드(POS 환경정보)  --> POS 셋팅값 --?                        
                        }
                        else
                        {
                            sendCardSB.Append("02;"); //  Slip 구분자(‘02’: 현금)
                            sendCardSB.Append(String.Format(amtFormat, Int32.Parse(dr["AMOUNT"].ToString()) * cnlNum) + ";"); //승인금액(반품매출일 경우 부호 변환)
                            sendCardSB.Append(dr["CASH_APPROVAL_NO"].ToString().PadRight(8) + ";"); //승인번호
                            sendCardSB.Append(dr["TRANSACTION_DATE"].ToString() + ";"); //승인일자(YYYYMMDD)
                            sendCardSB.Append(dr["TRANSACTION_TIME"].ToString() + ";"); //승인시간(hhmmss)
                            sendCardSB.Append(vanCD + ";"); //VAN사 코드(POS 환경정보)  --> POS 셋팅값 --?                        
                        }//

                        sb.Append(sendDetailSB.ToString() + "_");
                        sb.Append(sendCardSB.ToString() + "=");

                        sendHeaderSB.Length = 0;
                        sendDetailSB.Length = 0;
                        sendCardSB.Length = 0;
                    }//
                    prevTranNo[i] = recTransactionNo; 
                    i++;
                }//while

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.ToString());
            }

            return sb.ToString();
        }


    }
}
