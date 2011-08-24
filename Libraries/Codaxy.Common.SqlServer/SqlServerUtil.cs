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
		public static SqlScript[] ReadZipArchive(Stream zipPackageStream)
		{
			List<SqlScript> result = new List<SqlScript>();
			using (var zipFile = new ZipFile(zipPackageStream))
			{
				foreach (ZipEntry ze in zipFile)
				{
					if (ze.Name.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase))
					{
						using (var zs = zipFile.GetInputStream(ze))
						using (var sr = new StreamReader(zs))
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
			return result.ToArray();
		}

		public static SqlScript[] ReadDirectory(String directoryPath)
		{
			List<SqlScript> result = new List<SqlScript>();
			var info = new DirectoryInfo(directoryPath);
			foreach (var file in info.GetFiles("*.sql"))
			{
				result.Add(new SqlScript { SQL = File.ReadAllText(file.FullName), Name = file.Name });
			}
			return result.ToArray();
		}
	}
}
