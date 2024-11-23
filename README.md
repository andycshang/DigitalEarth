# DigitalEarth
数字地球

# 数字地球卫星可视化平台
本项目旨在开发一个实时卫星轨道可视化系统，解决研究中缺乏便捷可视化工具的问题，用户可以通过网页端输入卫星轨道数据，由后端处理后实时绘制轨道效果。后端使用C#开发，基于WebSocket协议实现了客户端与服务器之间的低延迟双向通信，并通过引入HPSocket库整合了SSL、HTTP和TCP等协议，提供高效、安全的数据传输能力。项目在前端与后端之间建立了实时数据更新机制，用户操作可快速反馈至可视化结果，同时通过深入研究Orekit等现有工具，为本系统的架构设计提供了参考。开发过程中，我从零学习了C#语言并编写了WebSocketServer的底层逻辑，解决了客户端连接管理、并发处理和数据解析等实际问题。整个开发流程严格遵循需求分析、数据定义、接口设计和功能封装的顺序，保证了系统的可复用性和扩展性，最终实现了一个高效、稳定的卫星轨道仿真和可视化平台。
