using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Reflection;
using Codaxy.Common.Reflection;

namespace Codaxy.Common.SqlServer
{
	public class SqlScriptUtil
	{
		public static SqlScript[] ReadZipArchive(Stream zipPackageStream, Encoding encoding = null)
		{
			List<SqlScript> result = new List<SqlScript>();
			using (var zipFile = new ZipFile(zipPackageStream))
			{
				foreach (ZipEntry ze in zipFile)
				{
					if (ze.Name.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase))
					{
						using (var zs = zipFile.GetInputStream(ze))
						using (var sr = new StreamReader(zs, encoding ?? Encoding.UTF8))
						{
							var command = sr.ReadToEnd();
							result.Add(new SqlScript
							{
								Name = ze.Name,
								SQL = command
							});
						}
					}
				}
			}
			return result.OrderBy(a=>a.Name).ToArray();
		}

		public static SqlScript[] ReadDirectory(String directoryPath, Encoding encoding = null)
		{
			List<SqlScript> result = new List<SqlScript>();
			var info = new DirectoryInfo(directoryPath);
			foreach (var file in info.GetFiles("*.sql"))
			{
				result.Add(new SqlScript { SQL = File.ReadAllText(file.FullName, encoding ?? Encoding.UTF8), Name = file.Name });
			}
			return result.OrderBy(a=>a.Name).ToArray();
		}

        public static SqlScript[] ReadEmbeddedDirectory(Assembly assembly, String directory, bool absolutePath = false, Encoding encoding = null)
        {
            List<SqlScript> result = new List<SqlScript>();

            var internalDirectory = (absolutePath ? "" : assembly.GetName().Name + ".")  + directory.Replace("\\", ".").Trim('.') + ".";

            foreach (var file in assembly.GetManifestResourceNames())
            {
                if (file.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase)
                    && file.StartsWith(internalDirectory, StringComparison.InvariantCultureIgnoreCase))
                {
                    var fileName = file.Substring(internalDirectory.Length);

                    using (var fs = assembly.GetManifestResourceStream(file))
                    using (var sr = new StreamReader(fs, encoding ?? Encoding.UTF8))
                    {
                        var command = sr.ReadToEnd();
                        result.Add(new SqlScript
                        {
                            Name = fileName,
                            SQL = command
                        });
                    }
                }
            }
            return result.OrderBy(a=>a.Name).ToArray();
        }
	}
}
