# 任务切换

## 目标
1. 保证尽可能高实时性前提下，可以随时暂停和恢复任务进度
2. 提高可扩展能力，尽量方便维护

## 背景
1. 任务指令的下发时立即生效并无需维持的，中断需要通过及时发送停止来实现
2. 

## 临时记录
- 也许应该主要考虑：模板方法模式（Template Method Pattern），主要优先扩展性，在父类中定义算法框架，具体实现由子类提供，避免代码重复，方便增添任务
- 借鉴协程 多线程 线程复用相关的任务流程
- 使用充足测试保证实时性和回退/继续任务能力