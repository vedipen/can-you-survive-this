using UnityEngine;

namespace JSS {
	public class Loader : MonoBehaviour {

		public GameObject gameManager;

		// Awake is invoked before Start
		void Awake() {
			// Create an instance of the game manager if one doesn't exist
			if(GameManager.instance == null) {
				Instantiate(gameManager);
			}
		}
	}
}