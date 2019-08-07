# Draw sketches from SVG, using Line-us robot and .NET Core Console application

Welcome to this project, you can read all about it in the blog article that accompanies the code:

https://blogs.siliconorchid.com/post/coding-inspiration/sketch-with-line-us/


## Prerequisites

* A *Line-us* robot

* There will be an assumption that you have familiarity working with:

    * [C#](https://docs.microsoft.com/en-us/dotnet/csharp/getting-started/introduction-to-the-csharp-language-and-the-net-framework)
    * [.NET Core Console projects](https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio)
    * [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection).
     

* This project uses .NET Core 2.x, so you'll need to have installed the [.NET Core 2.x  SDK](https://dotnet.microsoft.com/download), as appropriate to your platform.   .NET Core is cross-platform, which means this project will work on Windows, Mac and Linux.

* You'll need something to edit and build your project.  This demo was primarily created using .NET Core 2.2 on a Windows 10 system using [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) Community edition. There is no reason why you cannot use other platforms, code editors and [CLI](https://docs.microsoft.com/en-us/dotnet/core/tutorials/using-with-xplat-cli) tools - my recommendation would be to use [Visual Studio Code](https://code.visualstudio.com/download).



## Quick start to using the code

Aside from a small configuration edit, you should be able to clone the project from GitHub, add the  Line-us to your network and just run the code.   

Before running the project,  open the file `....\DrawWithLineUs\Config\ProgramConfig.cs` and update the configuration values.

* You will need to change the path to a sample SVG image.

* You will need to change the IP address, to whatever your router has assigned to your own  Line-us.
