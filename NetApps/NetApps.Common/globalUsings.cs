global using Libx;
global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.Text;
global using System.Threading.Tasks;
global using System.Data;
global using System.Data.Common;
global using static Libx.GlobalThis;

namespace Libx;
public static class GlobalThis
{
    public readonly static bool RunThis; // { get; set; }
    public readonly static bool NotRunThis; // { get; set; }
    public static void Write(object o) => MOutput.Write(o + "");

    static GlobalThis()
    {
        RunThis = true;
        NotRunThis = false;
    }

    public static void Never(string mesg = null)
    {
        throw new NotImplementedException("Never here: " + mesg);
    }

    //public static async Task NeverAsync(string mesg = null)
    //{
    //    //https://www.meziantou.net/awaiting-an-async-void-method-in-dotnet.htm
    //    if (RunThis) throw new NotImplementedException("Never here: " + mesg);
    //    await Task.Delay(1);
    //    //await Run(() => Test());

    //    await Task.Run(() =>
    //    {
    //        //;
    //    });
    //}

    public static void Noop(string mesg = null)
    {
        MOutput.WriteLine("Noop: " + mesg);
    }

    public static async Task NoopAsync(string mesg = null)
    {
        if (NotRunThis) await Task.Delay(1);
        MOutput.WriteLine("NoopAsync: " + mesg);
    }

}