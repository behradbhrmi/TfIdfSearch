using System.Diagnostics;

public static class PythonInterPreter
{
    static string _pythonInterPreterPath = Path.Combine(AppContext.BaseDirectory, "py\\venv\\Scripts\\python.exe");

    static string _pythonOCRFilePath = Path.Combine(AppContext.BaseDirectory, "py/ocr_extractor.py");

    static string _pythonWordProcessFilePath = Path.Combine(AppContext.BaseDirectory, "py/word_process.py");

    public static  string[] CreateProcess(string fileName, string argument)
    {
        if (!File.Exists(fileName)) return ["notFound"];

        ManualResetEventSlim cancelEvent = new ManualResetEventSlim();
        List<string> result = new();

        string arguments = argument;

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var thread = new Thread(() =>
        {
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();

                //while (!process.HasExited && !cancelEvent.Wait(100))
                //    if (cancellationToken.IsCancellationRequested)
                //    {
                //        process.Kill();
                //        result.Add("end");
                //        break;
                //    }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                result = output.Split().ToList();
            }
        });
        thread.Start();

        return result.ToArray();
    }

    public static string[] ReadImageContent(string path)
    {
        string arguments = $"{_pythonOCRFilePath} {path}";

        return CreateProcess(_pythonInterPreterPath, arguments);
    }

    public static string[] ProcessContent(string text)
    {
        string arguments = $"{_pythonWordProcessFilePath} {text}";

        return CreateProcess(_pythonInterPreterPath, arguments);

    }
}