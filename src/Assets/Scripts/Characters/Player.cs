using UnityEngine;

namespace JSS.Characters
{
    using System.Collections;
    using Enemies;

    public class Player : Character
    {

        // A Player's execution states
        private enum State
        {
            WAITING_FOR_TURN,
            HAS_NEXT_TURN,
            TURN_IN_PROGRESS,
            FOUND_EXIT
        }
        private State initialState = State.HAS_NEXT_TURN;
        private State currentState;

        // The Player's initial start position on every level
        public Vector2 initialPosition = Vector2.zero;

        // A delegate that's invoked when the Player
        // reaches the level's exit
        public delegate void OnReachedExitEventHandler();
        public event OnReachedExitEventHandler OnReachedExitEvent;

        // A delegate that's invoked when the Player's
        // health is updated
        public delegate void OnHealthUpdatedEventHandler(int healthValue);
        public event OnHealthUpdatedEventHandler OnHealthUpdatedEvent;

        // A delegate that's invoked when the Player dies
        public delegate void OnDeathEventHandler();
        public event OnDeathEventHandler OnDeathEvent;

        // Creates a Player from the provided arguments
        public static Player Create(GameObject playerObj, LayerMask blockingLayer)
        {
            Player player = playerObj.GetComponent<Player>();
            player.Init(blockingLayer);
            DontDestroyOnLoad(playerObj);
            return player;
        }

        // Initializes a Player's initial state
        protected override void Init(LayerMask blockingLayer)
        {
            // Set initial State
            Reset();

            // Invoke parent's init
            base.Init(blockingLayer);
        }

        // This is a rudimentary implementation of a finite state machine
        //
        // This method safeguards transitions from one state to another,
        // throwing a System.Exception if a transition isn't allowed
        private void transitionTo(State nextState)
        {
            bool isValidTransition = false;

            // WAITING_FOR_TURN => HAS_NEXT_TURN
            if (currentState == State.WAITING_FOR_TURN && nextState == State.HAS_NEXT_TURN)
            {
                isValidTransition = true;

                // HAS_NEXT_TURN => TURN_IN_PROGRESS
            }
            else if (currentState == State.HAS_NEXT_TURN && nextState == State.TURN_IN_PROGRESS)
            {
                isValidTransition = true;

                // TURN_IN_PROGRESS => WAITING_FOR_TURN
            }
            else if (currentState == State.TURN_IN_PROGRESS && nextState == State.WAITING_FOR_TURN)
            {
                isValidTransition = true;

                // TURN_IN_PROGRESS => FOUND_EXIT
            }
            else if (currentState == State.TURN_IN_PROGRESS && nextState == State.FOUND_EXIT)
            {
                //Check if any enemy is still alive
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag(Tags.Enemy);
                if (enemyObjs.Length == 0)
                {
                    isValidTransition = true;
                } else
                {
                    isValidTransition = false;
                }
                // WAITING_FOR_TURN => FOUND_EXIT
            }
            else if (currentState == State.WAITING_FOR_TURN && nextState == State.FOUND_EXIT)
            {
                //Check if any enemy is still alive
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag(Tags.Enemy);
                if (enemyObjs.Length == 0)
                {
                    isValidTransition = true;
                }
                else
                {
                    isValidTransition = false;
                }

                // HAS_NEXT_TURN => FOUND_EXIT
            }
            else if (currentState == State.HAS_NEXT_TURN && nextState == State.FOUND_EXIT)
            {
                //Check if any enemy is still alive
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag(Tags.Enemy);
                if (enemyObjs.Length == 0)
                {
                    isValidTransition = true;
                }
                else
                {
                    isValidTransition = false;
                }
            }

            // Only change state if the transition is valid
            if (isValidTransition)
            {
                currentState = nextState;

                // otherwise, throw an exception
            }
            else
            {
                //throw new System.Exception("Invalid transition attempt!");
                //Debug.Log("Enemies cleared?");
            }
        }

        // Returns true if it is the player's turn
        public bool hasNextTurn()
        {
            return (currentState == State.HAS_NEXT_TURN);
        }

        // Sets the Player's turn to true
        public void setNextTurn()
        {
            transitionTo(State.HAS_NEXT_TURN);
        }

        // Returns true if the Player's turn is currently in progress
        public bool isTurnInProgress()
        {
            return (currentState == State.TURN_IN_PROGRESS);
        }

        // Returns true if the Player is waiting for their turn
        public bool isWaitingForTurn()
        {
            return (currentState == State.WAITING_FOR_TURN);
        }

        // Returns true if the Player has found the exit
        public bool hasFoundTheExit()
        {
            return currentState == State.FOUND_EXIT;
        }

        // Moves this Movable from its current position along the specified
        // Vector2 'delta', if possible
        override public void MoveBy(Vector2 delta)
        {

            // Can't move if the Player has found the exit already
            if (hasFoundTheExit())
            {
                return;
            }

            Collision collision = GetCollision(delta);

            bool shouldMove = true;

            if (collision.hasOccurred())
            {
                handleCollision(collision);
                shouldMove = false;
            }

            transitionTo(State.TURN_IN_PROGRESS);

            // Move
            if (shouldMove)
            {
                StartCoroutine(MoveAndWait(delta));
            }
            else
            {
                // Turn is over, go back to waiting immediately
                transitionTo(State.WAITING_FOR_TURN);
            }
        }

        // A coroutine that moves the player by the specified delta Vector2,
        // and then changes the Player's state after it's done moving
        IEnumerator MoveAndWait(Vector2 delta)
        {
            Vector2 fromPos = GetCurrentPosition();
            Vector2 toPos = fromPos + delta;

            yield return StartCoroutine(SmoothMovement(fromPos, toPos));

            // If this last movement didn't cause the Player to find
            // the Exit, transition back to waiting for their turn
            if (!hasFoundTheExit() && isTurnInProgress())
            {
                transitionTo(State.WAITING_FOR_TURN);
            }
        }

        // Figures out and executes the necessary behavior for the specified Collision
        private void handleCollision(Collision collision)
        {
            Transform hitTransform = collision.getTransform();

            // Look for various components that the Player
            // may have collided with
            Wall wall = hitTransform.GetComponent<Wall>();
            IEnemy enemy = hitTransform.GetComponent<IEnemy>();

            // Check if the collision was with a Wall
            if (wall)
            {
                // Play chop animation
                animator.SetTrigger("playerChop");

                // Damage the wall
                wall.TakeDamage(getDamage());

                // Check if the collision was with an Enemy
            }
            else if (enemy)
            {
                // Play attack animation
                animator.SetTrigger("playerHit");

                // Damage the Enemy
                enemy.takeDamage(getDamage());
            }
        }

        // The Player regenerates the specified amount of health
        private void regenHealth(int amount)
        {
            increaseHealthBy(amount);

            // Notify subscribers of change in health
            if (OnHealthUpdatedEvent != null)
            {
                OnHealthUpdatedEvent(getHealth());
            }
        }

        // The Player takes the specified amount of damage
        // and updates their state accordingly
        override public void takeDamage(int amount)
        {
            base.takeDamage(amount);

            // Notify subscribers of change in health
            if (OnHealthUpdatedEvent != null)
            {
                OnHealthUpdatedEvent(getHealth());
            }

            if (hasDied())
            {
                // Notify subscribers of death
                if (OnDeathEvent != null)
                {
                    OnDeathEvent();
                }
            }
        }

        public Vector2 CurrentPosition()
        {
            return GetCurrentPosition();
        }

        // 'OnTriggerEnter2D' is invoked when another object enters a trigger collider
        // attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D otherCollider)
        {

            // Can't handle collisions if the Player has found the exit already
            if (hasFoundTheExit())
            {
                return;
            }

            // Get a reference to the other collider's game object
            GameObject other = otherCollider.gameObject;

            // Check if these components exist
            PickUps.HealthRegen healthRegenItem = other.GetComponent<PickUps.HealthRegen>();

            // If the Player has reached the Exit
            if (other.tag == Tags.Exit)
            {
                // Update state
                transitionTo(State.FOUND_EXIT);

                // If we have any subscribers to the
                // OnReachedExitEvent, invoke them
                if (OnReachedExitEvent != null)
                {
                    OnReachedExitEvent();
                }

                // If the Player has found a health regen item
            }
            else if (healthRegenItem != null)
            {

                // regenerate health by the amount that
                // the item provides
                regenHealth(healthRegenItem.GetRegenAmount());

                // Remove the health regen object
                other.gameObject.SetActive(false);
            }
        }

        // Resets the Player to their initial position, and
        // updates their current state
        public void Reset()
        {
            if (hasFoundTheExit())
            {
                MoveTo(initialPosition);
            }
            currentState = initialState;
        }
    }
}