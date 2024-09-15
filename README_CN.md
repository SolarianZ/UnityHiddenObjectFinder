# Unity隐藏Object查找工具

[English](./README.md)

查找Unity Editor中隐藏的Object。

![Hidden Object Finder Window](./Documents~/imgs/img_sample_hidden_object_finder_window.png)

## 支持的Unity版本

Unity 2019.4 或更新版本。

## 安装

[![openupm](https://img.shields.io/npm/v/com.greenbamboogames.hiddenobjectfinder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.greenbamboogames.hiddenobjectfinder/)

从 [OpenUPM](https://openupm.com/packages/com.greenbamboogames.hiddenobjectfinder) 安装，或者直接将此仓库克隆到项目的Packages文件夹下。

## 如何使用

从菜单 **Tools/Bamboo/Hidden Object Finder** 打开隐藏对象查找工具窗口。

使用 **Object Types** 选项设置要查找的对象类型，使用 **Hide Flags Filter** 选项设置要查找哪些种隐藏标记，然后点击 **Find** 按钮，即可在窗口左侧显示查找结果。选中查找结果后，可以在窗口右侧面板预览和编辑对象。

**注意**：该窗口不会自动刷新查找结果列表，如果修改了对象的隐藏标记，或者删除了对象，需要重新进行查找才能刷新列表。

### API

可以调用 `GBG.HiddenObjectFinder.Editor.HiddenObjectFinder` 类的静态方法，通过脚本来查找隐藏对象。