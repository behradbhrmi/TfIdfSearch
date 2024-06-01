using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.ConsoleApp.Extensions;

public static class ConvertToPath
{
    public static string StringToPath(this string path)
    {
        return path.Replace("/", "\\");
    }
}
