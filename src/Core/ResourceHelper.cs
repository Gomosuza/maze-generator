using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core
{
	/// <summary>
	/// Helper class for working with embedded resources.
	/// </summary>
	public static class ResourceHelper
	{
		#region Methods

		/// <summary>
		/// Returns whether a given resource exists.
		/// </summary>
		/// <param name="asm">The assembly to look in.</param>
		/// <param name="resourceName">The name of the resource, including folder structure if it was not stored in the root of the assembly.</param>
		/// <param name="defaultNamespace">C# doesn't allow access to default namespace. If left null, the assemblies name will be used.</param>
		/// <returns></returns>
		public static bool HasResource(Assembly asm, string resourceName, string defaultNamespace = null)
		{
			var res = ParseResourceName(asm, resourceName, defaultNamespace);
			return asm.GetManifestResourceNames().Contains(res);
		}

		private static string ParseResourceName(Assembly asm, string resourceName, string defaultNamespace = null)
		{
			return (defaultNamespace ?? asm.GetName().Name) + "." + resourceName.Replace("\\", ".");
		}

		/// <summary>
		/// Writes the given resource to disk.
		/// Returns false if resource does not exist or target path is already in use.
		/// </summary>
		/// <param name="asm">The assembly to look in.</param>
		/// <param name="resourceName">The name of the resource, including folder structure if it was not stored in the root of the assembly.</param>
		/// <param name="targetFileOnDisk">The path where the file should be written to.</param>
		/// <param name="defaultNamespace">C# doesn't allow access to default namespace. If left null, the assemblies name will be used.</param>
		/// <returns>True if resource exists and write was successful, false otherwise.</returns>
		public static bool WriteResourceToDisk(Assembly asm, string resourceName, string targetFileOnDisk, string defaultNamespace = null)
		{
			if (asm == null)
			{
				throw new ArgumentNullException(nameof(asm));
			}
			if (string.IsNullOrEmpty(resourceName))
			{
				throw new ArgumentNullException(nameof(resourceName));
			}
			if (string.IsNullOrEmpty(targetFileOnDisk))
			{
				throw new ArgumentNullException(nameof(targetFileOnDisk));
			}
			if (!HasResource(asm, resourceName, defaultNamespace))
			{
				return false;
			}
			if (File.Exists(targetFileOnDisk))
			{
				return false;
			}
			var dir = Path.GetDirectoryName(targetFileOnDisk);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			var resource = ParseResourceName(asm, resourceName, defaultNamespace);
			using (var stream = asm.GetManifestResourceStream(resource))
			{
				using (var file = File.OpenWrite(targetFileOnDisk))
				{
					if (stream == null)
					{
						throw new InvalidOperationException("Resource does not exist, even though we just checked that it does.");
					}
					stream.CopyTo(file);
				}
			}
			return true;
		}

		#endregion
	}
}