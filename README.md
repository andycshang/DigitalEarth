# DigitalEarth
# 数字地球卫星可视化平台
本项目旨在开发一个实时卫星轨道可视化系统，解决研究中缺乏便捷可视化工具的问题，用户可以通过网页端输入卫星轨道数据，由后端处理后实时绘制轨道效果。后端使用C#开发，基于WebSocket协议实现了客户端与服务器之间的低延迟双向通信，并通过引入HPSocket库整合了SSL、HTTP和TCP等协议，提供高效、安全的数据传输能力。
开发过程中，我学习了C#语言并编写了WebSocketServer的底层逻辑，解决了客户端连接管理、并发处理和数据解析等实际问题；并学习、参考了orekit等市面现有的可视化插件的原理，给我很大启发。

This project aims to develop a real-time satellite orbit visualization system to address the lack of convenient visualization tools in research. Users can input satellite orbit data through a web interface, which is processed by the back end to render orbital trajectories in real time. The back end is developed in C#, utilizing the WebSocket protocol to achieve low-latency, bidirectional communication between the client and server. By integrating the HPSocket library, which supports SSL, HTTP, and TCP protocols, the system ensures efficient and secure data transmission. 
During the development process, I learned C# from scratch and implemented the underlying logic of the WebSocketServer, solving practical challenges such as client connection management, concurrency handling, and data parsing. Additionally, I studied and drew inspiration from existing visualization tools like Orekit, which provided valuable insights into the project's implementation.
