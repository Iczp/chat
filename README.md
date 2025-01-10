# IM.Chat

abpvnext  chat module.


## Docker build
```bash
docker build -t iczpnet/chat-auth-server:v0 -f ./host/IczpNet.Chat.AuthServer/Dockerfile .
docker build -t iczpnet/chat-api-host:v0 -f ./host/IczpNet.Chat.HttpApi.Host/Dockerfile .
```


## Startup

```
abp new IczpNet.Chat -t module --no-ui 
```

### Set Startup object: IczpNet.Chat.AuthServer

```
Update-Database
```





### Message

| SenderId | ReceiverId | SessionId |
| -------- | ---------- | --------- |
| C1       | S1.1       | S1-C1     |
| s2       | C2         | S-C2      |
|          |            |           |

## 项目的需求

### 需求

### 

## 在线状态

### 在线状态管理 `IServiceStateManager`

1. 群/公众号 解析为 null
2. 人/Anonymous/ShopWaiter/Customer 解析为 [Online | Offline]
3. Shopper（自己或是子账号任何一下在线就解析为 [Online ]），都不在线都解析为【Offline】

## Internal structure 数据结构

### Message 消息表

### ChatObject 聊天对象(参与会话)

#### Persion 人(用户、员工)

#### Customer 人（客户）

#### Room 群

#### Official 公众号

#### ----OfficialGroup 公众号分组（人群定义、临时分组等）

#### Robot 机器人（智能对话）

#### Shopkeeper 电商掌柜(主账号)

#### ShopWaiter 店小二(子账号)

#### Square 广场



## Session



## Room

### CreateRoom

```json
{
  "name": "9999999999999999999999999",
  "code": "string",
  "ownerId": null,
  "type": 0,
  "description": "string",
  "chatObjectIdList": [
    "31F8B124-BEA6-85C4-B4E3-3A07BB313745","329A7C43-9B01-5966-322A-3A07D4A342EA","5B6A6100-CA52-8040-09F2-3A07D4A367ED","972157FA-2449-FEC6-49A1-3A07DE609F58",
  ]
}
```



## 待实现功能

### 以前要求未实现的功能



## Roadmap



### 聊天对象(ChatObject)

- [x] **单用户绑定多个聊天对象**
- [x] 机器人
- [x] 官方号(Official)
- [x] 订阅号(Subscription)
- [x] 掌柜(ShopKeeper)
- [x] 店小二(ShopWaiter)
- [x] 聊天广场(Square)
- [x] 匿名(Anonymous)
- [x] 动态信息添加（添加手机，QQ，职位等）

### 聊天对象扩展(ChatObject)

- [x] 个性签名(Motto)
- [ ] 朋友圈



### 会话功能(Session)

- [x] 单聊
- [x] 群聊
- [x] 官方通知/功能号(Official)
- [x] 聊天广场
- [x] 订阅号/服务号
- [ ] 客服系统（CallCenter）



### 会话验证系统（好友、加群、加聊天广场）

- [x] 好友验证、处理好友验证消息
- [x] 加群验证，群主/管理员处理验证
- [x] 设置验证方式(不需要验证、验证、自动拒绝验证)
- [x] 自动验证



### 会话单元(SessionUnit)

- [x] 我的会话消息
- [x] 会话成员(群内成员，广场成员)
- [x] 共同的好友、共同所在的群/广场
- [x] 群/广场内名称
- [x] 备注好友/群
- [x] 会话置顶
- [x] 会话免打扰功能
- [x] 标记为已读
- [x] 删除会话消息
- [x] 清除会话消息
- [x] 会话角标
- [x] 删除会话（不退群）
- [x] 退出会话（退群）
- [x] 会话开启与停用(官方号、功能号)
- [x] 订阅与取消订阅（订阅号、服务号）
- [x] 设置聊天背景
- [x] 只读会话（通知群、官方功能、公告等）
- [x] 【新消息】角标统计
- [x] 【私有消息】角标统计
- [x] 【特别关注】角标统计
- [x] 【@我】角标统计
- [x] 动态属性备注（添加手机，QQ，职位等）

### 会话管理功能

  - [x] 会话生成器(SessionGenerator)
  - [ ] 会话盒子(SessionBox)
  - [x] 创建聊天广场
  - [ ] 二维码扫码加入群聊
  - [ ] 二维码扫码加入聊天广场
  - [x] 角色管理、角色权限分配
  - [x] 组织架构
  - [x] 角色管理、角色权限分配
  - [x] 会话内权限（组织、角色、加人，踢人等）
  - [x] 权限分组
  - [ ] 权限启用与禁用
  - [ ] 会话标签（SessionTag）-- 
  - [x] 会话菜单功能
  - [ ] “拍一拍”
  - [x] @所有人、@XXX
  - [ ] 禁言（管理员、群主）
  - [x] 是否允许设置“免打扰”



### 群聊功能

  - [x] 创建群聊
  - [x] 邀请成员入群
  - [x] 邀请成员加群
  - [x] 群主权限（拥有所有权限）
  - [x] 群角色功能（可配置多角色）
  - [x] 群成员权限（可单一配置成员权限）
  - [x] [权限]更新群名称并通知群成员
  - [x] [权限]更新头头像并通知群成员
  - [x] 转让群主
  - [x] 共同所在的群
  - [x] 加群验证（验证消息由【机器人/群助手】发送到【群主或管理员】）
  - [x] 管理员/群主处理验证
  - [x] 添加/取消【特别关注】（关注群成员，有新消息时标注为【特别关注】）
  - [x] 入群方式设置
  - [ ] 二维码扫码加入群聊
  - [ ] 面对面加群
  - [ ] 邀请码加群
  - [x] 群内组织架构
  - [ ] 群内公告
  - [ ] 群机器人提醒功能
  - [ ] 设置是否可以转发群里消息
  - [ ] 设置新成员“默认角色”
  - [x] 设置新成员“历史消息查看范围”



### 聊天广场

  - [x] 创建聊天广场
  - [x] 广场成员（分组）
  - [ ] 特别关注（关注群成员，有新消息时标注为【特别关注】）
  - [ ] 设置广场进入方式（公有、私有）
  - [ ] 广场公告
  - [ ] 机器人提醒功能



### 单聊天功能

  - [ ] 设置“能过账号找到我”
  - [ ] 设置“通过手机号找到我”
  - [x] 设置“好友验证方式”
  - [ ] 设置是否群内加好友



### 官方号功能号(Official)

  - [x] 启用与停用功能
  - [x] 设置是否可以停用
  - [x] 设置是否为只读（通知功能 ，成员不能发消息，只能接收消息）



### 订阅号功能

  - [x] 订阅与取消订阅（同时发会话内私有通知）
  - [ ] 设置是否可以停用
  - [x] 设置是否为只读（通知功能 ，成员不能发消息，只能接收消息）



#### 掌柜与店小二

  - [x] 创建子账号
  - [ ] 子账号管理（开启、禁用、删除）
  - [x] 消息同步
  - [ ] 消息转接
  - [ ] 账号状态设置（挂起、忙录）



### 消息模板功能

  - [x] 系统消息(Command)
  - [x] 文本消息
  - [x] 图片消息
  - [x] 语音消息
  - [x] 视频消息
  - [x] 文件消息
  - [ ] 文章消息
  - [x] HTML消息
  - [x] 链接消息
  - [x] 名片消息
  - [x] HTML消息[H5、markdown]
  - [x] 位置消息（Location）
  - [x] 历史聊天记录
  - [ ] 公告消息（群公告、广场公告）
  - [x] 红包消息（未实现支付功能，只能积分形式收发红包）



### 消息扩展功能

  - [x] 转发消息
  - [x] 群发消息
  - [x] 引用消息
  - [x] 消息提醒器
  - [x] 消息收藏夹
  - [x] 消息已读记录器(ReadedRecorder)
  - [x] 消息打开记录器(OpenedRecorder)
  - [ ] **敏感词过滤\审核**
  - [ ] Elasticsearch（elastic.co）



### 客服系统（CallCenter）
  - [x] 子账号管理
  - [ ] 设置服务状态(挂起、接收、忙录等)
  - [ ] 会话转接功能
  - [ ] 消息分流功能
  - [ ] 客户发起会话(自动加入会话)



### 智能对话功能(机器人)

- [x] 机器人账号
- [x] 机器人主动发通知
- [ ] ChatGPT



### 开发者功能

- [x] 开发者设置（Token、EncodingAesKey、PostUrl）
- [x] 开启与关闭功能
- [x] 验证及验签（signature）
- [x] Http请求开发者服务日志
- [x] 后台作业调用开发者及重试功能
- [ ] 验证开发者主机（HOST）
- [ ] 开发者Demo
- [ ] 开发者SDK



### WebHook

- [ ] 权限验证（APIKey）

- [ ] ApiKey管理功能

- [ ] Api日志

  

### 扩展功能

  - [ ] 实现共享位置

  - [ ] 扫码登录

  - [ ] 文件服务器（文件预览）

    

