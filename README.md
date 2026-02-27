# __MouseClicker__
> ### ___小巧、易用、精准、高效___

> [!WARNING]
> ___程序使用 [`Win32API::SendInput`](https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendinput) 实现模拟鼠标点击___
> - ___需要管理员权限运行___
>    - ___部分安全等级较高的程序，即使拥有管理员权限依旧无法向其发送按键消息，以下说明来自 `MSDN`___
>    - > ___["此函数受 UIPI 约束。 仅允许应用程序将输入注入到完整性级别相等或更低级别的应用程序。"](https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendinput#remarks)___
> - ___软件层模拟，并非发送硬件指令，所有操作可被反作弊系统监视___
>    - ___程序内置各种感知模拟选项 [*]()，可模拟类人（伪人bushi）操作，可一定程度上降低被判定为作弊的可能___
>    - ___由于先前所述，反作弊系统一定以及肯定可以获知按键指令由软件模拟，在敏感场景请谨慎使用___
> - ___程序使用 [`Win32API::timeBeginPeriod`](https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timebeginperiod) 实现高精度计时___
>    - ___计时器精度提升仅在必要时启动，并使用 [`Win32API::timeEndPeriod`](https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timeendperiod) 适时关闭，避免产生额外的性能开销___
>    - ___在特定Windows版本（`Win10 21H2` 版本号2004 内部版本号19041）以前，调用该函数会同步提升所有进程的计时器精度，而非该版本及后续系统中的 '仅提升当前进程计时器精度' 的效果，可能产生额外性能开销，以下说明来自 `MSDN`___
>    - > ___["在 Windows 10 版本 2004 之前，此函数会影响全局 Windows 设置。 对于所有进程，Windows 使用最低值 (即任何进程请求的最高分辨率) 。 从 Windows 10 版本 2004 开始，此函数不再影响全局计时器分辨率。 对于调用此函数的进程，Windows 使用最低值 (即任何进程请求的最高分辨率) 。 对于未调用此函数的进程，Windows 不保证比默认系统分辨率更高的分辨率。"](https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timebeginperiod#remarks)___
>> ___目前程序已在多款游戏中测试可用（附[测试网站](https://www.mousetester.cn)）___

<br/>

![MouseClickerImg](https://raw.githubusercontent.com/FeiLingshu/MouseClicker/refs/heads/main/MouseClicker.png)<sup>___&emsp;此为展示图片，并非可点击的程序界面___</sup>

### __功能__
- [x] __支持常规鼠标按键点击（左键/中键/右键）__
- [x] __支持配置点击间隔__
   - [x] __使用高精度计时器+自动时间补偿，点击时间精确至±1ms__
   - [x] __支持添加额外的随机延迟（可选）__
- [x] __支持自定义点击位置（可选）__
   - [x] __使用鼠标点击定位方式，简单易操作__
   - [x] __支持多显示器定位__
   - [x] __支持添加额外的随机移位（可选）__
- [x] __使用全局低级别鼠标钩子（WH_MOUSE_LL）实现快捷键检测__
   - [x] __避免传统注册方式可能产生的快捷键冲突__
   - [x] __避免其他应用程序注册了相同的快捷键导致快捷键无响应__
- [x] __窗口默认置顶，可最小化至任务栏，确保显示效果的情况下避免遮挡__
> __请注意：在 `Win11` 系统中，若最小化程序窗口，将自动丢失高精度计时器配置，以下说明来自 `MSDN`__
> - __["从Windows 11开始，如果拥有窗口的进程完全被遮挡、最小化或最终用户看不见或听不见，Windows 不保证比默认系统分辨率更高的分辨率。"](https://learn.microsoft.com/zh-cn/windows/win32/api/timeapi/nf-timeapi-timebeginperiod#remarks)__

> [!NOTE]
> __程序需要 [`.Net Framework 4.8`](https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net48) 运行环境__
> - __如需自行编译，请下载「 生成应用 - 开发包 」列下的内容__
> - __如仅使用，请下载「 生成应用 - 运行时 」列下的内容__
> - __下载「 运行时 」时，推荐优先使用「 Web 安装程序 」进行安装__
>
> __⚠️ 使用「 脱机安装程序 」进行安装时，推荐下载所在地区的「 语言包 」，以确保程序内部分提示信息以当地语言呈现__

#
> __作者：FeiLingshu [(GitHub)](https://github.com/FeiLingshu) [(bilibili)](https://space.bilibili.com/483822869)__  
> __声明：本程序代码完全开源，严禁售卖，若使用本程序进行高风险操作，后果将由使用者自行承担__
