using System.Diagnostics;
using System.Linq;
using System.Threading;

var target = Argument("target", "Run");
// Whether to run with the default executable location (Native), steam rungameid (Steam), or a custom location (other)
var execute = Argument("execute", "Native"); 
// The location of the Steam executable if running with steam. Not necessary if steam is on the PATH
var steam = Argument("steam", "steam");
var config = Argument("configuration", "Release");

var tinyLifeDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Tiny Life";

Task("Build").DoesForEach(GetFiles("**/*.csproj"), p => {
    DeleteFiles($"bin/{config}/**/*");
    DotNetBuild(p.FullPath, new DotNetBuildSettings { Configuration = config });
});

Task("CopyToMods").IsDependentOn("Build").Does(() => {
    var dir = $"{tinyLifeDir}/Mods";
    var dotNetRoot = GetDirectories($"bin/{config}/net*").Last();
    
    CreateDirectory(dir);
    var files = GetFiles($"{dotNetRoot}/**/*");
    
    // CopyFiles(files, dir, true);
    Zip($"{dotNetRoot}", $"{dir}/IttyMod.zip", files);
});

Task("Run").IsDependentOn("CopyToMods").Does(() => {
    // start the tiny life process
    var exeDir = $"{tinyLifeDir}/GameDir";
    if (!FileExists(exeDir))
        throw new Exception("Didn't find game directory information. Run the game manually at least once to allow the Run task to be executed.");
    
    var exe = execute;
    var args = "-v --skip-splash --skip-preloads";
    if(exe == "Steam") {
        exe = "steam";
        args = "-applaunch 1651490 " + args;
    } else if(exe == "Native") {
        exe = $"{System.IO.File.ReadAllText(exeDir)}TinyLife";
    }

    var process = Process.Start(new ProcessStartInfo(exe) {
        Arguments = args,
        CreateNoWindow = true
    });

    // we wait a bit to make sure the process has generated a new log file, bleh
    Thread.Sleep(3000);

    // attach to the newest log file
    var logsDir = $"{tinyLifeDir}/Logs";
    var log = System.IO.Directory.EnumerateFiles(logsDir).OrderByDescending(System.IO.File.GetCreationTime).FirstOrDefault();
    if (log != null) {
        using (var stream = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            using (var reader = new StreamReader(stream)) {
                var lastPos = 0L;
                while (!process.HasExited) {
                    if (reader.BaseStream.Length > lastPos) {
                        reader.BaseStream.Seek(lastPos, SeekOrigin.Begin);
                        string line;
                        while ((line = reader.ReadLine()) != null)
                            Information(line);
                        lastPos = reader.BaseStream.Position;
                    }
                    Thread.Sleep(10);
                }
            }
        }
    }

    Information($"Tiny Life exited with exit code {process.ExitCode}");
});

Task("Publish").IsDependentOn("Build").DoesForEach(() => GetDirectories($"bin/{config}/net*"), d => {
    var dllFile = GetFiles($"{d}/**/*.dll").FirstOrDefault();
    if (dllFile == null)
        throw new Exception($"Couldn't find built mod in {d}");
    var dllName = System.IO.Path.GetFileNameWithoutExtension(dllFile.ToString());
    var zipLoc = $"{d.GetParent()}/{dllName}.zip";
    Zip(d, zipLoc, GetFiles($"{d}/**/*"));
    Information($"Published {dllName} to {zipLoc}");
});

RunTarget(target);
