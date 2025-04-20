using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.Diagnostics;

public class NodeJsSharp
{
    public string SessionId { get; private set; }
    public string NodeJsSharpDirectory { get; }

    [DllImport("user32.dll")]
    private static extern int SetWindowText(IntPtr hWnd, string text);

    public NodeJsSharp(string sessionId)
    {
        SessionId = sessionId;
        NodeJsSharpDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\nodejs_sharp";
    }

    public void ClearSession()
    {
        if (Directory.Exists(NodeJsSharpDirectory))
        {
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    if (File.Exists($"nodejs_sharp_session_{SessionId}_message_{i}_from_csharp_to_js"))
                    {
                        File.Delete($"nodejs_sharp_session_{SessionId}_message_{i}_from_csharp_to_js");
                    }
                }
                catch
                {

                }

                try
                {
                    if (File.Exists($"nodejs_sharp_session_{SessionId}_message_{i}_from_js_to_csharp"))
                    {
                        File.Delete($"nodejs_sharp_session_{SessionId}_message_{i}_from_js_to_csharp");
                    }
                }
                catch
                {

                }
            }
        }
        else
        {
            Directory.CreateDirectory(NodeJsSharpDirectory);
        }
    }

    public void WriteMessage(string message)
    {
        if (!Directory.Exists(NodeJsSharpDirectory))
        {
            Directory.CreateDirectory(NodeJsSharpDirectory);
        }

        for (int i = 1; i <= 100; i++)
        {
            try
            {
                if (!File.Exists($"{NodeJsSharpDirectory}\\nodejs_sharp_session_{SessionId}_message_{i}_from_csharp_to_js"))
                {
                    File.WriteAllText($"{NodeJsSharpDirectory}\\nodejs_sharp_session_{SessionId}_message_{i}_from_csharp_to_js", message);
                    break;
                }
            }
            catch
            {

            }
        }
    }

    public string ReadMessage()
    {
        if (!Directory.Exists(NodeJsSharpDirectory))
        {
            Directory.CreateDirectory(NodeJsSharpDirectory);
        }

        while (true)
        {
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    if (File.Exists($"{NodeJsSharpDirectory}\\nodejs_sharp_session_{SessionId}_message_{i}_from_js_to_csharp"))
                    {
                        return File.ReadAllText($"{NodeJsSharpDirectory}\\nodejs_sharp_session_{SessionId}_message_{i}_from_js_to_csharp");
                    }
                }
                catch
                {

                }
            }
        }
    }

    public void RunScript(string fileName, bool noWindow = false, string parameters = "")
    {
        if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".js"))
        {
            fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".js";
        }

        if (!File.Exists(fileName))
        {
            throw new Exception("The specified script file does not exist.");
        }

        if (!Path.GetExtension(fileName).ToLower().Equals(".js"))
        {
            throw new Exception("The specified script file has an invalid extension.");
        }

        Thread thread = new Thread(() =>
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = "cmd.exe";
            startInfo.WindowStyle = noWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            startInfo.CreateNoWindow = noWindow;

            if (parameters == null || parameters.Replace(" ", "").Replace('\t'.ToString(), "") == "")
            {
                startInfo.Arguments = $"/c node \"{fileName}\" {SessionId} {parameters}";
            }
            else
            {
                startInfo.Arguments = $"/c node \"{fileName}\" {SessionId}";
            }

            Process proc = Process.Start(startInfo);
            Thread.Sleep(1000);

            if (!noWindow)
            {
                SetWindowText(proc.MainWindowHandle, $"Node.js script \"{Path.GetFileName(fileName).ToLower()}\" - Session ID \"{SessionId}\"");
            }
        });

        thread.Priority = ThreadPriority.Highest;
        thread.Start();
    }
}