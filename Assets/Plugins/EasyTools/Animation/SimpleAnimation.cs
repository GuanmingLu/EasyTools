using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.Animation {

	public class SimpleAnimation : MonoBehaviour {
		[SerializeField] private Vector3 m_rotateSpeed;

		void Update() {
			transform.Rotate(m_rotateSpeed * Time.deltaTime);
		}
	}
}
