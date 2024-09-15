# Unity Hidden Object Finder

[中文](./README_CN.md)

Find hidden Obejcts in the Unity Editor.

![Hidden Object Finder Window](./Documents~/imgs/img_sample_hidden_object_finder_window.png)

## Supported Unity Version

Unity 2019.4 and later.

## Installation

[![openupm](https://img.shields.io/npm/v/com.greenbamboogames.hiddenobjectfinder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.greenbamboogames.hiddenobjectfinder/)

Install this package via [OpenUPM](https://openupm.com/packages/com.greenbamboogames.hiddenobjectfinder), or clone this repository directly into the Packages folder of your project.

## How to use

Open the Hidden Object Finder tool window from the menu **Tools/Bamboo/Hidden Object Finder**.

Use the **Object Types** option to set the types of objects you want to find, and use the **Hide Flags Filter** option to set which types of hidden flags to search for. Then click the **Find** button to display the search results on the left side of the window. After selecting a search result, you can preview and edit the object in the right panel of the window.

**Note**: The window does not automatically refresh the list of search results. If you modify the object's hidden flags or delete an object, you need to perform the search again to refresh the list.

### API

You can call the static methods of the `GBG.HiddenObjectFinder.Editor.HiddenObjectFinder` class to find hidden objects through a script.
