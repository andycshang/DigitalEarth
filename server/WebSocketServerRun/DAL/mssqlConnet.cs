using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using WebSocketServerRun.Common;

namespace DBconnet
{
    public class mssqlConnet
    {
        // 定义连接字符串:数据服务器目标

        //本地服务器
        public static string sqlcnnstr = "";
        //Catalog="数据库名称"

        /// <summary>
        /// 设置数据库连接
        /// </summary>
        public static void SetSqlcnnstr()
        {
            mssqlConnet.sqlcnnstr = Utils.GetCCFlowAppCenterDB("DBconnet");//设置数据库连接
        }

        #region 初始化相关ADO.NET变量
        SqlConnection sqlcn;//定义连接对象
        SqlCommand sqlcmd;//定义命令对象
        SqlDataAdapter sqlda;//定义数据适配器
        DataTable dt;//定义数据表
        #endregion


        #region 提取数据的ADO.NET通用方法
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="mysqlstr">存储过程文件名称</param>
        /// <param name="SQlCMDpas">参数</param>
        /// <returns></returns>
        public DataTable DAL_SelectDT_Par(string mysqlstr, SqlParameter[] SQlCMDpas)
        {
            //第一步SqlConnection：创建数据库连接类SqlConnection的对象sqlcn，好比修建湛江到广州的高速公路
            sqlcn = new SqlConnection(sqlcnnstr);
            //SqlConnection sqlcn = new SqlConnection();
            //第二步SqlCommand A：创建命令类SqlCommand的对象sqlcmd，好比安排运输计划：运输车和货物(SQL命令)，运输通道sqlcn
            sqlcmd = new SqlCommand(mysqlstr, sqlcn);
            //第二步SqlCommand B：设置命令对象执行的SQL代码类型，此处是执行数据库中存储过程
            sqlcmd.CommandType = CommandType.StoredProcedure;

            //第二步SqlCommand C：把外部传递过来的SQL命令对应的参数填充到SqlCommand对象sqlcmd的SqlParameters集合中   
            foreach (SqlParameter var in SQlCMDpas)
            {
                sqlcmd.Parameters.Add(var);
            }
            //准备好本地数据容器
            this.dt = new DataTable();
            //第三步SqlDataAdapter：用数据适配器SqlDataAdapter对象sqlda执行SqlCommand对象sqlcmd；适配器SqlDataAdapter好比高速路管理公司
            sqlda = new SqlDataAdapter(sqlcmd);//SqlDataAdapter可以隐式打开和关闭SqlConnection
            //第四步：将执行后的数据结果返回到DataTable对象dt中
            sqlda.Fill(this.dt);
            return this.dt;
        }
        #endregion

        #region 提取数据的ADO.NET通用方法
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="mysqlstr">存储过程文件名称</param>
        /// <param name="SQlCMDpas">参数</param>
        /// <param name="sqlcnnstr">数据库连接</param>
        /// <returns>成功返回查询结果，失败抛出出错</returns>
        public DataTable DAL_SelectDT_Par(string mysqlstr, SqlParameter[] SQlCMDpas, string sqlcnnstr)
        {
            //第一步SqlConnection：创建数据库连接类SqlConnection的对象sqlcn，好比修建湛江到广州的高速公路
            sqlcn = new SqlConnection(sqlcnnstr);
            //SqlConnection sqlcn = new SqlConnection();
            //第二步SqlCommand A：创建命令类SqlCommand的对象sqlcmd，好比安排运输计划：运输车和货物(SQL命令)，运输通道sqlcn
            sqlcmd = new SqlCommand(mysqlstr, sqlcn);
            //第二步SqlCommand B：设置命令对象执行的SQL代码类型，此处是执行数据库中存储过程
            sqlcmd.CommandType = CommandType.StoredProcedure;

            //第二步SqlCommand C：把外部传递过来的SQL命令对应的参数填充到SqlCommand对象sqlcmd的SqlParameters集合中   
            foreach (SqlParameter var in SQlCMDpas)
            {
                sqlcmd.Parameters.Add(var);
            }
            //准备好本地数据容器
            this.dt = new DataTable();
            //第三步SqlDataAdapter：用数据适配器SqlDataAdapter对象sqlda执行SqlCommand对象sqlcmd；适配器SqlDataAdapter好比高速路管理公司
            sqlda = new SqlDataAdapter(sqlcmd);//SqlDataAdapter可以隐式打开和关闭SqlConnection
            //第四步：将执行后的数据结果返回到DataTable对象dt中
            sqlda.Fill(this.dt);
            return this.dt;
        }
        #endregion

        #region   //插入、更新、删除数据库中的ADO.NET通用方法
        public int DAL_EditInt_Par(string mysqlstr, SqlParameter[] SQlCMDpas)
        {
            //第一步SqlConnection：创建数据库连接类SqlConnection的对象sqlcn，并显示打开；好比修建湛江到广州的高速公路
            sqlcn = new SqlConnection(sqlcnnstr.ToString());
            sqlcn.Open();
            //第二步SqlCommand A：创建命令类SqlCommand的对象sqlcmd，好比安排运输计划：运输车和货物(SQL命令)，运输通道sqlcn
            sqlcmd = new SqlCommand(mysqlstr, sqlcn);
            //第二步SqlCommand B：设置命令对象执行的SQL代码类型，此处是执行数据库中存储过程
            sqlcmd.CommandType = CommandType.StoredProcedure;
            //第二步SqlCommand C：把外部传递过来的SQL命令对应的参数填充到SqlCommand对象sqlcmd的SqlParameters集合中   
            foreach (SqlParameter var in SQlCMDpas)
            {
                sqlcmd.Parameters.Add(var);
            }
            //第三步 SqlCommand ：SqlCommand对象sqlcmd自己执行ExecuteNonQuery()调用SQL存储过程操作数据库
            int myop = sqlcmd.ExecuteNonQuery();
            sqlcn.Close();
            return myop;
        }
        #endregion

        #region 事务  插入、更新、删除数据库中的ADO.NET通用方法 
        public int DAL_EditInt_Par(string mysqlstr, SqlConnection sqlcn, SqlTransaction sqlTransaction, SqlParameter[] SQlCMDpas)
        {
            //第二步SqlCommand A：创建命令类SqlCommand的对象sqlcmd，好比安排运输计划：运输车和货物(SQL命令)，运输通道sqlcn
            sqlcmd = new SqlCommand(mysqlstr, sqlcn, sqlTransaction);
            //第二步SqlCommand B：设置命令对象执行的SQL代码类型，此处是执行数据库中存储过程
            sqlcmd.CommandType = CommandType.StoredProcedure;
            //第二步SqlCommand C：把外部传递过来的SQL命令对应的参数填充到SqlCommand对象sqlcmd的SqlParameters集合中   
            foreach (SqlParameter var in SQlCMDpas)
            {
                sqlcmd.Parameters.Add(var);
            }
            //第三步 SqlCommand ：SqlCommand对象sqlcmd自己执行ExecuteNonQuery()调用SQL存储过程操作数据库
            int myop = sqlcmd.ExecuteNonQuery();
            return myop;
        }
        #endregion

    }

    public static class DAL
    {

        /// <summary>
        /// 释放事务资源
        /// </summary>
        /// <param name="sqlcn">数据库操作</param>
        /// <param name="sqlTransaction">事务操作</param>
        public static void DisposeAffair(SqlConnection sqlcn, SqlTransaction sqlTransaction)
        {
            if (sqlcn!=null)
            {
                try
                {
                    sqlcn.Close();//关闭操作数据库
                }
                catch
                {

                }
                try
                {
                    sqlcn.Dispose();//释放数据库
                }
                catch
                {

                }
            }
            if (sqlTransaction!=null)
            {
                try
                {
                    sqlTransaction.Dispose();//释放事务操作                   
                }
                    catch
                {

                }
            }
        }
    }
}