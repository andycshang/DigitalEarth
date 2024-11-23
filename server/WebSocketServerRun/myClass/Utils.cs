
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using WebSocketServerRun.BLL;

namespace WebSocketServerRun.Common
{
    public static  class Utils
    {
        #region 获取配置文件的值
        /// <summary>
        /// 获取工作流数据库连接
        /// </summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="key">节点名称 AppCenterDSN</param>
        /// <param name="nodeDir">指定节点所在的节点目录 configuration/appSettings</param>
        /// <returns></returns>
        public static string GetCCFlowAppCenterDB(string key, string nodeDir = "configuration/appSettings", string strFileName = "")
        {
            string AppCenterDSN = "";
            if (strFileName == "")
            {
                //截取项目路径获取 app.config配置文件路径
                strFileName = System.Windows.Forms.Application.StartupPath;
                // strFileName = strFileName.Substring(0, System.Windows.Forms.Application.StartupPath.LastIndexOf("\\")) ;
                strFileName = strFileName + "\\config.config";
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strFileName);

                XmlNodeList nodeList = xmlDoc.SelectSingleNode(nodeDir).ChildNodes;//获取bookstore节点的所有子节点 

                foreach (XmlNode xn in nodeList)    //遍历所有子节点 
                {
                    if (xn != null && xn.Attributes != null && xn.Attributes.Count > 0 && xn.Attributes["key"].Value == key)
                    {
                        AppCenterDSN = xn.Attributes["value"].Value.Trim();//获取工作流数据库连接                  
                        break;
                    }

                }
            }
            catch
            {

            }
            return AppCenterDSN;
        }
        #endregion

        /// <summary>
        ///设置值
        /// </summary>
        /// <param name="v">值</param>
        /// <param name="key">key</param>
        /// <param name="key">节点名称 AppCenterDSN</param>
        /// <param name="nodeDir">指定节点所在的节点目录 configuration/appSettings</param>
        /// <returns></returns>
        public static bool SetCCFlowAppCenterDB(string v, string key, string nodeDir = "configuration/appSettings", string strFileName = "")
        {

            if (strFileName == "")
            {
                //截取项目路径获取 app.config配置文件路径
                strFileName = System.Windows.Forms.Application.StartupPath;
                // strFileName = strFileName.Substring(0, System.Windows.Forms.Application.StartupPath.LastIndexOf("\\")) ;
                strFileName = strFileName + "\\config.config";
            }
            bool b = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strFileName);

                XmlNodeList nodeList = xmlDoc.SelectSingleNode(nodeDir).ChildNodes;//获取bookstore节点的所有子节点 

                foreach (XmlNode xn in nodeList)    //遍历所有子节点 
                {
                    if (xn != null && xn.Attributes != null && xn.Attributes.Count > 0 && xn.Attributes["key"].Value == key)
                    {
                        xn.Attributes["value"].Value = v;//获取工作流数据库连接    
                        b = true;
                        break;
                    }

                }
                xmlDoc.Save(strFileName);//保存               
            }
            catch
            {

            }
            return b;
        }
    }
}
