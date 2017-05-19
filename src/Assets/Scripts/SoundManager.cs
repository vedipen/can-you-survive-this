using System;
using UnityEngine;

namespace JSS {
	public class SoundManager : MonoBehaviour {

		public static SoundManager Create(GameObject soundManagerObj) {
			SoundManager soundManager = soundManagerObj.GetComponent<SoundManager>();
			// todo init stuff
			return soundManager;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		// Plays a randomly chosen clip from the provided list of clips
		public void PlayRandomSFXClip(AudioClip[] sfxClips) {
			// TODO: Implement
		}

    }
}