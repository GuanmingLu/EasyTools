using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.InternalComponent {

	public class Comment : MonoBehaviour {
#if UNITY_EDITOR
		[SerializeField] private string m_comment;
#endif
	}
}
