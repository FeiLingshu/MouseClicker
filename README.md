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
