using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShimMaruMaria
{
    class XmlParse
    {
        public String nowDir;
        public XmlDocument xmlFile;
        public XmlParse()
        {
            nowDir = Environment.CurrentDirectory;

            xmlFile = new XmlDocument();
            //xmlFile.Load("..\\config\\query.xml");
            //xmlFile.Load("..\\config\\query_test.xml");

            //xmlFile.Load(UtilCls.rtnConfigPath() + "\\query.xml");
            xmlFile.Load(UtilCls.rtnConfigPath() + "\\query_maria.xml");

        }

        public String rtnQuery(string xmlItem)
        {
            String rtnQuery = "";
            try
            {
                XmlNodeList xmlList = xmlFile.SelectNodes("dbquery");

                foreach (XmlNode node1 in xmlList)
                {
                    //Console.WriteLine(node1[xmlItem].InnerText);
                    rtnQuery = node1[xmlItem].InnerText;
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return rtnQuery;
        }
    }
}
