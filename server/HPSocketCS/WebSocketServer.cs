using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace HPSocketCS
{
    public class WebSocketServer : HttpServer
    {
        public event WebSocketEvent.OnWSMessageHeaderEventHandler OnWSMessageHeader;
        public event WebSocketEvent.OnWSMessageBodyEventHandler OnWSMessageBody;
        public event WebSocketEvent.OnPointerWSMessageBodyEventHandler OnPointerWSMessageBody;
        public event WebSocketEvent.OnWSMessageCompleteEventHandler OnWSMessageComplete;

        HttpSdk.OnWSMessageHeader _OnWSMessageHeader;
        HttpSdk.OnWSMessageBody _OnWSMessageBody;
        HttpSdk.OnWSMessageComplete _OnWSMessageComplete;

        protected override void SetCallback()
        {
            base.SetCallback();

            // 设置websocket的callback
            _OnWSMessageHeader = new HttpSdk.OnWSMessageHeader(SDK_OnWSMessageHeader);
            _OnWSMessageBody = new HttpSdk.OnWSMessageBody(SDK_OnWSMessageBody);
            _OnWSMessageComplete = new HttpSdk.OnWSMessageComplete(SDK_OnWSMessageComplete);

            HttpSdk.HP_Set_FN_HttpServer_OnWSMessageHeader(pListener, _OnWSMessageHeader);
            HttpSdk.HP_Set_FN_HttpServer_OnWSMessageBody(pListener, _OnWSMessageBody);
            HttpSdk.HP_Set_FN_HttpServer_OnWSMessageComplete(pListener, _OnWSMessageComplete);
            
        }

        protected virtual HandleResult SDK_OnWSMessageHeader(IntPtr pSender, IntPtr dwConnID, bool bFinal, byte iReserved, byte iOperationCode, byte[] lpszMask, ulong ullBodyLen)
        {
            if (OnWSMessageHeader != null)
            {
                return OnWSMessageHeader(dwConnID, bFinal, iReserved, iOperationCode, lpszMask, ullBodyLen);
            }
            return HandleResult.Ok;
        }

        protected virtual HandleResult SDK_OnWSMessageBody(IntPtr pSender, IntPtr dwConnID, IntPtr pData, int length)
        {
            if (OnPointerWSMessageBody != null)
            {
                return OnPointerWSMessageBody(dwConnID, pData, length);
            }
            else if (OnWSMessageBody != null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnWSMessageBody(dwConnID, bytes);
            }
            return HandleResult.Ok;
        }

        protected virtual HandleResult SDK_OnWSMessageComplete(IntPtr pSender, IntPtr connId)
        {
            if (OnWSMessageComplete != null)
            {
                return OnWSMessageComplete(connId);
            }
            return HandleResult.Ok;
        }


        protected override HttpParseResult SDK_OnUpgrade(IntPtr pSender, IntPtr connId, HttpUpgradeType upgradeType)
        {
            if (upgradeType == HttpUpgradeType.HttpTunnel)
            {
                SendResponse(connId, HttpStatusCode.Ok, "Connection Established", null, null, 0);
            }
            else if (upgradeType == HttpUpgradeType.WebSocket)
            {
                THeader[] headers =
                {
                    new THeader() { Name= "Connection", Value = "Upgrade" },
                    new THeader() { Name= "Upgrade", Value = "WebSocket" },
                    new THeader(),
                    new THeader(),
                };

                var keyName = "Sec-WebSocket-Key";
                var secWebSocketKey = GetHeader(connId, keyName);
                if (string.IsNullOrEmpty(secWebSocketKey))
                {
                    return HttpParseResult.Error;
                }


                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(secWebSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
                byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
                string str_sha1_out = Convert.ToBase64String(bytes_sha1_out);

                headers[2].Name = "Sec-WebSocket-Accept";
                headers[2].Value = str_sha1_out;


                var secWebSocketProtocol = GetHeader(connId, "Sec-WebSocket-Protocol");
                if (!string.IsNullOrEmpty(secWebSocketProtocol))
                {
                    var arr = secWebSocketProtocol.Split(new[] { ',', ' ' });
                    if (arr.Length > 0)
                    {
                        headers[3].Name = "Sec-WebSocket-Protocol";
                        headers[3].Value = arr[0];
                    }

                }

                SendResponse(connId, HttpStatusCode.SwitchingProtocols, null, headers, null, 0);
            }
            return HttpParseResult.Ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connId">客户ID</param>
        /// <param name="state">客户状态</param>
        /// <param name="pData">指针长度</param>
        /// <param name="length">长度</param>
        /// <returns>bool</returns>
        public bool SendMessage(IntPtr connId, WSMessageState state, IntPtr pData, int length)
        {
            return HttpSdk.HP_HttpServer_SendWSMessage(pServer, connId, state.Final, state.Reserved, state.OperationCode, state.Mask, pData, length, 0);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="connId">客户ID</param>
        /// <param name="state">客户状态</param>
        /// <param name="data">数据</param>
        /// <returns>bool</returns>
        public bool SendWSMessage(IntPtr connId, WSMessageState state, byte[] data)
        {
            state.Mask = null;
            int bodyLen = data == null ? 0 : data.Length;
            return HttpSdk.HP_HttpServer_SendWSMessage(pServer, connId, state.Final, state.Reserved, state.OperationCode, state.Mask, data, bodyLen, 0);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="connId">客户ID</param>
        /// <param name="state">客户状态</param>
        /// <param name="data">数据</param>
        /// <returns>bool</returns>
        public bool SendWSMessage(IntPtr connId, WSMessageState state, string txt)
        {
          
            state.Mask = null;
            int bodyLen =string.IsNullOrEmpty(txt) ? 0 : txt.Length;
            byte[] data = Encoding.UTF8.GetBytes(txt);//
            bodyLen = data.Length;
           //bool b= this.Send(connId, data, bodyLen);
            return HttpSdk.HP_HttpServer_SendWSMessage(pServer, connId, state.Final, state.Reserved, state.OperationCode, state.Mask, data, bodyLen, 0);
        }

        /// <summary>
        /// 获取客户端的state
        /// </summary>
        /// <param name="connId">客户ID</param>
        /// <returns>WSMessageState</returns>
        public WSMessageState GetWSMessageState(IntPtr connId)
        {
            bool final = false;
            WSReserved reserved =  WSReserved.Off;
            WSOpcode operationCode = WSOpcode.Cont;
            IntPtr maskPtr = IntPtr.Zero;
            ulong bodyLen = 0;
            ulong bodyRemain = 0;
            bool ret = HttpSdk.HP_HttpServer_GetWSMessageState(pServer, connId, ref final, ref reserved, ref operationCode, ref maskPtr, ref bodyLen, ref bodyRemain);
            if (ret)
            {
                WSMessageState state = new WSMessageState()
                {
                    Final = final,
                    Reserved = reserved,
                    OperationCode = operationCode,

                    BodyLen = bodyLen,
                    BodyRemain = bodyRemain,
                };

                if (maskPtr != IntPtr.Zero)
                {
                    state.Mask = new byte[4];
                    Marshal.Copy(maskPtr, state.Mask, 0, state.Mask.Length);
                }

                return state;
            }

            return null;
        }

        public byte[] PackData(string message)
        {
            byte[] contentBytes = null;
            byte[] temp = Encoding.UTF8.GetBytes(message);
            if (temp.Length < 126)
            {
                contentBytes = new byte[temp.Length + 2];
                contentBytes[0] = 0x81;
                contentBytes[1] = (byte)temp.Length;
                Array.Copy(temp, 0, contentBytes, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)//65535
            {
                contentBytes = new byte[temp.Length + 4];
                contentBytes[0] = 0x81;
                contentBytes[1] = 126;
                contentBytes[2] = (byte)(temp.Length >> 8);
                contentBytes[3] = (byte)(temp.Length & 0xFF);
                Array.Copy(temp, 0, contentBytes, 4, temp.Length);
            }
            else
            {
                contentBytes = new byte[temp.Length + 10];
                contentBytes[0] = 0x81;
                contentBytes[1] = 127;
                contentBytes[2] = 0;
                contentBytes[3] = 0;
                contentBytes[4] = 0;
                contentBytes[5] = 0;
                contentBytes[6] = (byte)(temp.Length >> 24);
                contentBytes[7] = (byte)(temp.Length >> 16);
                contentBytes[8] = (byte)(temp.Length >> 8);
                contentBytes[9] = (byte)(temp.Length & 0xFF);
                Array.Copy(temp, 0, contentBytes, 10, temp.Length);
            }

            return contentBytes;
        }

        #region 发送数据
        /// <summary>
        /// 把发送给客户端消息打包处理
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="message">Message.</param>
        private byte[] PackageServerData(string msg)
        {
            byte[] content = null;
            byte[] temp = Encoding.UTF8.GetBytes(msg);
            if (temp.Length < 126)
            {
                content = new byte[temp.Length + 2];
                content[0] = 0x81;
                content[1] = (byte)temp.Length;
                Buffer.BlockCopy(temp, 0, content, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)
            {
                content = new byte[temp.Length + 4];
                content[0] = 0x81;
                content[1] = 126;
                content[2] = (byte)(temp.Length & 0xFF);
                content[3] = (byte)(temp.Length >> 8 & 0xFF);
                Buffer.BlockCopy(temp, 0, content, 4, temp.Length);
            }
            return content;
        }
        #endregion
    }

}
