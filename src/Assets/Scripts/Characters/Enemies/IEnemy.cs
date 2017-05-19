using UnityEngine;
using System.Collections;

namespace JSS.Characters.Enemies {
	public class IEnemy : Character {

		private Vector2[] possibleDirections;
		private ArrayList directions;

		// Initializes an IEnemy's initial state
		override protected void Init(LayerMask blockingLayer) {
			possibleDirections = new Vector2[4];
			possibleDirections[0] = new Vector2(1f, 0f);
			possibleDirections[1] = new Vector2(-1f, 0f);
			possibleDirections[2] = new Vector2(0f, -1f);
			possibleDirections[3] = new Vector2(0f, 1f);

			directions = new ArrayList();
			ResetDirectionsList();

			base.Init(blockingLayer);
		}

		override public void takeDamage(int amount) {
			base.takeDamage(amount);
			if( getHealth() <= 0 ) {
				gameObject.SetActive(false);
			}
		}

		// Invoked to tell the AI to make a move
		virtual public IEnumerator Move() {
			yield return StartCoroutine(MakeRandomMove());
		}

		// Makes a move in a random direction
		protected IEnumerator MakeRandomMove() {
			// The direction to move in
			Vector2 direction = Vector2.zero;

			bool hasValidMove = false;

			while(!hasValidMove) {
				if(directions.Count == 0) {
					// No possible moves to make
					break;
				}

				// Pick a random direction
				direction = (Vector2) getRandomItemFromList(directions);

				// Check if it's possible to move in that direction
				Collision collision = GetCollision(direction);
				if(collision.hasOccurred()) {
					GameObject obj = collision.getTransform().gameObject;

					// If collided with Player
					if(obj.tag == Tags.Player) {
						Player player = obj.GetComponent<Player>();

						// Play appropriate animation
						animator.SetTrigger("enemyHit");

						// Deal damage to them
						player.takeDamage(getDamage());

						// This counts as a valid move
						hasValidMove = true;

						// but reset direction to zero,
						// so the Enemy doesn't move into
						// the Player's spot
						direction = Vector2.zero;
					}
				} else {
					hasValidMove = true;
				}
			}

			// Only move if a valid move was found
			if(hasValidMove) {
				yield return StartCoroutine(MoveByCoroutine(direction));
			}

			// Reset the directions ArrayList
			// for the next move
			ResetDirectionsList();
		}

		// Returns a random item from the specified IList
		private object getRandomItemFromList(IList list) {
			// Pick a random item
			int randomIndex = Random.Range(0, list.Count);
			object item = list[randomIndex];

			// Remove it from the list
			list.RemoveAt(randomIndex);

			return item;
		}

		// Resets the ArrayList of directions
		private void ResetDirectionsList() {
			directions.Clear();
			foreach(Vector2 direction in possibleDirections) {
				directions.Add(direction);
			}
		}
	}
}