using HPSocketCS;
using Newtonsoft.Json;
using System;
using System.Threading;
using WebSocketDemo;

namespace WebSocketServerRun.myClass
{
    class TimerSendUtil
    {
        private static Timer _timer;
        private static int flag;
        

        public static void send(MyWebSocketServer myWebSocketServer,string myMsg)
        {
            flag = 0;

            Console.WriteLine("定时任务已启动");
            while (true) {
                _timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                Thread.Sleep(5000);
                IntPtr[] arr = myWebSocketServer.wsServer.GetAllConnectionIDs();
                if (arr != null && arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        IntPtr connId = arr[i];
                        string kson = JsonConvert.SerializeObject(myMsg);
                        var state = myWebSocketServer.wsServer.GetWSMessageState(connId);
                        if (state != null && state.OperationCode != WSOpcode.Close)
                        {
                            myWebSocketServer.wsServer.SendWSMessage(connId, state, kson);
                        }
                    }
                }
            }
            /* 
            try
            { 
                
             Console.ReadKey();
            } catch (InvalidOperationException e) {
                Console.WriteLine(""+ e.Message); ;
            }
            */
        }
        private static void ExecuteTask(Object o)
        {
            flag++;
            Console.WriteLine($"执行任务:{DateTime.Now}-{flag}");
        }
    }
}
