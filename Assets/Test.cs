using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public int m_value;

	public int m_valueInMemory;

	private EntangledInt m_hv;

	public Rect r;

	// Use this for initialization
	void Start () {
		m_hv = new EntangledInt();
		m_hv.Value = m_value;
		m_valueInMemory = m_hv.m_value;
	}
	
	// Update is called once per frame
	void Update () {
		try {
			guiText.text = "Value : " + m_hv.Value;
			m_valueInMemory = m_hv.m_value;
		} catch(System.Security.SecurityException e) {
			guiText.text = e.Message;
		}
	}

	void OnGUI() {
		GUILayout.BeginArea(r);
		GUILayout.Label (gameObject.name);
		m_value = System.Int32.Parse ( GUILayout.TextField(m_value.ToString()) );
		if( GUILayout.Button ("Safe Change") ) {
			// do safe change
			m_hv.Value = m_value;
		}
		else if( GUILayout.Button ("Hack!") ) {
			// do hack change
			int mask = m_hv.m_mask;
			m_hv.m_value = m_value ^ mask;
		}
		GUILayout.EndArea();
	}
}
