using HPSocketCS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocketDemo;
using WebSocketServerRun.BLL;
using WebSocketServerRun.Common;
using WebSocketServerRun.myClass;

namespace WebSocketServerRun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        MyWebSocketServer myWebSocketServer;
        public Dictionary<IntPtr, MyUser> dUser = new Dictionary<IntPtr, MyUser>();
        #region 日志输出
        public delegate void WriteControl(string Message);
        public void WriteMessage(string Message)
        {
            this.BeginInvoke(new WriteControl(Write), new object[] { Message });
        }
        private void Write(string Message)
        {
            try
            {
                if (Message.Length == 0)
                {
                    return;
                }
                if (this.txtMsg.Text.Length > 10000)
                    this.txtMsg.Clear();
                this.txtMsg.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss：ffff") + "]：" + Message + "\r\n");
                this.txtMsg.ScrollToCaret();
            }
            catch
            {

            }
        }
        #endregion
        #region 消息监听
        public delegate void dMsgEvent(baseInfo info, IntPtr connId);
        public void MsgEvent(baseInfo info, IntPtr connId)
        {
            this.BeginInvoke(new dMsgEvent(EventInfo), new object[] { info, connId });
        }
        /// <summary>
        /// 处理客户端发送的消息
        /// </summary>
        /// <param name="info"></param>
        public void EventInfo(baseInfo info, IntPtr connId)
        {
            if (info == null)
            {
                return;
            }
            try
            {
                object data = info.data;

                switch (info.msgType)
                {
                    case emMsgType.Type1:
                        {
                            #region 客户端上线
                            WriteMessage("用户ID：" + info.myUser.id + "，名称：" + info.myUser.name + " 通知服务器上线了！");
                            Userlogin(info, connId);
                            #endregion
                        }
                        break;
                    case emMsgType.Type2:
                        {
                            #region 客户端下线
                            WriteMessage("用户ID：" + info.myUser.id + "，名称：" + info.myUser.name + " 通知服务器要下线了！");
                            UserOut(info, connId);
                            #endregion
                        }
                        break;
                    case emMsgType.Type3:
                        {
                            #region 客户端发送普通文本给服务器
                            string str = info.data.ToString();
                            MsgTxt myMsgTxt = Newtonsoft.Json.JsonConvert.DeserializeObject<MsgTxt>(str);
                            WriteMessage("用户ID：" + info.myUser.id + "，名称：" + info.myUser.name + " 发送普通消息：" + myMsgTxt.txt + "");
                            #endregion
                        }
                        break;
                    case emMsgType.Type4:
                        {
                            #region 客户端发送普通文本给其他的客户端
                            string str = info.data.ToString();
                            MsgAppointSendUserText myMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<MsgAppointSendUserText>(str);
                            List<IntPtr> list = GetUserIPByInfo(myMsg.toUser);
                            SendMsgAppointSendUserText(list, info.myUser, myMsg);
                            #endregion
                        }
                        break;
                    case emMsgType.Type5:
                        {
                            #region 服务器关闭，通知客户端

                            #endregion
                        }
                        break;
                    case emMsgType.Type6:
                        {
                            #region 服务器推送普通文本消息给客户端

                            #endregion
                        }
                        break;
                    case emMsgType.Type7:
                        {
                            #region 客户端告诉服务器，发布的商品
                            string json = info.data.ToString();
                            MsgAppointSendUserCommodity myMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<MsgAppointSendUserCommodity>(json);
                            if (myMsg != null)
                            {
                                if (myMsg.fromUser == null)
                                {
                                    myMsg.fromUser = info.myUser;
                                }
                                WriteMessage("用户ID：" + info.myUser.id + "，名称：" + info.myUser.name + " 用户告诉服务器发布的商品：" + myMsg.title + "");
                                //调用方法去处理商品的信息的推送
                                #region 
                                AppointSendUserCommodity(myMsg);
                                #endregion
                            }
                            #endregion
                        }
                        break;
                    case emMsgType.Type8:
                        {
                            #region 服务器发送普通文本给客户端

                            #endregion
                        }
                        break;
                    case emMsgType.Type9:
                        {
                            #region 服务器推送在线的用户列表给客户端

                            #endregion
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
            }

        }
        #endregion

        //load事件
        private void Form1_Load(object sender, EventArgs e)
        {
            string ip = Utils.GetCCFlowAppCenterDB("IP"); ;
            string port = Utils.GetCCFlowAppCenterDB("Port"); ;
            txtIP.Text = ip;
            txtPort.Text = port;
        }

        //关闭事件
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否退出程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
            {
                button2_Click(null, null);
                this.Dispose();
            }
            else
            {
                e.Cancel = true;
            }
        }
        //开启
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                button2.Enabled = false;
                string ip = txtIP.Text.Trim();
                ushort port = (ushort)Convert.ToInt16(txtPort.Text.Trim());
                myWebSocketServer = new MyWebSocketServer(this);
                myWebSocketServer.Run(ip, port);
                WriteMessage("开启成功！");
                button1.Enabled = false;
                button2.Enabled = true;
                startClearAccept();//开启清除多余的连接
                Utils.SetCCFlowAppCenterDB(ip, "IP");
                Utils.SetCCFlowAppCenterDB(port.ToString(), "Port");
            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                button2.Enabled = false;
                myWebSocketServer = null;
                WriteMessage(ex.Message);
            }
        }
        void Work()
        {
            baseInfo mybaseInfo = new baseInfo();
            mybaseInfo.myUser = new MyUser { id = "Administrator", name = Guid.NewGuid().ToString("N"), strTime = DateTime.Now.ToShortTimeString() };
            mybaseInfo.msgType = emMsgType.Type4;
            mybaseInfo.data = JsonConvert.SerializeObject(mybaseInfo.myUser);
            string kson = JsonConvert.SerializeObject(mybaseInfo);
            myClass.TimerSendUtil.send(myWebSocketServer, kson);
        }
        //关闭
        private void button2_Click(object sender, EventArgs e)
        {
            endClearAccept();//关闭清除多余的连接
            try
            {
                if (myWebSocketServer != null)
                {
                    IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                    if (arr != null && arr.Length > 0)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            IntPtr connId = arr[i];

                            if (dUser.ContainsKey(connId))
                            {
                                MyUser myUser = dUser[connId];
                                CompulsoryDownline(myUser, connId, "服务器关闭", emCompulsoryDownline.Type2);
                            }
                            else
                            {//不存在用户记录里面
                                var state = myWebSocketServer.wsServer.GetWSMessageState(connId);//获取用户状态
                                if (state != null && state.OperationCode != WSOpcode.Close)
                                {
                                    bool b = myWebSocketServer.wsServer.Disconnect(connId);//断开连接
                                    if (b)
                                    {
                                        myWebSocketServer.wsServer.Release(connId);//释放连接
                                    }
                                }
                                else
                                {
                                    myWebSocketServer.wsServer.Release(connId);//释放连接
                                }
                            }
                        }
                    }
                    myWebSocketServer.wsServer.Destroy();
                    WriteMessage("关闭成功！");
                }
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
            }
            finally
            {
                button1.Enabled = true;
                button2.Enabled = false;
            }

        }

        /// <summary>
        /// 客户上线
        /// </summary>
        /// <param name="info"></param>
        /// <param name="connId"></param>
        void Userlogin(baseInfo info, IntPtr connId)
        {
            bool u = false;
            try
            {
                if (dUser.Count <= 0)
                {
                    MyUser MyUser = info.myUser;
                    MyUser.time = DateTime.Now;
                    MyUser.strTime = MyUser.time.ToString("yyyy/MM/dd HH:mm:ss");
                    dUser.Add(connId, MyUser);
                    u = true;
                }
                else
                {
                    if (dUser.ContainsKey(connId))
                    {
                        //已经存在
                        MyUser MyUser = dUser[connId];
                        if (info.myUser.id != MyUser.id
                            || info.myUser.type != MyUser.type)
                        {
                            dUser[connId] = info.myUser;//更换的用户
                        }
                        else
                        {
                            if (dUser[connId].name != info.myUser.name)
                                dUser[connId].name = info.myUser.name;
                        }
                        u = true;
                    }
                    else
                    {
                        #region 判断该用户是否已经上线。可能不是他 ，不处理。可以台手机登录          
                        List<IntPtr> list = dUser.Keys.ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            IntPtr ip = list[i];
                            MyUser myUser = dUser[ip];
                            if (info.myUser.id == myUser.id
                           && info.myUser.type == myUser.type)
                            {
                                //已经上线，不允许多台设备登录
                                //CompulsoryDownline(info.myUser, connId, "上次登录时间：" + myUser.strTime,emCompulsoryDownline.Type1);
                                // return;
                            }
                        }
                        #endregion
                        #region 添加用户
                        MyUser MyUser = info.myUser;
                        MyUser.time = DateTime.Now;
                        MyUser.strTime = MyUser.time.ToString("yyyy/MM/dd HH:mm:ss");
                        dUser.Add(connId, MyUser);
                        #endregion
                        u = true;
                    }
                }
                UpdateUserDGV();
                Thread thread = new Thread(new ThreadStart(Work));
                thread.Start();
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
            }
            finally
            {
                if (u)
                    PushUserList();
            }
        }

        /// <summary>
        /// 客户下线
        /// </summary>
        /// <param name="info"></param>
        /// <param name="connId"></param>
        void UserOut(baseInfo info, IntPtr connId)
        {
            UserOut(info.myUser, connId);
        }

        /// <summary>
        /// 客户下线
        /// </summary>
        /// <param name="myUser"></param>
        /// <param name="connId"></param>
        void UserOut(MyUser myUser, IntPtr connId)
        {
            bool u = false;
            try
            {
                if (dUser.Count <= 0)
                {

                }
                else if (dUser.ContainsKey(connId))
                {
                    dUser.Remove(connId);
                    u = true;
                }
            }
            catch
            {

            }
            finally
            {
                if (u)
                {
                    UpdateUserDGV();
                    PushUserList();
                }

            }
        }

        /// <summary>
        /// 更新table
        /// </summary>
        void UpdateUserDGV()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("id", typeof(string));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("type", typeof(string));
                dt.Columns.Add("time", typeof(string));
                dt.Columns.Add("IntPtr", typeof(IntPtr));
                List<IntPtr> list = dUser.Keys.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    IntPtr ip = list[i];
                    MyUser myUser = dUser[ip];
                    DataRow dr = dt.NewRow();
                    dr["id"] = myUser.id;
                    dr["name"] = myUser.name;
                    dr["type"] = myUser.type;
                    dr["time"] = myUser.time.ToString();
                    dr["IntPtr"] = ip;
                    dt.Rows.Add(dr);
                }
                dgvUser.DataSource = dt;
            }
            catch (Exception ex)
            {
                WriteMessage("更新用户出错！" + ex.Message);
            }
        }

        /// <summary>
        /// 根据用户信息查询 IntPtr 
        /// </summary>
        /// <param name="myUser"></param>
        /// <returns></returns>
        public List<IntPtr> GetUserIPByInfo(MyUser myUser)
        {
            List<IntPtr> list = new List<System.IntPtr>();
            try
            {
                List<IntPtr> listIP = dUser.Keys.ToList();
                for (int i = 0; i < listIP.Count; i++)
                {
                    IntPtr ip = listIP[i];
                    MyUser myUser2 = dUser[ip];
                    if (myUser.id == myUser2.id && myUser.type == myUser2.type)
                    {
                        list.Add(ip);
                    }
                }
            }
            catch
            {

            }
            return list;
        }

        /// <summary>
        /// 指定发送给其他客户的文本消息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="myMsg"></param>
        public void SendMsgAppointSendUserText(List<IntPtr> list, MyUser myUser, MsgAppointSendUserText myMsg)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    IntPtr connId = list[i];
                    baseInfo mybaseInfo = new baseInfo();
                    mybaseInfo.myUser = myMsg.toUser;
                    mybaseInfo.msgType = emMsgType.Type4;
                    mybaseInfo.data = JsonConvert.SerializeObject(myMsg);
                    string kson = JsonConvert.SerializeObject(mybaseInfo);
                    // 获取客户端的state
                    var state = myWebSocketServer.wsServer.GetWSMessageState(connId);
                    if (state != null && state.OperationCode != WSOpcode.Close)
                    {
                        // 原样返回给客户端
                        bool b = myWebSocketServer.wsServer.SendWSMessage(connId, state, kson);
                        if (b)
                        {
                            //成功
                            WriteMessage("指定发送给其他客户的文本消息成功:" + "用户：" + myMsg.toUser.name + "！");
                        }
                        else
                        {
                            //失败
                            WriteMessage("指定发送给其他客户的文本消息失败:" + "用户：" + myMsg.toUser.name + "！");
                        }
                    }
                    else
                    {
                        //
                        WriteMessage("指定发送给其他客户的文本消息错误:" + "获取用户：" + myMsg.toUser.name + "的状态信息失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteMessage("指定发送给其他客户的文本消息异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (dUser.Count <= 0)
            {
                WriteMessage("无用户！");
                return;
            }
            string txt = txt1.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                WriteMessage("请输入内容！");
                return;
            }
            bool cb = checkBox1.Checked;
            if (cb == false)
            {
                if (dgvUser.CurrentRow == null || dgvUser.CurrentRow.Index < 0)
                {
                    WriteMessage("请选择用户！");
                    return;
                }
                IntPtr IntPtr = (IntPtr)dgvUser.Rows[dgvUser.CurrentRow.Index].Cells["IntPtr"].Value;
                MyUser myUser2 = dUser[IntPtr];
                MsgTxt myMsgTxt = new MsgTxt();
                myMsgTxt.txt = txt;
                baseInfo mybaseInfo = new baseInfo();
                mybaseInfo.myUser = myUser2;
                mybaseInfo.msgType = emMsgType.Type8;
                mybaseInfo.data = JsonConvert.SerializeObject(myMsgTxt);
                SendMsgAppointSendUserText(IntPtr, mybaseInfo);
            }
            else
            {
                List<IntPtr> listIP = dUser.Keys.ToList();
                for (int i = 0; i < listIP.Count; i++)
                {
                    IntPtr ip = listIP[i];
                    MyUser myUser2 = dUser[ip];
                    MsgTxt myMsgTxt = new MsgTxt();
                    myMsgTxt.txt = txt;
                    baseInfo mybaseInfo = new baseInfo();
                    mybaseInfo.myUser = myUser2;
                    mybaseInfo.msgType = emMsgType.Type8;
                    mybaseInfo.data = JsonConvert.SerializeObject(myMsgTxt);
                    SendMsgAppointSendUserText(ip, mybaseInfo);
                }
            }

        }

        /// <summary>
        /// 推送商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (dUser.Count <= 0)
            {
                WriteMessage("无用户！");
                return;
            }
            string txt = txt1.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                WriteMessage("请输入内容！");
                return;
            }
            bool cb = checkBox1.Checked;
            if (cb == false)
            {
                if (dgvUser.CurrentRow == null || dgvUser.CurrentRow.Index < 0)
                {
                    WriteMessage("请选择用户！");
                    return;
                }
                IntPtr IntPtr = (IntPtr)dgvUser.Rows[dgvUser.CurrentRow.Index].Cells["IntPtr"].Value;
                MyUser myUser2 = dUser[IntPtr];
                MsgAppointSendUserCommodity myMsgAppointSendUserCommodity = new MsgAppointSendUserCommodity();
                myMsgAppointSendUserCommodity.fromUser = null;
                myMsgAppointSendUserCommodity.AppointType = "2";
                myMsgAppointSendUserCommodity.commodityID = "01";
                myMsgAppointSendUserCommodity.title = txt;
                myMsgAppointSendUserCommodity.many = 10.00;
                myMsgAppointSendUserCommodity.describe = "";
                myMsgAppointSendUserCommodity.address = "测试地址";
                myMsgAppointSendUserCommodity.latitude = 116;
                myMsgAppointSendUserCommodity.longitude = 23.333;
                myMsgAppointSendUserCommodity.time = "2018/01/01 00:00:11";
                myMsgAppointSendUserCommodity.remarks = "测试";
                baseInfo mybaseInfo = new baseInfo();
                mybaseInfo.myUser = myUser2;
                mybaseInfo.msgType = emMsgType.Type11;
                mybaseInfo.data = JsonConvert.SerializeObject(myMsgAppointSendUserCommodity);
                SendMsgAppointSendUserText(IntPtr, mybaseInfo);
            }
            else
            {
                List<IntPtr> listIP = dUser.Keys.ToList();
                for (int i = 0; i < listIP.Count; i++)
                {
                    IntPtr ip = listIP[i];
                    MyUser myUser2 = dUser[ip];
                    MsgAppointSendUserCommodity myMsgAppointSendUserCommodity = new MsgAppointSendUserCommodity();
                    myMsgAppointSendUserCommodity.fromUser = null;
                    myMsgAppointSendUserCommodity.AppointType = "2";
                    myMsgAppointSendUserCommodity.commodityID = "01";
                    myMsgAppointSendUserCommodity.title = txt;
                    myMsgAppointSendUserCommodity.many = 10.00;
                    myMsgAppointSendUserCommodity.describe = "";
                    myMsgAppointSendUserCommodity.address = "测试地址";
                    myMsgAppointSendUserCommodity.latitude = 116;
                    myMsgAppointSendUserCommodity.longitude = 23.333;
                    myMsgAppointSendUserCommodity.time = "2018/01/01 00:00:11";
                    myMsgAppointSendUserCommodity.remarks = "测试";
                    baseInfo mybaseInfo = new baseInfo();
                    mybaseInfo.myUser = myUser2;
                    mybaseInfo.msgType = emMsgType.Type11;
                    mybaseInfo.data = JsonConvert.SerializeObject(myMsgAppointSendUserCommodity);
                    SendMsgAppointSendUserText(ip, mybaseInfo);
                }
            }
        }

        /// <summary>
        /// 处理客户端发布的商品推送
        /// </summary>
        /// <param name="myMsg"></param>
        public void AppointSendUserCommodity(MsgAppointSendUserCommodity myMsg)
        {
            MyUser fromUser = myMsg.fromUser;
            //假设推送给全部的用户
            try
            {
                if (myWebSocketServer != null)
                {

                    IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                    if (arr == null)
                    {
                        return;
                    }

                    for (int i = 0; i < arr.Length; i++)
                    {
                        IntPtr connId = arr[i];
                        if (dUser.ContainsKey(connId))
                        {
                            MyUser myUser2 = dUser[connId];
                            if (fromUser.id == myUser2.id &&
                                fromUser.type == myUser2.type)
                            {
                                continue;//不用推送给自己了
                            }
                            baseInfo mybaseInfo = new baseInfo();
                            mybaseInfo.myUser = myUser2;
                            mybaseInfo.msgType = emMsgType.Type11;
                            mybaseInfo.data = JsonConvert.SerializeObject(myMsg);
                            SendMsgAppointSendUserText(connId, mybaseInfo);
                        }
                    }
                    WriteMessage("推送商品成功！");
                }
            }
            catch (Exception ex)
            {
                WriteMessage("推送商品异常：" + ex.Message);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 发送消息给客户
        /// </summary>
        /// <param name="list"></param>
        /// <param name="myMsg"></param>
        public bool SendMsgAppointSendUserText(IntPtr connId, baseInfo myMsg)
        {
            string kson = JsonConvert.SerializeObject(myMsg);
            // 获取客户端的state
            var state = myWebSocketServer.wsServer.GetWSMessageState(connId);
            if (state != null && state.OperationCode != WSOpcode.Close)
            {
                bool b = myWebSocketServer.wsServer.SendWSMessage(connId, state, kson);
                return b;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 推送用户列表给客户端
        /// </summary>
        public void PushUserList()
        {
            if (dUser.Count <= 0)
            {
                WriteMessage("无用户！");
                return;
            }
            try
            {
                List<IntPtr> listIP = dUser.Keys.ToList();
                List<MyUser> listUser = new List<MyUser>();
                //查询用户列表
                for (int i = 0; i < listIP.Count; i++)
                {
                    IntPtr ip = listIP[i];
                    MyUser myUser = dUser[ip];
                    listUser.Add(myUser);
                }
                //发送用户列表
                for (int i = 0; i < listIP.Count; i++)
                {
                    IntPtr ip = listIP[i];
                    MyUser myUser = dUser[ip];
                    baseInfo mybaseInfo = new baseInfo();
                    mybaseInfo.myUser = myUser;
                    mybaseInfo.msgType = emMsgType.Type9;
                    mybaseInfo.data = JsonConvert.SerializeObject(listUser);
                    SendMsgAppointSendUserText(ip, mybaseInfo);
                }
            }
            catch (Exception ex)
            {
                WriteMessage("推送用户列表给客户端异常：" + ex.Message + "！");
            }
        }

        /// <summary>
        /// 强制用户下线
        /// </summary>
        /// <param name="myUser"></param>
        public void CompulsoryDownline(MyUser newUser, IntPtr connId, string txt, emCompulsoryDownline type)
        {
            try
            {
                MsgCompulsoryDownline myMsgTxt = new MsgCompulsoryDownline();
                myMsgTxt.txt = txt;
                myMsgTxt.type = type;
                baseInfo info = new baseInfo();
                info.myUser = newUser;
                info.msgType = emMsgType.Type10;
                info.data = JsonConvert.SerializeObject(myMsgTxt);
                SendMsgAppointSendUserText(connId, info);
                bool b = myWebSocketServer.wsServer.Disconnect(connId);
                UserOut(newUser, connId);
                WriteMessage("用户ID：" + newUser.id + "，名称：" + newUser.name + " 强制下线！");
                myWebSocketServer.wsServer.Release(connId);//释放连接
            }
            catch (Exception ex)
            {
                WriteMessage("强制下线发生异常：" + ex.Message);
            }
        }

        //测试连接人数
        private void button4_Click(object sender, EventArgs e)
        {
            if (myWebSocketServer == null)
            {
                return;
            }
            if (myWebSocketServer.wsServer == null)
            {
                return;
            }
            if (myWebSocketServer.wsServer.State != ServiceState.Started)
            {
                return;
            }
            try
            {
                IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                int Length = arr == null ? 0 : arr.Length;
                MessageBox.Show("连接的人数：" + Length.ToString());
                myWebSocketServer.wsServer.MaxConnectionCount = (uint)Convert.ToInt32(textBox1.Text);
                uint count = myWebSocketServer.wsServer.MaxConnectionCount;
                MessageBox.Show("最大连接的人数：" + count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //清空日志输出
        private void txtMsg_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            // 清理listbox
            if (key == Keys.Delete)
            {
                if (MessageBox.Show("是否清空日志输出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.txtMsg.Clear();
                }
            }
        }


        #region 定时清除多余的连接
        Dictionary<IntPtr, int> dicCount = new Dictionary<IntPtr, int>();
        /// <summary>
        /// 清除多余的连接 线程
        /// </summary>
        Thread thClearAccept = null;
        /// <summary>
        /// 清除多余的连接 线程状态
        /// </summary>
        bool bClearAccept = false;

        /// <summary>
        /// 开启清除多余的连接 线程
        /// </summary>
        void startClearAccept()
        {
            if (bClearAccept)
            {
                try
                {
                    if (thClearAccept != null)
                    {
                        thClearAccept.Abort();
                        thClearAccept = null;
                    }
                }
                catch
                {

                }
            }
            bClearAccept = true;
            try
            {
                thClearAccept = new Thread(new ThreadStart(ClearAccept));
                thClearAccept.IsBackground = true;
                thClearAccept.Start();
            }
            catch (Exception ex)
            {
                this.WriteMessage("清除多余的连接线程开启异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 停止清除多余的连接 线程
        /// </summary>
        void endClearAccept()
        {
            bClearAccept = false;
            try
            {
                if (thClearAccept != null)
                {
                    thClearAccept.Abort();
                    thClearAccept = null;
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage("清除多余的连接线程关闭异常：" + ex.Message);
            }
        }

        void ClearAccept()
        {
            while (bClearAccept)
            {
                try
                {
                    if (myWebSocketServer == null)
                    {
                        continue;
                    }
                    if (myWebSocketServer.wsServer == null)
                    {
                        continue;
                    }
                    if (myWebSocketServer.wsServer.State != ServiceState.Started)
                    {
                        continue;
                    }
                    IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                    if (arr == null || arr.Length <= 0)
                    {
                        continue;
                    }
                    List<IntPtr> list = new List<IntPtr>();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        bool b = false;
                        IntPtr ip = arr[i];
                        if (dUser.Count > 0)
                        {
                            if (dUser.ContainsKey(ip))
                            {
                                b = true;//存在用户之中
                            }
                        }
                        if (b == false)
                        {
                            list.Add(ip);
                        }
                    }

                    //处理

                    //移除，已经上线的用户
                    List<IntPtr> list2 = dicCount.Keys.ToList();
                    for (int i = 0; i < list2.Count; i++)
                    {
                        IntPtr ip = list2[i];
                        if (!list.Contains(ip))
                        {
                            dicCount.Remove(ip);//移除
                        }
                    }

                    //叠加次数
                    for (int i = 0; i < list.Count; i++)
                    {
                        IntPtr ip = list[i];
                        if (dicCount.ContainsKey(ip))
                        {
                            dicCount[ip] = dicCount[ip] + 1;//修改
                        }
                        else
                        {
                            dicCount.Add(ip, 1);//添加
                        }
                    }

                    //断开连接 释放连接
                    foreach (var item in dicCount)
                    {
                        if (item.Value > 6)//五次的时间，都没有出现在用户里面
                        {
                            //强制的下线
                            myWebSocketServer.wsServer.Disconnect(item.Key);//断开连接
                            myWebSocketServer.wsServer.Release(item.Key);//释放连接
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage("清除多余的连接 执行线程异常：" + ex.Message);
                }
                Thread.Sleep(1 * 10000);
            }
        }
        #endregion

        #region 消息推送
        BLL_Message myMessage = new BLL_Message();
        /// <summary>
        /// 消息推送 线程
        /// </summary>
        Thread thMessagePush = null;
        /// <summary>
        /// 消息推送 线程状态
        /// </summary>
        bool bMessagePush = false;

        /// <summary>
        /// 开启消息推送 线程
        /// </summary>
        void startMessagePush()
        {
            if (bMessagePush)
            {
                try
                {
                    if (thMessagePush != null)
                    {
                        thMessagePush.Abort();
                        thMessagePush = null;
                    }
                }
                catch
                {

                }
            }
            bMessagePush = true;
            button7.Enabled = false;
            button6.Enabled = false;
            try
            {
                thMessagePush = new Thread(new ThreadStart(MessagePush));
                thMessagePush.IsBackground = true;
                thMessagePush.Start();

                button7.Enabled = false;
                button6.Enabled = true;
            }
            catch (Exception ex)
            {
                button7.Enabled = true;
                button6.Enabled = false;
                this.WriteMessage("消息推送线程开启异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 停止消息推送 线程
        /// </summary>
        void endMessagePush()
        {
            bMessagePush = false;
            button7.Enabled = false;
            button6.Enabled = false;
            try
            {
                if (thMessagePush != null)
                {
                    thMessagePush.Abort();
                    thMessagePush = null;
                }
                button7.Enabled = true;
                button6.Enabled = false;
            }
            catch (Exception ex)
            {
                button7.Enabled = false;
                button6.Enabled = true;
                this.WriteMessage("消息推送线程关闭异常：" + ex.Message);
            }
        }

        void MessagePush()
        {
            while (bMessagePush)
            {
                try
                {
                    if (myWebSocketServer == null)
                    {
                        continue;
                    }
                    if (myWebSocketServer.wsServer == null)
                    {
                        continue;
                    }
                    if (myWebSocketServer.wsServer.State != ServiceState.Started)
                    {
                        continue;
                    }
                    IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                    if (arr == null || arr.Length <= 0)
                    {
                        continue;
                    }
                    DataTable dt = myMessage.SelectNewMessage();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string txt = "";// dt.Rows[j][""].ToString()

                        List<IntPtr> listIP = dUser.Keys.ToList();
                        for (int i = 0; i < listIP.Count; i++)
                        {
                            IntPtr ip = listIP[i];
                            MyUser myUser2 = dUser[ip];
                            MsgAppointSendUserCommodity myMsgAppointSendUserCommodity = new MsgAppointSendUserCommodity();
                            myMsgAppointSendUserCommodity.fromUser = null;
                            myMsgAppointSendUserCommodity.AppointType = "2";
                            myMsgAppointSendUserCommodity.commodityID = "01";
                            myMsgAppointSendUserCommodity.title = txt;
                            myMsgAppointSendUserCommodity.many = 10.00;
                            myMsgAppointSendUserCommodity.describe = "";
                            myMsgAppointSendUserCommodity.address = "测试地址";
                            myMsgAppointSendUserCommodity.latitude = 116;
                            myMsgAppointSendUserCommodity.longitude = 23.333;
                            myMsgAppointSendUserCommodity.time = "2018/01/01 00:00:11";
                            myMsgAppointSendUserCommodity.remarks = "测试";
                            baseInfo mybaseInfo = new baseInfo();
                            mybaseInfo.myUser = myUser2;
                            mybaseInfo.msgType = emMsgType.Type11;
                            mybaseInfo.data = JsonConvert.SerializeObject(myMsgAppointSendUserCommodity);
                            SendMsgAppointSendUserText(ip, mybaseInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage("消息推送 执行线程异常：" + ex.Message);
                }
                Thread.Sleep(1 * 10000);
            }
        }

        //开启消息推送
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (myWebSocketServer == null)
                {
                    MessageBox.Show("未开启服务器");
                    return;
                }
                if (myWebSocketServer.wsServer == null)
                {
                    MessageBox.Show("未开启服务器");
                    return;
                }
                if (myWebSocketServer.wsServer.State != ServiceState.Started)
                {
                    MessageBox.Show("未开启服务器");
                    return;
                }
                if (bMessagePush)
                {
                    MessageBox.Show("已开启");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            startMessagePush();
        }

        //关闭消息推送
        private void button6_Click(object sender, EventArgs e)
        {
            if (!bMessagePush)
            {
                MessageBox.Show("未开启");
                return;
            }
            endMessagePush();
        }
        #endregion


    }
}
