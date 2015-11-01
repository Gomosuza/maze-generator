using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Core;
using Core.Extensions;

namespace Engine
{
	/// <summary>
	/// Helper class for static resources that are embedded with the engine.
	/// Resources in this cache are generally loaded on first call and kept alive forever.
	/// </summary>
	public static class StaticResourceCache
	{
		#region Fields

		private static readonly object _resourceLock = new object();

		private static SpriteFont _large;
		private static SpriteFont _small;
		private static Texture2D _white;

		#endregion

		#region Methods

		/// <summary>
		/// A large font has been embedded in the engine to be used. Its font size is currently 64.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static SpriteFont GetLargeFont(ContentManager content)
		{
			if (_large == null)
			{
				lock (_resourceLock)
				{
					if (_large == null)
					{
						_large = LoadFont(content, "InternalContent\\InternalEngineFont_Segoe UI_64.xnb");
					}
				}
			}
			return _large;
		}

		/// <summary>
		/// A small font has been embedded in the engine to be used. Its font size is currently 16.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static SpriteFont GetSmallFont(ContentManager content)
		{
			if (_small == null)
			{
				lock (_resourceLock)
				{
					if (_small == null)
					{
						_small = LoadFont(content, "InternalContent\\InternalEngineFont_Segoe UI_16.xnb");
					}
				}
			}
			return _small;
		}

		/// <summary>
		/// Returns a single pixel 1x1 white texture.
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public static Texture2D GetWhite(GraphicsDevice device)
		{
			if (_white == null)
			{
				lock (_resourceLock)
				{
					if (_white == null)
					{
						_white = new Texture2D(device, 1, 1);
						_white.SetData(new[]
						{
							Color.White
						});
					}
				}
			}
			return _white;
		}

		/// <summary>
		/// Deletes the passed directory if it is empty and tries to walk up to the parent and repeats the same step again.
		/// Will delete <paramref name="dirsToDeleteInHierarchy"/> many directories. If you only want to delete the provided directory, set it to 1.
		/// If you want to delete the provided and then the parent dir (given it is also empty) set it to 2, etc.
		/// </summary>
		/// <param name="directory">The first directory to delete.</param>
		/// <param name="dirsToDeleteInHierarchy">The total maximum number of directory nodes to delete.</param>
		private static void DeleteEmptyDirectories(string directory, int dirsToDeleteInHierarchy)
		{
			while (!string.IsNullOrEmpty(directory) && dirsToDeleteInHierarchy > 0)
			{
				if (Directory.Exists(directory))
				{
					var hasContent = Directory.GetFileSystemEntries(directory).Any();
					if (hasContent)
					{
						return;
					}
					Directory.Delete(directory);
					dirsToDeleteInHierarchy--;
					if (!directory.Contains("\\"))
					{
						return;
					}
					directory = directory.Substring(0, directory.LastIndexOf('\\'));
				}
				else
				{
					return;
				}
			}
		}

		private static SpriteFont LoadFont(ContentManager content, string resourceName)
		{
			SpriteFont font;
			var root = Assembly.GetEntryAssembly().GetDirectory();
			var tempFile = Path.Combine(root, content.RootDirectory, resourceName);
			try
			{
				if (!ResourceHelper.WriteResourceToDisk(typeof(StaticResourceCache).Assembly, resourceName, tempFile))
				{
					throw new InvalidOperationException("Unable to write resource to disk.");
				}
				var contentManagerFriendlyName = resourceName;
				if (contentManagerFriendlyName.EndsWith(".xnb", true, CultureInfo.InvariantCulture))
				{
					contentManagerFriendlyName = contentManagerFriendlyName.Substring(0, contentManagerFriendlyName.Length - ".xnb".Length);
				}
				font = content.Load<SpriteFont>(contentManagerFriendlyName);
				File.Delete(tempFile);
				if (resourceName.Contains("\\"))
				{
					// delete as many empty dirs as we just had to create
					// +1 because we may create the "Content" directory as well, which may not exist if the game in question doesn't have any custom content
					var dirCountToDelete = resourceName.Count(c => c == '\\') + 1;
					DeleteEmptyDirectories(Path.GetDirectoryName(tempFile), dirCountToDelete);
				}
			}
			finally
			{
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
			}
			if (font == null)
			{
				throw new ContentLoadException($"Engine could not load an internal resource. '{resourceName}'. See inner exception for more details.",
					new FileLoadException($"Failure while trying to load embedded resource '{resourceName}'. Tried to copy it to '{tempFile}' " +
										  $"but either path is in use or not part of the content directory. ContentManager said it loads from '{content.RootDirectory}'." +
										  $"Assumed this is relative to: {root}."));
			}
			return font;
		}

		#endregion
	}
}