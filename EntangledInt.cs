/*
 * EntangledInt:
 * written by Hiroki Omae(@pigeon6), Andrew Innes
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntangledInt {
	
	private class Manager {
		public LinkedList<EntangledInt> list;	// list of all engangled ints
		public EntangledInt m_ie;				// at least one item must be in list
		public int m_hashOfRegisteredItems;	// last hash of all items

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
			int result = 6559;			
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

//#include "stdafx.h"
//#include <random>

//class HackProofInt
//{
//public:
//		
//		HackProofInt()
//	{
//		Register(this);
//		Initialise(0);
//	}
//	
//	HackProofInt(int initialValue)
//	{
//		Register(this);
//		Initialise(initialValue);
//	}
//	
//	~HackProofInt()
//	{
//		Deregister(this);
//	}
//	
//	void operator = (int newValue)
//	{
//		Initialise(newValue);
//	}
//	
//	// TODO: Add the various arithmetic operators so we use these more conveniently.
//	
//	int GetValue() const
//	{
//		if (hashOfRegisteredItems == ComputeHashOfAllItems())
//		{
//			return m_value ^ m_mask;
//		}
//		
//		throw new std::exception("Hack detected");
//	}
//	
//private:
//		
//		static void Register(HackProofInt* that)
//	{
//		registeredItems.push_back(that);
//	}
//	
//	static void Deregister(HackProofInt* that)
//	{
//		std::vector<HackProofInt*>::iterator match = std::find(registeredItems.begin(), registeredItems.end(), that);
//		if (match != registeredItems.end())
//		{
//			registeredItems.erase(match);
//		}
//	}
//	
//	void Initialise(int initialValue)
//	{
//		// An additional item used to entangle the hash of any item with the hash of at least one other.
//		static HackProofInt entanglement;
//		
//		m_mask = rand(); // TODO: Use a random generator which fills all the bits.
//		m_value = initialValue ^ m_mask;
//		
//		// This ensures the old hash is no longer valid.
//		if (this != &entanglement)
//		{
//			entanglement = rand();
//		}
//		
//		hashOfRegisteredItems = ComputeHashOfAllItems();
//	}
//	
//	static int ComputeHashOfAllItems()
//	{
//		int result = 6559;
//		
//		// Compute SDBM hash of all HackProofInt(s), as if they were contiguous in memory.
//		std::vector<HackProofInt*>::const_iterator iter = registeredItems.begin();
//		while (iter != registeredItems.end())
//		{
//			HackProofInt* that = *iter++;
//			const unsigned char* readPtr = reinterpret_cast<const unsigned char*>(that);
//			int loop = sizeof(HackProofInt);
//			while (loop--)
//			{
//				unsigned char c = *readPtr++;
//				result = c + (result << 6) + (result << 16) - result;
//			}
//		}
//		
//		return result;
//	}
//	
//	// The obfuscated value.
//	int m_value;
//	
//	// XOR bitmask used to (de)obfuscate value.
//	int m_mask;
//	
//	// The address of all fully constructed items.
//	static std::vector<HackProofInt*> registeredItems;
//	
//	// Hash value of all fully constructed items.
//	static int hashOfRegisteredItems;
//	
//	friend class Hacker;
//};
//
//std::vector<HackProofInt*> HackProofInt::registeredItems;
//
//int HackProofInt::hashOfRegisteredItems;
//
//class Hacker
//{
//public:
//		
//		static int GetHash()
//	{
//		return HackProofInt::hashOfRegisteredItems;
//	}
//	
//	static void SetHash(int newValue)
//	{
//		HackProofInt::hashOfRegisteredItems = newValue;
//	}
//};
//
//int _tmain(int argc, _TCHAR* argv[])
//{
//	HackProofInt goldCoins(1234);
//	
//	_ASSERT(goldCoins.GetValue() == 1234);
//	
//	goldCoins = 999;
//	_ASSERT(goldCoins.GetValue() == 999);
//	
//	{
//		// Jot down the in-memory state, and also the hash.
//		unsigned char oldState[sizeof(goldCoins)];
//		memcpy(&oldState, &goldCoins, sizeof(oldState));
//		int oldHash = Hacker::GetHash();
//		
//		// Spend the gold coins.
//		goldCoins = 345;
//		
//		// Hack the previous in-memory state back into place, and restore the hash.
//		memcpy(&goldCoins, &oldState, sizeof(oldState));
//		Hacker::SetHash(oldHash);
//		
//		// This throws an exception, because the hack is detected.
//		int value = goldCoins.GetValue();
//	}
//	
//	return 0;
//}
