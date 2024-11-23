using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketServerRun.myClass
{
    public class baseInfo
    {
        /// <summary>
        /// 用户类
        /// </summary>
        public MyUser myUser { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public emMsgType msgType { get; set; }

        /// <summary>
        /// json数据
        /// </summary>
        public object data { get; set; }
    }
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum emMsgType
    {
        /// <summary>
        /// 客户端上线
        /// </summary>
        Type1 =1,
        /// <summary>
        /// 客户端下线
        /// </summary>
        Type2 = 2,
        /// <summary>
        /// 客户端发送普通文本给服务器
        /// </summary>
        Type3 = 3,
        /// <summary>
        /// 客户端发送普通文本给其他的客户端
        /// </summary>
        Type4 = 4,
        /// <summary>
        /// 服务器关闭，通知客户端
        /// </summary>
        Type5 = 5,
        /// <summary>
        /// 服务器推送普通文本消息给客户端
        /// </summary>
        Type6 = 6,
        /// <summary>
        /// 客户端告诉服务器，发布的商品。可以对该商品进行处理
        /// </summary>
        Type7 = 7,
        /// <summary>
        /// 服务器发送普通文本给客户端
        /// </summary>
        Type8 = 8,
        /// <summary>
        /// 服务器推送在线的用户列表给客户端
        /// </summary>
        Type9 = 9,
        /// <summary>
        /// 强制客户端下线
        /// </summary>
        Type10 = 10,
        /// <summary>
        /// 服务器推送商品消息给客户端
        /// </summary>
        Type11 = 11,
    }

    /// <summary>
    /// 普通文本消息
    /// </summary>
    public class MsgTxt
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        public string txt { get; set; }
    }

    /// <summary>
    /// 客户指定发送给其他客户的文本消息
    /// </summary>
    public class MsgAppointSendUserText
    {
        /// <summary>
        /// 来自的用户类
        /// </summary>
        public MyUser fromUser { get; set; }

        /// <summary>
        /// 送到的用户类
        /// </summary>
        public MyUser toUser { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string strTime { get; set; }

        /// <summary>
        /// 普通文本
        /// </summary>
        public string txt { get; set; }
    }

    /// <summary>
    /// 用户告诉服务器发布的商品
    /// </summary>
    public class MsgAppointSendUserCommodity
    {
        /// <summary>
        /// 来自的用户类
        /// </summary>
        public MyUser fromUser { get; set; }

        /// <summary>
        /// 推送类型，1=客户端发起的推送，2=服务器主动的推送
        /// </summary>
        public string AppointType { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string commodityID { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public double many { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double longitude { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
    }

    /// <summary>
    /// 强制下线类
    /// </summary>
    public class MsgCompulsoryDownline
    {
        /// <summary>
        /// 提示内容
        /// </summary>
        public string txt { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public emCompulsoryDownline type { get; set; }
    }
    public enum emCompulsoryDownline
    {
        /// <summary>
        /// 重复登录
        /// </summary>
        Type1 = 1,
        /// <summary>
        /// 服务器关闭
        /// </summary>
        Type2 = 2,
        /// <summary>
        /// 手动强制下线
        /// </summary>
        Type3 = 3,
    }
}
