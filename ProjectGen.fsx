#!/usr/bin/env -S dotnet fsi

open System
open System.IO

//#region Collecting INFO
printfn "Project name:"

let name =
    match Console.ReadLine().Trim() with
    | "" -> failwith "Project name cannot be empty."
    | n -> n

printfn "Project AOT setting(Default NO)?"
printfn "Type y/n"

let isProjectAOT =
    match Console.ReadLine().Trim().ToLower() with
    | "y"
    | "yes"
    | "true" -> true
    | _ -> false

printfn "Project Type: Exe(Default)/WinExe/Library"
printfn "Type e/w/l"

let projectType =
    match Console.ReadLine().Trim().ToLower() with
    | "e" -> "Exe"
    | "w" -> "WinExe"
    | "l" -> "Library"
    | _ -> "Exe"

printfn $"Project name:{name}, Type:{projectType}, AOT:{isProjectAOT}"

//#region Prepare Contents

let gitattributes =
    """ 
# Auto detect text files and perform LF normalization 
* text=auto 

# Sources 
*.cshtml text diff=html 
*.csx text diff=csharp 
*.fsx text diff=fsharp 

# Graphics 
*.png binary 
*.jpg binary 
*.jpeg binary 
*.gif binary 
*.tif binary 
*.tiff binary 
*.ico binary 
*.eps binary 
*.svg text 

# Scripts 
*.bash text eol=lf 
*.fish text eol=lf 
*.ksh text eol=lf 
*.sh text eol=lf 
*.zsh text eol=lf 

# These are explicitly windows files and should use crlf 
*.bat text eol=crlf 
*.cmd text eol=crlf 
*.ps1 text eol=crlf 

# Serialization 
*.json text 
*.toml text 
*.xml text 
*.xaml text 
*.axaml text 
*.yaml text 
*.yml text 

# Text files where line endings should be preserved 
*.patch -text 

# Exclude files from exporting 
.gitattributes export-ignore 
.gitignore export-ignore 
.gitkeep export-ignore 
"""

let gitignore =
    """ 
# .NET 
obj 
bin 
.fake 

# IDE 
/.vs 
/.vscode 
/.idea 

# MACOS 
.DS_Store 
"""

let solution =
    $""" 
<Solution> 
  <Project Path="src/{name}/{name}.fsproj" /> 
</Solution> 
"""

let fsproject =
    $""" 
<Project Sdk="Microsoft.NET.Sdk"> 
  <PropertyGroup> 
    <OutputType>{projectType}</OutputType> 
    <TargetFramework>net10.0</TargetFramework> 
    <!-- !AOT SETTING! --> 
    <PublishAot>{isProjectAOT.ToString().ToLower()}</PublishAot> 
    <EnableAotAnalyzer>{isProjectAOT.ToString().ToLower()}</EnableAotAnalyzer> 
    <OtherFlags>--reflectionfree</OtherFlags> 
    <PublishSingleFile>true</PublishSingleFile> 
    <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile> 
    <StripSymbols>true</StripSymbols> 
  </PropertyGroup> 
  <!-- !RELEASE SETTING! --> 
  <PropertyGroup Condition="'$(Configuration)'=='Release'"> 
    <DebugSymbols>False</DebugSymbols> 
    <DebugType>None</DebugType> 
  </PropertyGroup> 
  <ItemGroup> 
    <Compile Include="Program.fs" /> 
  </ItemGroup> 
</Project> 
"""

let readme =
    """ 
INSERT SOCIALIFY HEADER HERE 

## Overview 

## Dev 

## License 

## Credits 
"""

let program =
    $""" 
module {name}

[<EntryPoint>] 
let main _ = 
    0 
"""

//#region CreatingProject

// Define dirs
let solutionDir = Path.Combine(".", name)
let srcDir = Path.Combine(solutionDir, "src")
let projectDir = Path.Combine(srcDir, name)

Directory.CreateDirectory projectDir

File.WriteAllText(Path.Combine(solutionDir, ".gitignore"), gitignore)
File.WriteAllText(Path.Combine(solutionDir, ".gitattributes"), gitattributes)
File.WriteAllText(Path.Combine(solutionDir, "README.md"), readme)
File.WriteAllText(Path.Combine(solutionDir, $"{name}.slnx"), solution)
File.WriteAllText(Path.Combine(projectDir, $"{name}.fsproj"), fsproject)
File.WriteAllText(Path.Combine(projectDir, "Program.fs"), program)

printfn $"Finished! Project created at: {Path.GetFullPath solutionDir}"
