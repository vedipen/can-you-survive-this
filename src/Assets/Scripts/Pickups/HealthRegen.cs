using UnityEngine;

namespace PickUps {
	public class HealthRegen : MonoBehaviour {

		// The configured inital amount of health regen
		public int initialHealthRegenVal = 1;

		// The actual health regen amount at any given time
		private int healthRegenAmount;

		// Invoked exactly once when initialized
		virtual protected void Awake() {
			healthRegenAmount = initialHealthRegenVal;
		}

		// Returns the amount of health that this item
		// regenerates
		public int GetRegenAmount() {
			return healthRegenAmount;
		}
	}
}