using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils.Iso9660;
using System.IO;
using System.Diagnostics;

namespace Heroes1ModInstallator
{
    class IsoFromDirectory
    {
        static public void Create(string dir, string isoPath)
        {
            CDBuilder builder = new CDBuilder();
            builder.UseJoliet = true;
            builder.VolumeIdentifier = "H1_GOG_FILE";
            var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                string inIsoPath = file.Substring(dir.Length);
                builder.AddFile(inIsoPath, file);
            }
            builder.Build(isoPath);
        }
    }
}
