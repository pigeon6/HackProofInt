/*
 * Author:
 * ===========
 * Hiroki Omae, Andrew Innes
 * 
 * Lisense:
 * ============
 * This is free and unencumbered software released into the public domain.
 * 
 * Anyone is free to copy, modify, publish, use, compile, sell, or
 * distribute this software, either in source code form or as a compiled
 * binary, for any purpose, commercial or non-commercial, and by any
 * means.
 * 
 * In jurisdictions that recognize copyright laws, the author or authors
 * of this software dedicate any and all copyright interest in the
 * software to the public domain. We make this dedication for the benefit
 * of the public at large and to the detriment of our heirs and
 * successors. We intend this dedication to be an overt act of
 * relinquishment in perpetuity of all present and future rights to this
 * software under copyright law.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 * 
 * For more information, please refer to <http://unlicense.org>
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntangledInt {
	
	private class Manager {
		public LinkedList<EntangledInt> list;	// list of all engangled ints
		public EntangledInt m_ie;				// at least one item must be in list
		public int m_hashOfRegisteredItems;		// last hash of all items

		private static Manager s_manager;

		public static Manager instance {
			get {
				if( s_manager == null ) {
					s_manager = new Manager();
					s_manager.Initialize();
				}
				return s_manager;
			}
		}

		public Manager() {}

		public void Initialize() {
			list = new LinkedList<EntangledInt>();
			m_ie = new EntangledInt(Random.Range (int.MaxValue/2, int.MaxValue));
		}

		public void Register(EntangledInt i) {
			list.AddLast(i);
			UpdateHash();
		}

		public void Unregister(EntangledInt i) {
			list.Remove(i);
			UpdateHash();
		}

		private int _ComputeHashOfAllItems() {
			int result = 65599;
			// Compute SDBM hash of all HackProofInt(s), as if they were contiguous in memory.
			foreach(EntangledInt i in list) {
				byte[] bytes = System.BitConverter.GetBytes( i._value );
				foreach(byte b in bytes) {
					result = b + (result << 6) + (result << 16) - result;
				}
			}
			return result;
		}

		public bool Validate() {
			return m_hashOfRegisteredItems == _ComputeHashOfAllItems();
		}
		public void UpdateHash() {
			m_hashOfRegisteredItems = _ComputeHashOfAllItems();
		}
	}
	
	public int m_value;	// value
	public int m_mask;		// value mask
	

	public int Value {
		get {
			if( !Manager.instance.Validate() ) {
				throw new System.Security.SecurityException("unexpected value change detected");
			}
			return _value;
		}
		set {
			m_value = unchecked(value ^ m_mask);
			Manager.instance.UpdateHash();
		}
	}

	private int _value {
		get {
			return unchecked(m_value ^ m_mask);
		}
	}
	
	public EntangledInt() {
		_Initialize(0);
	}
	
	public EntangledInt(int i) {
		_Initialize(i);
	}
	~EntangledInt() {
		Manager.instance.Unregister(this);
	}
	
	private void _Initialize(int i) {
		m_mask = Random.Range(0, int.MaxValue);
		m_value = i ^ m_mask;
		Manager.instance.Register(this);
	}
}

