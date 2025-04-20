using System;

public class Program
{
    public static void Main()
    {
        NodeJsSharp nodeJsSharp = new NodeJsSharp("test_session");
        nodeJsSharp.ClearSession();
        nodeJsSharp.RunScript("node_js_sharp_test.js");

        while (true)
        {
            nodeJsSharp.WriteMessage(Console.ReadLine());
        }
    }
}