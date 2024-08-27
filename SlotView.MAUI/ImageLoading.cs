using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Maui.Platform;

namespace SlotView.Maui;
public static class ImageLoading
{
	// thx to https://github.com/Esri/arcgis-maps-sdk-dotnet-samples
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	static async Task<Stream> LoadImageFromFile(string file, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		if (Path.IsPathRooted(file) && File.Exists(file))
			return File.OpenRead(file);

#if ANDROID
		var context = Android.App.Application.Context;
		var resources = context.Resources;

		var resourceId = context.GetDrawableId(file);
		if (resourceId > 0)
		{
			var imageUri = new Android.Net.Uri.Builder()
				.Scheme(Android.Content.ContentResolver.SchemeAndroidResource)
				.Authority(resources.GetResourcePackageName(resourceId))
				.AppendPath(resources.GetResourceTypeName(resourceId))
				.AppendPath(resources.GetResourceEntryName(resourceId))
				.Build();

			var stream = context.ContentResolver.OpenInputStream(imageUri);
			if (stream is not null)
				return stream;

/* Unmerged change from project 'SlotView.Maui(net8.0-android)'
Before:
        }
#elif WINDOWS
        try
        {
            var sf = await Windows.Storage.StorageFile.GetFileFromPathAsync(file);
            if (sf is not null)
            {
                var stream = await sf.OpenStreamForReadAsync();
                if (stream is not null)
                    return stream;
            }
        }
        catch
        {
        }

        if (AppInfo.PackagingModel == AppPackagingModel.Packaged)
        {
            var uri = new Uri("ms-appx:///" + file);
            var sf = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await sf.OpenStreamForReadAsync();
            if (stream is not null)
                return stream;
        }
        else
        {
            var root = AppContext.BaseDirectory;
            file = Path.Combine(root, file);
            if (File.Exists(file))
                return File.OpenRead(file);
        }
#elif IOS || MACCATALYST
        var root = Foundation.NSBundle.MainBundle.BundlePath;
#if MACCATALYST || MACOS
        root = Path.Combine(root, "Contents", "Resources");
#endif
        file = Path.Combine(root, file);
        if (File.Exists(file))
            return File.OpenRead(file);
#endif

        return null;
After:
		}
#elif WINDOWS
        try
        {
            var sf = await Windows.Storage.StorageFile.GetFileFromPathAsync(file);
            if (sf is not null)
            {
                var stream = await sf.OpenStreamForReadAsync();
                if (stream is not null)
                    return stream;
            }
        }
        catch
        {
        }

        if (AppInfo.PackagingModel == AppPackagingModel.Packaged)
        {
            var uri = new Uri("ms-appx:///" + file);
            var sf = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await sf.OpenStreamForReadAsync();
            if (stream is not null)
                return stream;
        }
        else
        {
            var root = AppContext.BaseDirectory;
            file = Path.Combine(root, file);
            if (File.Exists(file))
                return File.OpenRead(file);
        }
#elif IOS || MACCATALYST
        var root = Foundation.NSBundle.MainBundle.BundlePath;
#if MACCATALYST || MACOS
        root = Path.Combine(root, "Contents", "Resources");
#endif
        file = Path.Combine(root, file);
        if (File.Exists(file))
            return File.OpenRead(file);
#endif

		return null;
*/
        }
#elif WINDOWS
		try
		{
			var sf = await Windows.Storage.StorageFile.GetFileFromPathAsync(file);
			if (sf is not null)
			{
				var stream = await sf.OpenStreamForReadAsync();
				if (stream is not null)
					return stream;
			}
		}
		catch
		{
		}

		if (AppInfo.PackagingModel == AppPackagingModel.Packaged)
		{
			var uri = new Uri("ms-appx:///" + file);
			var sf = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
			var stream = await sf.OpenStreamForReadAsync();
			if (stream is not null)
				return stream;
		}
		else
		{
			var root = AppContext.BaseDirectory;
			file = Path.Combine(root, file);
			if (File.Exists(file))
				return File.OpenRead(file);

/* Unmerged change from project 'SlotView.Maui(net8.0-windows10.0.19041.0)'
Before:
        }
#elif IOS || MACCATALYST
        var root = Foundation.NSBundle.MainBundle.BundlePath;
#if MACCATALYST || MACOS
        root = Path.Combine(root, "Contents", "Resources");
#endif
        file = Path.Combine(root, file);
        if (File.Exists(file))
            return File.OpenRead(file);
#endif

        return null;
After:
		}
#elif IOS || MACCATALYST
        var root = Foundation.NSBundle.MainBundle.BundlePath;
#if MACCATALYST || MACOS
        root = Path.Combine(root, "Contents", "Resources");
#endif
        file = Path.Combine(root, file);
        if (File.Exists(file))
            return File.OpenRead(file);
#endif

		return null;
*/
        }
#elif IOS || MACCATALYST
		var root = Foundation.NSBundle.MainBundle.BundlePath;
#if MACCATALYST || MACOS
		root = Path.Combine(root, "Contents", "Resources");
#endif
		file = Path.Combine(root, file);
		if (File.Exists(file))
			return File.OpenRead(file);
#endif

		return null;
	}

	static async Task<Stream> LoadImageFromUri(string uri, CancellationToken cancellationToken = default)
	{
		using var client = new HttpClient();
		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}
		return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
	}

	public static async Task<Stream> GetImageStream(string path, CancellationToken cancellationToken = default)
	{
		try
		{
			if (path is null) return null;
			if (string.IsNullOrEmpty(path)) return null;
			if (IsWebUrl(path))
			{
				return await LoadImageFromUri(path);
			}
			else
			{
				return await LoadImageFromFile(path);
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("IMAGE LOADING EXCEPTION: " + ex);
			return null;
		}
	}

	static bool IsWebUrl(string url)
	{
		// Regular expression to check if a string is a web URL
		string pattern = @"^(https?://|www\.)\S+$";
		return Regex.IsMatch(url, pattern);

/* Unmerged change from project 'SlotView.Maui(net8.0-android)'
Before:
    }
After:
	}
*/

/* Unmerged change from project 'SlotView.Maui(net8.0-ios)'
Before:
    }
After:
	}
*/

/* Unmerged change from project 'SlotView.Maui(net8.0-maccatalyst)'
Before:
    }
After:
	}
*/

/* Unmerged change from project 'SlotView.Maui(net8.0-windows10.0.19041.0)'
Before:
    }
After:
	}
*/
	}
}
