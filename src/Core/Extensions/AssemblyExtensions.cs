using System.IO;
using System.Reflection;

namespace Core.Extensions
{
	public static class AssemblyExtensions
	{
		#region Methods

		/// <summary>
		/// Returns the directory where the assembly was originally loaded from.
		/// This helps to work around the issue of shadow copying and will always return the original path.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		public static string GetDirectory(this Assembly asm)
		{
			var dir = new FileInfo(GetLocation(asm)).DirectoryName;
			if (!dir.EndsWith("\\"))
			{
				dir += "\\";
			}
			return dir;
		}

		/// <summary>
		/// Returns the full file path of the original location from the assembly.
		/// This helps to work around the issue of shadow copying and will always return the original path.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		public static string GetLocation(this Assembly asm)
		{
			var p = asm.CodeBase;
			if (p.StartsWith("file:///"))
			{
				p = p.Substring("file:///".Length);
			}
			return p.Replace("/", "\\");
		}

		#endregion
	}
}