using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Data;

namespace EasyTools.Editor {
	public class VideoImporter : EditorWindow {

		private string _sourcePath = "请选择源视频";
		private bool _selectedSource = false;
		private string _sourceInfo = "";

		private string _outputPath;

		private Vector2Int _resolution;
		private int _bitrate;
		private int _frameRate;


		[MenuItem("EasyTools/工具/导入视频")]
		static void Init() {
			var window = GetWindow<VideoImporter>(false, "导入视频", true);
			window.OnShow();
		}

		private void OnShow() {
			Show();

			var pos = position;
			pos.width = 400;
			pos.height = 400;
			position = pos;
		}

		private void OnGUI() {

			using (new Utils.BoxGroupScope("源")) {
				if (GUILayout.Button(_sourcePath)) {
					_sourcePath = EditorUtility.OpenFilePanelWithFilters("选择源视频", _sourcePath, new[] { "Video files", "mp4,avi,mpg,mpeg,mov,wmv,asf,asx,flv,rm,rmvb,mkv,webm,ts,vob,ogv,3gp,3g2,m4v,dat,divx,mtv,m2v,mpe,mpv2,mpeg1,mpeg2,mpeg4,ogm,qt,amv,dv,f4v,gif,m2ts,m2v,m4p,m4v,mkv,mng,mp4v,mpg2,ogx,qt,rm,swf,vob,wmv" });

					try {
						_sourceInfo = FfProbe($"\"{_sourcePath}\" -show_entries stream=codec_name,width,height,r_frame_rate:format=bit_rate -v error -print_format json");

						var obj = JObject.Parse(_sourceInfo);
						_resolution.x = obj["streams"][0]["width"].Value<int>();
						_resolution.y = obj["streams"][0]["height"].Value<int>();
						_bitrate = int.Parse(obj["format"]["bit_rate"].Value<string>());
						_frameRate = int.Parse(new DataTable().Compute(obj["streams"][0]["r_frame_rate"].Value<string>(), null).ToString());

						_sourceInfo = $"分辨率={_resolution.x}x{_resolution.y}; 码率={_bitrate:N0}; 帧率={_frameRate}; 编码={obj["streams"][0]["codec_name"].Value<string>()}";
						_selectedSource = true;
					}
					catch (JsonReaderException) { }
				}
				if (!string.IsNullOrWhiteSpace(_sourceInfo)) EditorGUILayout.LabelField(_sourceInfo);
			}
			if (!_selectedSource) return;

			using (new Utils.BoxGroupScope("输出路径"))
				Utils.ChooseProjectPath(ref _outputPath);
			if (!AssetDatabase.IsValidFolder(_outputPath)) return;

			using (new Utils.BoxGroupScope("输出参数")) {
				_resolution = EditorGUILayout.Vector2IntField("分辨率", _resolution);
				_bitrate = EditorGUILayout.IntField("码率", _bitrate);
				_frameRate = EditorGUILayout.IntField("帧率", _frameRate);
			}

			using (new Utils.BoxGroupScope("导入")) {
				if (GUILayout.Button("导入视频")) {
					var outputPath = $"{Utils.AssetPathToFullPath(_outputPath)}/{Path.GetFileNameWithoutExtension(_sourcePath)}.webm";
					Ffmpeg($"-i \"{_sourcePath}\" -c:v libvpx -b:v {_bitrate} -s {_resolution.x}x{_resolution.y} -r {_frameRate} -c:a libvorbis \"{outputPath}\"");
					AssetDatabase.Refresh();
				}
			}
		}

		private string _exeDir;
		private string ExeDir => _exeDir ??= Path.GetDirectoryName(Utils.GetAssetFullPath(MonoScript.FromScriptableObject(this)));
		private string FfmpegPath => ExeDir + "\\ffmpeg.exe";
		private string FfProbe(string args) {
			Process p = new();
			p.StartInfo.FileName = ExeDir + "\\ffprobe.exe";
			p.StartInfo.Arguments = args;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();
			var err = p.StandardError.ReadToEnd();
			return (string.IsNullOrWhiteSpace(err) ? p.StandardOutput.ReadToEnd() : err).Trim('\r', '\n');
		}

		private void Ffmpeg(string args) {
			Process p = new();
			p.StartInfo.FileName = ExeDir + "\\ffmpeg.exe";
			p.StartInfo.Arguments = args;
			p.StartInfo.UseShellExecute = true;
			p.Start();
			p.WaitForExit();
		}
	}
}
