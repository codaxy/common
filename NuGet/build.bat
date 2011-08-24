del *.nupkg

mkdir Codaxy.Common\lib\
copy ..\Libraries\Codaxy.Common\bin\Release\*.* Codaxy.Common\lib\
nuget pack Codaxy.Common\Codaxy.Common.nuspec

mkdir Codaxy.Common.Localization\lib\
copy ..\Libraries\Codaxy.Common.Localization\bin\Release\Codaxy.Common.Localization.* Codaxy.Common.Localization\lib\
nuget pack Codaxy.Common.Localization\Codaxy.Common.Localization.nuspec

mkdir Codaxy.Common.SqlServer\lib\
copy ..\Libraries\Codaxy.Common.SqlServer\bin\Release\Codaxy.Common.SqlServer.* Codaxy.Common.SqlServer\lib\
nuget pack Codaxy.Common.SqlServer\Codaxy.Common.SqlServer.nuspec



