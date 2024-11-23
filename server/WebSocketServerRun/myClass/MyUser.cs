using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketServerRun.myClass
{
    public class MyUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public string strTime { get; set; }
    }
}
