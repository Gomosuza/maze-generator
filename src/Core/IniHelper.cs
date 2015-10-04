using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Core
{
	/// <summary>
	/// Generic helper class to read and write ini files.
	/// Note that this is not intended for high performance operations, as each call will open the file anew.
	/// </summary>
	public static class IniHelper
	{
		#region Methods

		/// <summary>
		/// Reads the specific key from the specific section.
		/// Returns null if file, section or key was not found.
		/// </summary>
		/// <param name="fileName">Path must be rooted.</param>
		/// <param name="sectionName">The section to look in.</param>
		/// <param name="keyName">The key to look for.</param>
		/// <param name="capacity">The max supported size for the return value.</param>
		/// <returns>Null if section or key do not exist.</returns>
		public static string ReadValue(string fileName, string sectionName, string keyName, int capacity = 500)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentNullException(nameof(sectionName));
			}
			if (string.IsNullOrEmpty(keyName))
			{
				throw new ArgumentNullException(nameof(keyName));
			}
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			if (!Path.IsPathRooted(fileName))
			{
				// we enforce this, as GetPrivateProfileString will search in windows directory when path is relative
				// which in 99% of all cases is not what we'd want/expect
				throw new NotSupportedException("Path must be rooted");
			}
			try
			{
				var sb = new StringBuilder(capacity);

				var status = GetPrivateProfileString(sectionName, keyName, "", sb, sb.Capacity, fileName);
				if (status == 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				return sb.ToString();
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Writes the specific value to the key in the specific section.
		/// </summary>
		/// <returns>True on success, false on error.</returns>
		public static bool WriteValue(string fileName, string sectionName, string keyName, string newValue)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentNullException(nameof(sectionName));
			}
			try
			{
				var status = WritePrivateProfileString(sectionName, keyName, newValue, fileName);

				if (status == 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFilePath);

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string lPAppName, string lpKeyName, string lpString, string lpFileName);

		#endregion
	}
}