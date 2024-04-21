# dotnet make

A build tool for your build tools.

## Installing

Install the tool by running `dotnet tool -g install make` in your terminal.
If you want to install the tool on a repository basis, omit the `-g` option.


You can then invoke builds by running `dotnet make` anywhere in your repository.  

```console
$ dotnet make --help

USAGE:
    dotnet make [TARGET] [OPTIONS]

ARGUMENTS:
    [TARGET]    The target to run

OPTIONS:
    -h, --help       Prints help information                                    
    -v, --version    Prints version information
```

The tool uses conventions to determine how to invoke your projects' builds. Currently, only [Cake][1], [Frosting][2], and [Make][3] are supported. Support for solution files and [MSBuild traversal projects][4] is planned.

## Supported build providers

* [Cake][1]
* [Cake Frosting][2]
* [Make][3]

## Building

We're using [Cake][1] as a [dotnet tool][5]
for building. So make sure that you've restored Cake by running 
the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
```

[1]: https://github.com/cake-build/cake
[2]: https://cakebuild.net/docs/running-builds/runners/cake-frosting
[3]: https://en.wikipedia.org/wiki/Make_(software)
[4]: https://github.com/microsoft/MSBuildSdks/blob/main/src/Traversal/README.md
[5]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools