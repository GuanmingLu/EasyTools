using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using EasyTools.Settings;

namespace EasyTools.InternalComponent {
	internal class EasyDialogComponent : MonoBehaviour {
		[SerializeField] private TMP_Text m_nameText;
		[SerializeField] private TMP_Text m_contentText;
		[SerializeField] private Transform m_avatarRoot;


		#region Tag处理

		private const string AvailableSingleTags = "showAvatar|oneAvatar";
		private const string AvailableRangeTags = "shake|wave";
		private const string SingleTagLink = "cTag";

		private string PreProcessDialog(string dialog) => TMP_TagExtension.PreProcessDialog(dialog, AvailableSingleTags, AvailableRangeTags);

		private void OnSingleTagShow(TMP_SingleTag tag) {
			Debug.Log($"OnSingleTagShow: {tag.name} {tag.MainParam}");
			switch (tag.name) {
				case "showAvatar":
					ShowAvatar(tag);
					break;
				case "oneAvatar":
					HideAllAvatars();
					ShowAvatar(tag);
					break;
			}
		}

		#endregion

		#region 头像

		private Dictionary<string, Image> _avatars = new();
		private void HideAllAvatars() => _avatars.Values.ForEach(a => a.gameObject.SetActive(false));
		private void ShowAvatar(TMP_SingleTag tag) {
			var key = tag.MainParam;

			RectTransform t;

			var avatarName = key.Split('.')[0];

			if (string.IsNullOrWhiteSpace(avatarName)) return;

			if (_avatars.TryGetValue(avatarName, out var avatar)) {
				t = avatar.rectTransform;
			}
			else {
				t = new GameObject(avatarName, typeof(Image)).transform.ToRect();
				t.SetParent(m_avatarRoot);

				avatar = t.GetComponent<Image>();
				_avatars.Add(avatarName, avatar);
			}

			if (EasyToolsSettings.TryGetAvatar(key, out var sprite)) {
				avatar.sprite = sprite;
				avatar.SetNativeSize();
				t.anchoredPosition = tag.GetV2("pos", Vector2.zero);
				t.localRotation = Quaternion.identity;
				t.SetAllScale(tag.GetFloat("scale", 1));
				avatar.gameObject.SetActive(true);
			}
			else {
				avatar.gameObject.SetActive(false);
			}

		}

		#endregion

		#region 读取并显示对话

		/// <summary>
		/// 读取对话文件并显示连续的多段对话 <br/>
		/// </summary>
		/// <example>
		/// SampleDialog 文件名对应的文件路径为
		/// StreamingAssets/EasyTools/Localization/zh-CN/Dialogues/SampleDialog.json
		/// 文件内容为 [{ name": "角色名", "content": "对话内容" }, ...]
		/// </example>
		internal IEnumerator ShowDialogues(string fileName) {
			var key = $"Dialogues/{fileName}";
			Debug.Log(key);
			return ShowDialogues(EasyLocalization.AsArray<EasyDialogContent>(key));
		}

		/// <summary>
		/// 显示连续的多段对话
		/// </summary>
		internal IEnumerator ShowDialogues(IEnumerable<EasyDialogContent> contents) {
			gameObject.SetActive(true);
			foreach (var dialog in contents) {
				Debug.Log(dialog.name);
				yield return ShowDialog(dialog.name, dialog.content);
			}
			gameObject.SetActive(false);
		}

		private Coroutine _inputHandler;
		private bool _skipped;
		IEnumerator InputHandler() {
			_skipped = false;
			while (true) {
				yield return null;
				if (EasyToolsSettings.NextInput) _skipped = true;
			}
		}

		/// <summary>
		/// 显示一段对话并等待一次点击
		/// </summary>
		private IEnumerator ShowDialog(string name, string content) {
			m_nameText.text = name;
			m_contentText.maxVisibleCharacters = 0;
			m_contentText.text = PreProcessDialog(content);
			m_contentText.textInfo.linkInfo = new TMP_LinkInfo[0];
			m_contentText.ForceMeshUpdate();

			yield return null;

			InputHandler().RunOn(this, ref _inputHandler);
			var total = m_contentText.textInfo.characterCount;
			for (int i = 0; i < total; i++) {
				if (!_skipped) yield return Wait.Seconds(EasyToolsSettings.CharInterval);
				m_contentText.maxVisibleCharacters = i + 1;
				OnCharacterShow(i);
			}
			this.StopCoroutine(ref _inputHandler);

			yield return null;
			yield return Wait.Until(() => EasyToolsSettings.NextInput);
		}

		private void OnCharacterShow(int index) {
			m_contentText.GetSingleTagsOfIndex(index).ForEach(OnSingleTagShow);
		}

		#endregion
	}
}
