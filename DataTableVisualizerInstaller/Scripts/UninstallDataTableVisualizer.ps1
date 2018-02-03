$csc = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\Roslyn\csc.exe"
$autoexp = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Packages\Debugger\Visualizers\Original\autoexp.cs"
$autoexpDll = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Packages\Debugger\Visualizers\Original\autoexp.dll"
$visualizers = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Packages\Debugger\Visualizers"

cat $autoexp | where {$_ -notmatch 'ShineTools.DataTableVisualizer'} | sc $autoexp"tmp"
cp $autoexp"tmp" $autoexp
rm $autoexp"tmp"