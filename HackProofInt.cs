using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class HackProofInt {

	private static string s_hashkey = "put whatever string here";

	private int m_value;
	private int m_magic;
	private byte[] m_hash;
	private HMACSHA1 m_hmac;

	public int Value {
		get {
			if( !Check () ) {
				throw new System.Security.SecurityException("unexpected value change detected");
			}
			return m_value - m_magic;
		}
		set {
			m_value = value + m_magic;
			m_hash = m_hmac.ComputeHash( System.BitConverter.GetBytes(m_value) );
		}
	}

	public bool Check() {
		byte[] h = m_hmac.ComputeHash( System.BitConverter.GetBytes(m_value) );

		if( h.Length != m_hash.Length ) {
			return false;
		}

		for(int i = 0; i<h.Length; ++i) {
			if( h[i] != m_hash[i] ) {
				return false;
			}
		}
		return true;
	}

	public HackProofInt() {
		_Initialize(0);
	}

	public HackProofInt(int i) {
		_Initialize(i);
	}

	private void _Initialize(int i) {
		m_hmac = new HMACSHA1(System.Text.Encoding.UTF8.GetBytes(s_hashkey));
		m_magic = Random.Range(int.MaxValue / 2, int.MaxValue);
		m_value = unchecked(i + m_magic);
		m_hash = m_hmac.ComputeHash( System.BitConverter.GetBytes(m_value) );
	}
}
