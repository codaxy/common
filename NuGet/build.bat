mkdir Codaxy.Common\lib\
mkdir Codaxy.Common.Localization\lib\

copy ..\Libraries\Codaxy.Common\bin\Release\*.* Codaxy.Common\lib\
copy ..\Libraries\Codaxy.Common.Localization\bin\Release\Codaxy.Common.Localization.* Codaxy.Common.Localization\lib\

tools\nuget pack Codaxy.Common\Codaxy.Common.nuspec
tools\nuget pack Codaxy.Common.Localization\Codaxy.Common.Localization.nuspec
