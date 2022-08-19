# NET_AlbertzhaozToolHelper
此项目用于收集常用的.NET套件，包含高效工具类(ToolHelper)、常用类库(CommonNuget)、项目实战(Project)、高效工具(Tools)等，以做到对.NET资源的整合。
- 高效工具类：高效的第三方业务库。根据版本可与现有项目无缝集成。其中一部分收集整理的实用类库，一部分自己写的类库，分文件夹分版本，对于每个文件夹增加相关说明，并验证每一个类的有效性和正确性。
- 常用类库：在工作中常用的Nuget包、第三方类库的介绍和使用文档等。
- 项目实战：收集主流的实战项目。
     
# 高效工具类(ToolHelper)
## [01_RsCode](https://gitee.com/kuiyu/RsCode)
### 介绍
**一款开箱即用的.net工具库，助力.net开发。** 基于 .NET Standard 2.1/.NET 5，可直接引用丰富的 .NET 类库。可与已有的 ASP.NET Core MVC、Razor Pages 项目无缝集成。
### 版本支持
- >=.NET Core 3.1
```shell
PM> Install-Package RsCode.AspNetCore -Version 1.5.3
```
### 官方文档
https://rscode.cn

## [02_Masuit.Tools](https://github.com/ldqk/Masuit.Tools)
### 介绍
包含一些常用的操作类，大都是静态类，加密解密，反射操作，权重随机筛选算法，分布式短id，表达式树，linq扩展，文件压缩，多线程下载和FTP客户端，硬件信息，字符串扩展方法，日期时间扩展操作，中国农历，大文件拷贝，图像裁剪，验证码，断点续传，集合扩展、Excel导出等常用封装。**诸多功能集一身，代码量不到2MB！**  
### 版本支持
- ≥.NET Core 2.1
```shell
PM> Install-Package Masuit.Tools.Core
```
- ≥.NET Standard 2.1
```shell
PM> Install-Package Masuit.Tools.Abstraction
```
- ≥.NET Framework 4.6.1
```shell
PM> Install-Package Masuit.Tools.Net
```
- .NET Framework 4.5特供版   
请注意：`这是.NET Framework 4.5的专用版本，相比4.6.1及.NET Core的版本，阉割了Redis、HTML、文件压缩、ASP.NET扩展、硬件监测、Session扩展等一些功能。`**如果你的项目版本高于4.5，请务必使用上述版本的包，以享受完整的功能体验！**
```shell
PM> Install-Package Masuit.Tools.Net45
```
- Masuit.Tools.AspNetCore  
ASP.NET Core Web专用包，包含Masuit.Tools.Core的全部功能，并且增加了一些对ASP.NET Core Web功能的额外支持。
- Masuit.Tools.Excel    
Excel导入导出的专用独立包
- Masuit.Tools.NoSQL.MongoDBClient     
mongodb的封装操作类独立包
### 官方文档
https://masuit.com/55

## [03_dotnetcodes](https://gitee.com/kuiyu/dotnetcodes)
### 介绍
该项目基于MIT协议，它是一个类库，里面包含大量可直接使用的功能代码，可以帮你减少开发与调试时间,而且类与类之间没有什么依赖,每个类都可以单独拿出来使用。**此项目的动机来源于让更多的.net开发者更轻松高效的完成业务，很多代码都是原创，并且应用在了多个真实的项目中。**
### 版本支持
- >=.NET Framework 4.5
### 官方文档
https://gitee.com/kuiyu/dotnetcodes

## [99_Albertzhaoz.Tools](https://github.com/AlbertZhaoz/NET_AlbertzhaozToolHelper)


# 常用类库(CommonNuget)

# 项目实战(Project)

# 高效工具(Tools)

# 如何贡献
如果你希望参与贡献，欢迎 [Pull Request](https://github.com/AlbertZhaoz/NET_AlbertzhaozToolHelper/pulls)，或给我们 [报告 Bug](https://github.com/AlbertZhaoz/NET_AlbertzhaozToolHelper/issues) 。
