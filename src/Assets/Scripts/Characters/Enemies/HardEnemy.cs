using UnityEngine;
using System.Collections;

namespace JSS.Characters.Enemies {
	public class HardEnemy : IEnemy {

		// Creates an HardEnemy from the provided arguments
		public static HardEnemy Create(GameObject enemyObj, LayerMask blockingLayer) {
			HardEnemy enemy = enemyObj.GetComponent<HardEnemy>();
			enemy.Init(blockingLayer);
			return enemy;
		}

		// Initializes an HardEnemy's state
		override protected void Init(LayerMask blockingLayer) {
			base.Init(blockingLayer);
		}

		// Invoked to tell the AI to make a move
		override public IEnumerator Move() {
			// This method invokes IEnemy's MakeRandomMove right now
			yield return StartCoroutine(MakeRandomMove());

			// However, your submission should use this line instead.
			// (Feel free to rename methods as you think necessary)
			// yield return StartCoroutine(TrackPlayer());
		}

		// FOR CANDIDATES
		// --------------
		// TODO: Implement an algorithm where the HardEnemy always moves towards the Player along the
		//       shortest path possible
		//
		//		 Feel free to use `MakeRandomMove` in `IEnemy` as reference as you write this method.
		IEnumerator TrackPlayer() {
			// This line is only here to suppress project warnings
			// Remove it when you implement this method
			yield return null;
		}
	}
}