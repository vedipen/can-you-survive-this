using UnityEngine;
using System.Collections;

namespace JSS {
	public abstract class Movable : MonoBehaviour {

		private LayerMask blockingLayer;

		private BoxCollider2D boxCollider2D;
		private Rigidbody2D   rigidBody2D;

		// Configurable
		// public float timeToMove;

		public class Collision {
			bool      occurred;
			Transform transform;

			public Collision(bool occurrence, Transform hitTransform) {
				occurred = occurrence;
				transform   = hitTransform;
			}

			public bool hasOccurred() {
				return occurred;
			}

			public Transform getTransform() {
				return transform;
			}
		}

		// Initalizes the Movable's state
		virtual protected void Init(LayerMask blockingLayer) {
			this.blockingLayer = blockingLayer;

			boxCollider2D = GetComponent<BoxCollider2D>();
			rigidBody2D   = GetComponent<Rigidbody2D>();
		}

		// Creates and returns a Collision object with information about if
		// the Movable can move in the specified delta direction
		public Collision GetCollision(Vector2 delta) {
			RaycastHit2D raycastHit;
			Vector2 start = transform.position;
			Vector2 end   = start + delta;

			// "Disable the boxCollider so that linecast doesn't hit this object's own collider."
			boxCollider2D.enabled = false;

			// Cast a line from start point to end point checking collision on blockingLayer.
			raycastHit = Physics2D.Linecast(start, end, blockingLayer); 

			// Re-enable the box collider after linecast
			boxCollider2D.enabled = true;

			// Collision occurred if something was hit
			bool collisionOccurred = (raycastHit.transform != null);

			return new Collision(collisionOccurred, raycastHit.transform);
		}

		// Returns the current Vector2 position of this Movable
		virtual protected Vector2 GetCurrentPosition() {
			return rigidBody2D.transform.position;
		}

		// Moves this Movable from its current position along the specified
		// Vector2 'delta'. Assumes that they can move to that spot.
		virtual public void MoveBy(Vector2 delta) {
			Vector2 fromPos = GetCurrentPosition();
			Vector2 toPos   = fromPos + delta;

			// Start a co-routine to move to the specified location
			StartCoroutine(SmoothMovement(fromPos, toPos));
		}

		// Moves this Movable from its current position along the specified
		// Vector2 'delta'. Assumes that they can move to that spot.
		virtual public IEnumerator MoveByCoroutine(Vector2 delta) {
			Vector2 fromPos = GetCurrentPosition();
			Vector2 toPos   = fromPos + delta;

			// Start a co-routine to move to the specified location
			yield return StartCoroutine(SmoothMovement(fromPos, toPos));
		}

		// Moves this Movable to the specified Vector2 destination
		virtual public void MoveTo(Vector2 destination) {
			Vector2 fromPos = GetCurrentPosition();

			// Start a co-routine to move to the specified location
			StartCoroutine(SmoothMovement(fromPos, destination));
		}

		// Co-routine to move the Movable to the specified Vector2 destination
		protected IEnumerator SmoothMovement(Vector2 fromPos, Vector2 toPos) {
			// TODO: Fix smooth movement. Commenting out this stuff
			// because it's crashing.
			//		- Vidur [7th Dec, 2016]

			// square magnitude is supposed to be computationally cheaper
			// float remainingDistanceSquared = (toPos - fromPos).sqrMagnitude;

			// // while the remaining distance is greater than a very small amount
			// while(remainingDistanceSquared > float.Epsilon) {
			// 	Debug.Log("Smooth moving...");

			// 	// Calculate the next position to move to
			// 	Vector3 newPosition = Vector3.MoveTowards(rigidBody2D.position, toPos, Time.deltaTime / timeToMove);
			// 	Debug.Log("New Position:");
			// 	Debug.Log(newPosition);

			// 	// Move the body
			// 	// rigidBody2D.MovePosition(newPosition);

			// 	// Recalculate remaining distance
			// 	// fromPos = GetCurrentPosition();
			// 	// remainingDistanceSquared = (toPos - fromPos).sqrMagnitude;

			// 	// Return and loop
			// 	yield return null;
			// }

			// Snap to position
			rigidBody2D.MovePosition(toPos);
			yield return null;
		}

		// Use this for initialization
		void Start () {}

		// Update is called once per frame
		void Update () {}
	}
}