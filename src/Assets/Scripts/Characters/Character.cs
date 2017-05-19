using UnityEngine;

namespace JSS.Characters {
	public class Character : Movable {

		// The damage that the Character starts with
		public int initialDamage = 1;

		// The Character's actual damage at any given time
		protected int damage;

		// The health that the Character starts with
		public int initalHealth = 5;

		// The Character's actual health at any given time
		protected int health;

		// A reference to the Character's Animator
		protected Animator animator;

		// Initializes a Character's initial state
		override protected void Init(LayerMask blockingLayer) {
			damage = initialDamage;
			health = initalHealth;

			animator = GetComponent<Animator>();

			base.Init(blockingLayer);
		}

		// Returns the amount of health that
		// this Character has
		public int getHealth() {
			return health;
		}

		// Sets this Character's health to the
		// specified amount
		protected void setHealth(int amount) {
			health = amount;
		}

		// Increases this Character's health by the
		// specified amount
		protected void increaseHealthBy(int amount) {
			if(amount > 0) {
				setHealth(getHealth() + amount);
			}
		}

		// Returns the amount of damage that
		// this Character has
		protected int getDamage() {
			return damage;
		}

		// Returns true if this Character's health
		// is at or below zero
		protected bool hasDied() {
			return getHealth() <= 0;
		}

		// The Character takes the specified amount of damage
		// and updates its state accordingly
		virtual public void takeDamage(int amount) {
			int health = getHealth();

			if(amount > 0 && health > 0) {
				setHealth(health - amount);

				// If the Character has lost all its health
				if(getHealth() <= 0) {
					// Disable the game object
					gameObject.SetActive(false);
				}
			}
		}
	}
}