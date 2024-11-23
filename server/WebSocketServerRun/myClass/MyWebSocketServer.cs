using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPSocketCS;
using System.Runtime.InteropServices;
using WebSocketServerRun;
using Newtonsoft.Json;
using WebSocketServerRun.myClass;
namespace WebSocketDemo
{
    public class MyWebSocketServer
    {
        public WebSocketServer wsServer = null;

        public MyWebSocketServer(Form1 myForm1)
        {
            this.myForm1 =myForm1;
        }
        Form1 myForm1;

        public void Run(string bindAddress, ushort port)
        {
           
            try
            {
                wsServer = new WebSocketServer();
                wsServer.IpAddress = bindAddress;
                wsServer.Port = port;
                //wsServer.SocketBufferSize =1024*1024*10;
               uint SocketListenQueue= wsServer.SocketListenQueue;
                // 指针形式的事件
                //wsServer.OnPointerWSMessageBody += new WebSocketEvent.OnPointerWSMessageBodyEventHandler(OnPointerDataBody);
                wsServer.OnWSMessageBody += new WebSocketEvent.OnWSMessageBodyEventHandler(OnWSMessageBody);
                wsServer.OnWSMessageComplete += new WebSocketEvent.OnWSMessageCompleteEventHandler(OnWSMessageComplete);
                wsServer.OnWSMessageHeader += new WebSocketEvent.OnWSMessageHeaderEventHandler(OnWSMessageHeader);

                wsServer.OnAccept += WsServer_OnAccept;
                if (!wsServer.Start())
                {
                    throw new MyException(string.Format("启动失败,错误码:{0},错误信息:{1}", wsServer.ErrorCode, wsServer.ErrorMessage));
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        private HandleResult WsServer_OnAccept(IntPtr connId, IntPtr pClient)
        {
            return HandleResult.Ok;
        }

        HandleResult OnPointerDataBody(IntPtr connId, IntPtr data, int length)
        {
            byte[] ys = new byte[length];
            Marshal.Copy(data, ys, 0, length);
            // 如果是文本,应该用utf8编码
            string str = Encoding.UTF8.GetString(ys);
            Console.WriteLine("OnPointerDataBody() -> {0}", str);
            myForm1.WriteMessage(connId.ToInt32() + "，" + "OnPointerDataBody() -> " + str + "");
            // 获取客户端的state
            var state = wsServer.GetWSMessageState(connId);
            if (state != null)
            {
                // 原样返回给客户端
                wsServer.SendWSMessage(connId, state, ys);
            }
            // 指针形式的事件
            return HandleResult.Ok;
        }

        HandleResult OnWSMessageBody(IntPtr connId, byte[] data)
        {
            try
            {
                // 如果是文本,应该用utf8编码
                string text = Encoding.UTF8.GetString(data);
                baseInfo baseInfo =JsonConvert.DeserializeObject<baseInfo>(text);
                if (baseInfo!=null)
                {
                    //处理消息，调用事务，给主程序处理
                    this.myForm1.MsgEvent(baseInfo, connId);
                }
                else
                {//转换失败，定义的消息类型错误
                    MyUser myUser =myForm1.dUser.ContainsKey(connId)? myForm1.dUser [connId] :null;
                    if (myUser==null)
                    {
                        myUser =new MyUser ();
                        myUser.name = "未知用户！";
                        bool b = wsServer.Disconnect(connId);//断开连接
                        if (b)
                        {
                            wsServer.Release(connId);//释放连接
                        }
                    }
                    myForm1.WriteMessage("接收消息："+myUser.name + " 发送的内容解析失败：" + text );
                }
            }
            catch (Exception ex)
            {
                myForm1.WriteMessage("接收消息异常:"+ex.Message);
            } 
            return HandleResult.Ok;
        }

        HandleResult OnWSMessageComplete(IntPtr connId)
        {
            return HandleResult.Ok;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="reserved"></param>
        /// <param name="operationCode"></param>
        /// <param name="mask"></param>
        /// <param name="bodyLength"></param>
        /// <returns></returns>
        HandleResult OnWSMessageHeader(IntPtr connId, bool final, byte reserved, byte operationCode, byte[] mask, ulong bodyLength)
        {
            var state = wsServer.GetWSMessageState(connId);
            //WSOpcode.Close为客户端主动断开连接
            if (state != null)
            {
                if (state.OperationCode == WSOpcode.Close)//关闭了连接
                {
                    wsServer.Disconnect(connId);//断开连接
                    wsServer.Release(connId);//释放连接

                    myForm1.WriteMessage(connId.ToInt32() + "=Close");
                    //处理消息，调用事务，给主程序处理
                    if(myForm1.dUser.ContainsKey(connId))
                    {
                        MyUser MyUser = myForm1.dUser[connId];
                        //处理消息，调用事务，给主程序处理
                        baseInfo myBaseInfo =new baseInfo ();
                        myBaseInfo.myUser = MyUser;
                        myBaseInfo.msgType = emMsgType.Type2;
                        this.myForm1.MsgEvent(myBaseInfo, connId);
                    }
                }
                else if (state.OperationCode == WSOpcode.Cont)
                {
                    //myForm1.WriteMessage(connId.ToInt32() + "=Cont");
                }
                else if (state.OperationCode == WSOpcode.Binary)
                {
                    //myForm1.WriteMessage(connId.ToInt32() + "=Binary");
                }
                else if (state.OperationCode == WSOpcode.Ping)
                {
                   // myForm1.WriteMessage(connId.ToInt32() + "=Ping");
                }
                else if (state.OperationCode == WSOpcode.Pong)
                {
                    //myForm1.WriteMessage(connId.ToInt32() + "=Pong");
                }
                else if (state.OperationCode == WSOpcode.Text)
                {
                   // myForm1.WriteMessage(connId.ToInt32() + "=Text");
                }
            }
            else
            {
                //获取失败
                myForm1.WriteMessage(connId.ToInt32() + "=获取客户端的state失败！");
            }           
            return HandleResult.Ok;
        }


    }


}
