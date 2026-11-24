using PicoGK;

string root = Path.GetFullPath("../../../");
string results = Path.Combine(root, "results");

Directory.CreateDirectory(results);

string log = Path.Combine(results, "helix-heatx.log");

try
{
    Library.Go(
        1.0f,
        Leap71.CoolCube.HelixHeatX.Task,
        log,
        true,
        "Kimi GeometryLab - HelixHeatX");
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    Environment.ExitCode = 1;
}
