using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShimMaruMaria
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /* 서비스 생성용 코드
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() 
            };
            ServiceBase.Run(ServicesToRun);
             */

            /*
            MariaDB db = new MariaDB();

            if (db.ConnTest())
            {
                Console.WriteLine("연결성공");
            }//

            db.ERestTest();
            */

            SendKex sc = new SendKex();
            String today = UtilCls.rtnToDay("yyyyMMdd");
            String yesterday = UtilCls.rtnDay("yyyyMMdd", -1);
            
            //sc.SendSingleHistory(today);

            //sc.SendMultiHistory(today);

            sc.SendDayClose(yesterday);


        }
    }
}
