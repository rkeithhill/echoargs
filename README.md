# Echoargs Command Line Utility

`echoargs` is an extremely simple command line utillity that displays how the calling program has processed and passed
parameters to it. This is most useful when used from a shell where certain characters are interpreted by the shell and
as a result, do not get passed to the calling program.

The utility was written with PowerShell in mind, since PowerShell has a number of characters that have special meaning
to PowerShell and, unless escaped, would not get passed to the native application as expected. Some of the characters
that PowerShell interprets specially are: `$`, `@`, `|`, `&`, `&&`, `||`, `(`, `)`, `{`, `}`, `,`, `;`.

## Building Echoargs

### Pre-requisites

This utility is based on .NET. You can install the .NET 8.0 SDK by following the install instructions
[here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for your operating system.

Building echoargs is accomplished by running:

```pwsh
cd <repo-root-dir>
dotnet publish -c Release
```

Afterwards, the built `echoargs` binary will be in `<repo-root-dir>/bin/Release/net8.0/<runtime-identifier>/publish`,
where `<runtime-identifier>` will be something like: `win-x64`, `linux-x64` or `osx-arm64` depending on your OS.

## Using Echoargs

Echoargs is most useful when you're attempt to call a native application and you're getting an error you don't
think you should be getting. The error could be a "unexpected token" error, or the native application isn't receiving
the argument you sent it, or PowerShell is attempting to execute an argument value as a command e.g.

```text
PS> ./script.bat -pw:my;Password
Password: The term 'Password' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

Replacing the app or script, `script.bat` in this case, with `echoargs` will show what arguments it is receiving:

```text
PS> .\bin\Release\net8.0\win-x64\publish\echoargs.exe -pw:my;Password
Command line:
C:\Users\Keith\Code\dotnet\echoargs\bin\Release\net8.0\win-x64\publish\echoargs.exe -pw:my

Arg[0] is: -pw:my
Password: The term 'Password' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

From this we can see that `script.bat` is not receiving the `;Password` part of the argument. This is because a
semi-colon is a statement terminator in PowerShell. When PowerShell see `;` it considers that the end of the
`script.bat` command invocation and then tries to invoke the `Password` command. Adding qoutes around the argument
is a common way to fix this issue, but you can also escape the `;`:

```text
PS> .\bin\Release\net8.0\win-x64\publish\echoargs.exe -pw:my;Password
Command line:
C:\Users\Keith\Code\dotnet\echoargs\bin\Release\net8.0\win-x64\publish\echoargs.exe -pw:my

Arg[0] is: -pw:my
Password: The term 'Password' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

### Cowsay Example

```text
PS> wsl cd / && ls . | cowsay
cowsay: The term 'cowsay' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

Using `echoargs` (below) we can see that `wsl` is getting the `cd /`, but when PowerShell sees the `&&` it interprets
that as the start of a new command. PowerShell will try to execute the if the first command is successful.

```text
PS> .\bin\Release\net8.0\win-x64\publish\echoargs.exe cd / && ls . | cowsay
Command line:
C:\Users\Keith\Code\dotnet\echoargs\bin\Release\net8.0\win-x64\publish\echoargs.exe cd /

Arg[0] is: cd
Arg[1] is: /
cowsay: The term 'cowsay' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

To prevent PowerShell from interpreting `&&` as a pipeline chaining operator, let's escape each character as shown
below:

```text
PS> .\bin\Release\net8.0\win-x64\publish\echoargs.exe cd / `&`& ls . | cowsay
cowsay: The term 'cowsay' is not recognized as a name of a cmdlet, function, script file, or executable program.
Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
```

This output may be a bit confusing since we don't see any output from `echoarags`. That's because everything to the
left of the `|` is getting piped into the `cowsay` command, but PowerShell can't find `cowsay` on Windows.  This makes
sense because PowerShell interprets `|` as the way to pipe output of one command into another command.  Let's escape
the `|` symbol and try again:

```text
PS> .\bin\Release\net8.0\win-x64\publish\echoargs.exe cd / `&`& ls . `| cowsay
Command line:
C:\Users\Keith\Code\dotnet\echoargs\bin\Release\net8.0\win-x64\publish\echoargs.exe cd / && ls . | cowsay

Arg[0] is: cd
Arg[1] is: /
Arg[2] is: &&
Arg[3] is: ls
Arg[4] is: .
Arg[5] is: |
Arg[6] is: cowsay
```

Trying again with the original `wsl` command yields the desired results:

```text
 PS> wsl cd / `&`& ls . `| cowsay
 _________________________________________
/ bin boot dev etc home init lib lib32    \
| lib64 libx32 lost+found media mnt opt   |
| proc root run sbin snap srv sys tmp usr |
\ var                                     /
 -----------------------------------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||
```
