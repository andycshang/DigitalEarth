using DBconnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WebSocketServerRun.BLL
{
    /// <summary>
    /// Models
    /// </summary>
    public class BLL_Message
    {
        mssqlConnet myDALPublic =
        new mssqlConnet();



        ////新增访客提醒信息
        //public int InsertVisitorAlertMessage(int ReminderMessageTypeID, int VisitorUserID, string ReminderTitle, DateTime ReminderTime, string ReminderContent, string Url)
        //{
        //    SqlParameter[] mySqlParameters
        //        = {
        //        new SqlParameter("@type",SqlDbType.VarChar),
        //          new SqlParameter("@ReminderMessageTypeID",SqlDbType.Int),
        //            new SqlParameter("@VisitorUserID",SqlDbType.Int),
        //              new SqlParameter("@ReminderTitle",SqlDbType.VarChar),
        //                new SqlParameter("@ReminderTime",SqlDbType.DateTime),
        //                   new SqlParameter("@ReminderContent",SqlDbType.VarChar),
        //                     new SqlParameter("@Url",SqlDbType.VarChar),
        //          };
        //    mySqlParameters[0].Value = "InsertVisitorAlertMessage";
        //    mySqlParameters[1].Value = ReminderMessageTypeID;
        //    mySqlParameters[2].Value = VisitorUserID;
        //    mySqlParameters[3].Value = ReminderTitle;
        //    mySqlParameters[4].Value = ReminderTime;
        //    mySqlParameters[5].Value = ReminderContent;
        //    mySqlParameters[6].Value = Url;
        //    int i
        //         = myDALPublic.DAL_EditInt_Par("InsertMessageModels", mySqlParameters);
        //    return i;
        //}

        //查询消息
        public DataTable SelectNewMessage()
        {
            SqlParameter[] mySqlParameters
                = {
                new SqlParameter("@type",SqlDbType.VarChar),
                  };
            mySqlParameters[0].Value = "SelectNewMessage";
            DataTable dt
                 = myDALPublic.DAL_SelectDT_Par("MessageModels", mySqlParameters);
            return dt;
        }

    }
}
