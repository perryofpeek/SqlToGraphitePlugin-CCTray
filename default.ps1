properties {
  $Build_Solution = 'SqlToGraphitePlugin-CCTray.sln'
  $Debug = 'Debug'
  $pwd = pwd
  $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
  $nunit =  "$pwd\packages\NUnit.Runners.2.6.2\tools\nunit-console-x86.exe"
  $openCover = "$pwd\packages\OpenCover.4.0.804\OpenCover.Console.exe"
  $reportGenerator = "$pwd\packages\ReportGenerator.1.7.1.0\ReportGenerator.exe"
  $TestOutput = "$pwd\BuildOutput"
  $UnitTestOutputFolder = "$TestOutput\UnitTestOutput";
  $TestReport = "";
  $Build_Artifacts = 'output'
  $Build_Configuration = 'Release'
  $version = '0.1.0.0'
  $nuspecFile = "SqlToGraphitePlugin-CCTray.nuspec"
}

task default -depends Init, Clean, Compile, Test, Package , Report

task Test { 			
	$sinkoutput = mkdir $TestOutput -Verbose:$false;  
    $sinkoutput = mkdir $UnitTestOutputFolder -Verbose:$false;  
	
	$unitTestFolders = Get-ChildItem test\* -recurse | Where-Object {$_.PSIsContainer -eq $True} | where-object {$_.Fullname.Contains("output")} | where-object {$_.Fullname.Contains("output\") -eq $false}| select-object FullName
	foreach($folder in $unitTestFolders)
	{
		$x = [string] $folder.FullName
		copy-item -force -path $x\* -Destination "$UnitTestOutputFolder\" 
	}
	#Copy all the unit test folders into one folder 
	cd $UnitTestOutputFolder
	foreach($file in Get-ChildItem *test*.dll)
	{
		$files = $files + " " + $file.Name
	}
	#write-host $files
	#write-host " $openCover -target:$nunit -filter:+[SqlToGraphite*]* -register:user -mergebyhash -targetargs:$files /err=err.nunit.txt /noshadow /nologo /config=SqlToGraphite.UnitTests.dll.config"
	#write-host  "unit test"
	Exec { & $openCover "-target:$nunit" "-filter:-[.*test*]* +[SqlToGraphite*]* " -register:user -mergebyhash "-targetargs:$files /err=err.nunit.txt /noshadow /nologo /config=SqlToGraphite.UnitTests.dll.config" } 	
	Exec { & $reportGenerator "-reports:results.xml" "-targetdir:..\report" "-verbosity:Error" "-reporttypes:Html;HtmlSummary;XmlSummary"}	
	cd $pwd	
}

task Package {
   if ((Test-path -path $Build_Artifacts -pathtype container) -eq $false)
    {		
		mkdir $Build_Artifacts
	}
	
	Copy-item .\src\SqlToGraphite.Plugin.CCTray\output\SqlToGraphite.Plugin.CCTray.dll $Build_Artifacts\
	Copy-item $nuspecFile $Build_Artifacts\
	Exec { packages\NuGet.CommandLine.1.7.0\tools\NuGet.exe Pack $nuspecFile -BasePath $Build_Artifacts -outputdirectory $Build_Artifacts  -Version  $version }	
}

task Compile {  
   Exec {  & $msbuild /m:4 /verbosity:quiet /nologo /p:OutDir=""$Build_Artifacts\"" /t:Rebuild /p:Configuration=$Build_Configuration $Build_Solution }   	
}

task Clean {
  #write-output "!! $Build_Artifacts"
  if((test-path  $Build_Artifacts -pathtype container))
  {
	rmdir -Force -Recurse $Build_Artifacts;
  }     
  if (Test-Path $TestOutput) 
  {
	Remove-Item -force -recurse $TestOutput
  }  
  
  Exec {  & $msbuild /m:4 /verbosity:quiet /nologo /p:OutDir=""$Build_Artifacts\"" /t:Clean $Build_Solution }  
}

task Init {

	$Company = "peek.org.uk";
	$Description = "SqlToGraphite plugin for collecting metrics from SQL server";
	$Product = "SqlToGraphite CCTray Plugin $version";
	$Title = "SqlToGraphite CCTray Plugin $version";
	$Copyright = "PerryOfPeek 2012";	

	$files = Get-ChildItem src\* -recurse | Where-Object {$_.Fullname.Contains("AssemblyInfo.cs")}
	foreach ($file in $files)
	{
		Generate-Assembly-Info `
        -file $file.Fullname `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version $version `
        -copyright $Copyright
	}
}

task Report  {
	write-host "================================================================="	
	$xmldata = [xml](get-content BuildOutput\UnitTestOutput\testresult.xml)
	
	write-host "Total tests "$xmldata."test-results".GetAttribute("total") " Errors "$xmldata."test-results".GetAttribute("errors") " Failures " $xmldata."test-results".GetAttribute("failures") "Not-run "$xmldata."test-results".GetAttribute("not-run") "Ignored "$xmldata."test-results".GetAttribute("ignored")
	#write-host "Total errors "$xmldata."test-results".GetAttribute("errors")
	#write-host "Total failures "$xmldata."test-results".GetAttribute("failures")
	#write-host "Total not-run "$xmldata."test-results".GetAttribute("not-run")
	#write-host "Total inconclusive "$xmldata."test-results".GetAttribute("inconclusive")
	#write-host "Total ignored "$xmldata."test-results".GetAttribute("ignored")
	#write-host "Total skipped "$xmldata."test-results".GetAttribute("skipped")
	#write-host "Total invalid "$xmldata."test-results".GetAttribute("invalid")

	$xmldata1 = [xml](get-content "$TestOutput\report\summary.xml")
	$xmldata1.SelectNodes("/CoverageReport/Summary")
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}

function Get-Git-Commit
{
    $gitLog = git log --oneline -1
    return $gitLog.Split(' ')[0]
}

function Generate-Assembly-Info
{
param(
    [string]$clsCompliant = "true",
    [string]$title, 
    [string]$description, 
    [string]$company, 
    [string]$product, 
    [string]$copyright, 
    [string]$version,
    [string]$file = $(Throw "file is a required parameter.")
)
  $commit = Get-Git-Commit
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliantAttribute($clsCompliant)]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

    $dir = [System.IO.Path]::GetDirectoryName($file)
    if ([System.IO.Directory]::Exists($dir) -eq $false)
    {
        Write-Host "Creating directory $dir"
        [System.IO.Directory]::CreateDirectory($dir)
    }
   # Write-Host "Generating assembly info file: $file"
    out-file -filePath $file -encoding UTF8 -inputObject $asmInfo
}