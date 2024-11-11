using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyTools {

	public static class StreamingAssetsReader {
		private const string FileListName = "EasyTools-StreamingAssetsReader-FileList.g.jsonc";
		private static string ToPath(this string path) => path.Replace('\\', '/');

		private static JObject _fileList;
		private static JObject GetFileList() => _fileList ??= JObject.Parse(Download(FileListName).text);

		private static JToken GetTree(string relativePath) {
			JToken obj = GetFileList();
			foreach (var p in relativePath.ToPath().Split('/', StringSplitOptions.RemoveEmptyEntries)) {
				if (obj == null) return null;
				obj = obj[p];
			}
			return obj;
		}

		public static DownloadHandler Download(string relativePath) {
			var path = Path.Combine(Application.streamingAssetsPath, relativePath);
			var request = UnityWebRequest.Get(path.ToPath());
			request.SendWebRequest().WaitForComplete();
			return request.downloadHandler;
		}

		public static async Task<DownloadHandler> DownloadAsync(string relativePath) {
			var path = Path.Combine(Application.streamingAssetsPath, relativePath);
			var request = UnityWebRequest.Get(path.ToPath());
			await request.SendWebRequest();
			return request.downloadHandler;
		}

		public static string ReadAllText(string relativePath) => Download(relativePath).text;
		public static async Task<string> ReadAllTextAsync(string relativePath)
			=> (await DownloadAsync(relativePath)).text;
		public static byte[] ReadAllBytes(string relativePath) => Download(relativePath).data;
		public static async Task<byte[]> ReadAllBytesAsync(string relativePath)
			=> (await DownloadAsync(relativePath)).data;

		public static bool IsFile(string relativePath) => GetTree(relativePath)?.Type == JTokenType.Integer;
		public static bool IsDirectory(string relativePath) => GetTree(relativePath)?.Type == JTokenType.Object;

		private static IEnumerable<JProperty> EnumerateProperties(string relativePath) {
			var tree = GetTree(relativePath);
			if (tree?.Type == JTokenType.Object)
				return tree.Value<JObject>().Properties();
			else return Enumerable.Empty<JProperty>();
		}

		public static IEnumerable<string> Enumerate(string relativePath) => EnumerateProperties(relativePath)
			.Select(p => Path.Combine(relativePath, p.Name).ToPath()).OrderBy(p => p);

		public static IEnumerable<string> EnumerateDirectories(string relativePath)
			=> EnumerateProperties(relativePath).Where(p => p.Value.Type == JTokenType.Object)
				.Select(p => Path.Combine(relativePath, p.Name).ToPath()).OrderBy(p => p);

		public static IEnumerable<string> EnumerateFiles(string relativePath)
			=> EnumerateProperties(relativePath).Where(p => p.Value.Type == JTokenType.Integer)
				.Select(p => Path.Combine(relativePath, p.Name).ToPath()).OrderBy(p => p);

#if UNITY_EDITOR
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void Auto_GenerateFileListJson() {
			if (!Directory.Exists(Application.streamingAssetsPath)) return;

			static JObject Generate(string path) {
				JObject tree = new();
				foreach (var file in Directory.EnumerateFiles(path)) {
					var name = Path.GetFileName(file);
					if (name == FileListName || Path.GetExtension(file) == ".meta") continue;
					tree.Add(name, JToken.FromObject(new FileInfo(file).Length));
				}
				foreach (var dir in Directory.EnumerateDirectories(path)) {
					tree.Add(Path.GetFileName(dir), Generate(dir));
				}
				return tree;
			}

			File.WriteAllText(Path.Combine(Application.streamingAssetsPath, FileListName), Generate(Application.streamingAssetsPath).ToString(Newtonsoft.Json.Formatting.Indented));
		}
#endif

	}
}
