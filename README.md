# SharpMediaFoundation

## Overview

This is a limited port of the original [SharpDX] interop assemblies for working with the [Microsoft Media
Foundation](https://docs.microsoft.com/en-us/windows/win32/api/_mf/) library in C#.

[SharpDX] was abandoned in March 2019. Since then, the [Vortice] project took over and updated the interop assemblies to
work with the .NET Standard 2.0 API. However, the [Vortice] project did not port over the [SharpDX.MediaFoundation]
assembly. This project fills that gap. It uses and works in partnership with the [Vortice] assemblies.

## Development

The [SharpGen] tools are used to generate accurate and high performance C++ interop code for C#. As part of the build,
the [Mapping.xml](Source\SharpMediaFoundation) is used to read in the native Windows header files for the Media
Foundation and extract constants, enums, functions, and interfaces. The generated code gets put into the `obj`
directory. The actual classes in the project are usually partial classes to add functionality to the generated code.

## Documentation

The documentation for [SharpDX.MediaFoundation] applies to this library as well.

[sharpdx]: https://github.com/sharpdx/SharpDX
[sharpdx.mediafoundation]: http://sharpdx.org/wiki/class-library-api/mediafoundation/
[sharpgen]: https://github.com/SharpGenTools/SharpGenTools
[vortice]: https://github.com/amerkoleci/Vortice.Windows
