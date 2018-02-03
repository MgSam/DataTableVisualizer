param($VsPath)

#'Args: ' + $args > 'c:\test.txt'
#'VsPath: ' + $vspath >> 'c:\test.txt'

$vs = $VsPath#"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\"
$csc = $vs + "\MSBuild\15.0\Bin\Roslyn\csc.exe"
$autoexpDir = $vs + "\Common7\Packages\Debugger\Visualizers\Original\"
$autoexp = $autoexpDir + "autoexp.cs"
$visualizersDir = $vs + "\Common7\Packages\Debugger\Visualizers"

#Make a backup of autoexp.cs
cp $autoexp ($autoexp + ".bak")

#Make sure autoexp.cs does not already have visualizer registered
$autoexpTmp = $autoexp + ".tmp"
cat $autoexp | where {$_ -notmatch 'ShineTools.DataTableVisualizer'} | sc $autoexpTmp
cp $autoexpTmp $autoexp
#rm $autoexpTmp

#Register Visualizer to autoexp.cs
ac $autoexp "`r`n`r`n//ShineTools.DataTableVisualizer`r`n[assembly: DebuggerVisualizer(typeof(ShineTools.DataTableVisualizer), Target = typeof(System.Data.DataTable))]"

#Navigate to autoexp dir before running compiler - compiler just spits out files to current dir
cd $autoexpDir

#Compile autoexp.cs
& $csc /t:library $autoexp /lib:$visualizersDir /r:ShineTools.DataTableVisualizer.dll /r:Microsoft.VisualStudio.DebuggerVisualizers.dll

#Run extension (should invoke VS extension installer)
ii ($visualizersDir + "\DataTableVisualizerExtension.vsix")