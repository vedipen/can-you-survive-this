using UnityEngine;
using System.Collections;
using System;

namespace JSS.Characters.Enemies
{

    public class IEnemy : Character
    {

        private Vector2[] possibleDirections;
        private ArrayList directions;
        private Transform target;

        // Initializes an IEnemy's initial state
        override protected void Init(LayerMask blockingLayer)
        {
            possibleDirections = new Vector2[4];
            possibleDirections[0] = new Vector2(1f, 0f);
            possibleDirections[1] = new Vector2(-1f, 0f);
            possibleDirections[2] = new Vector2(0f, -1f);
            possibleDirections[3] = new Vector2(0f, 1f);

            directions = new ArrayList();
            ResetDirectionsList();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            base.Init(blockingLayer);
        }

        override public void takeDamage(int amount)
        {
            base.takeDamage(amount);
            if (getHealth() <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        // Invoked to tell the AI to make a move
        virtual public IEnumerator Move()
        {
            yield return StartCoroutine(MakeRandomMove());
        }

        // Makes a move in a random direction
        protected IEnumerator MakeRandomMove()
        {
            // The direction to move in
            Vector2 direction = Vector2.zero;

            bool hasValidMove = false;

            while (!hasValidMove)
            {
                if (directions.Count == 0)
                {
                    // No possible moves to make
                    break;
                }

                // Pick a random direction
                direction = (Vector2)getRandomItemFromList(directions);

                // Check if it's possible to move in that direction
                Collision collision = GetCollision(direction);
                if (collision.hasOccurred())
                {
                    GameObject obj = collision.getTransform().gameObject;

                    // If collided with Player
                    if (obj.tag == Tags.Player)
                    {
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
                }
                else
                {
                    hasValidMove = true;
                }
            }

            // Only move if a valid move was found
            if (hasValidMove)
            {
                yield return StartCoroutine(MoveByCoroutine(direction));
            }

            // Reset the directions ArrayList
            // for the next move
            ResetDirectionsList();
        }

        // Returns a random item from the specified IList
        private object getRandomItemFromList(IList list)
        {
            // Pick a random item
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            object item = list[randomIndex];

            // Remove it from the list
            list.RemoveAt(randomIndex);

            return item;
        }

        // Resets the ArrayList of directions
        private void ResetDirectionsList()
        {
            directions.Clear();
            foreach (Vector2 direction in possibleDirections)
            {
                directions.Add(direction);
            }
        }

        protected IEnumerator TrackPlayer()
        {
            //Debug.Log("Triggered");
            Vector2 direction = Vector2.zero;
            Vector2 directionAlternative = Vector2.zero;
            bool hasValidMove = false;
            float xDir = 0;
            float yDir = 0;
            float xDirAlternative = 0;
            float yDirAlternative = 0;
            if (Mathf.Abs(target.position.y - transform.position.y) < float.Epsilon)
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
                xDirAlternative = 0;
                yDirAlternative = target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                yDir = target.position.y > transform.position.y ? 1 : -1;
                xDirAlternative = target.position.x > transform.position.x ? 1 : -1;
                yDirAlternative = 0;
            }
            direction = new Vector2(xDir, yDir);
            directionAlternative = new Vector2(xDirAlternative, yDirAlternative);

            // Check if it's possible to move in that direction (original one)
            Collision collision = GetCollision(direction);
            if (collision.hasOccurred())
            {
                GameObject obj = collision.getTransform().gameObject;
                // If collided with Player
                if (obj.tag == Tags.Player)
                {
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
            }
            else
            {
                hasValidMove = true;
            }
            // Only move if a valid move was found (original)
            if (hasValidMove)
            {
                yield return StartCoroutine(MoveByCoroutine(direction));
            }
            else
            {
                //Original move didn't make it due to collisions with hurdles. Now try the alternate path.
                Collision collisionAlternative = GetCollision(directionAlternative);
                if (collisionAlternative.hasOccurred())
                {
                    GameObject obj = collisionAlternative.getTransform().gameObject;
                    // If collided with Player
                    if (obj.tag == Tags.Player)
                    {
                        Player player = obj.GetComponent<Player>();
                        // Play appropriate animation
                        animator.SetTrigger("enemyHit");
                        // Deal damage to them
                        player.takeDamage(getDamage());
                        // This counts as a valid move
                        hasValidMove = true;
                        // but reset directionAlternative to zero,
                        // so the Enemy doesn't move into
                        // the Player's spot
                        directionAlternative = Vector2.zero;
                    }
                }
                else
                {
                    hasValidMove = true;
                }
                if (hasValidMove) //if possible
                {
                    yield return StartCoroutine(MoveByCoroutine(directionAlternative));
                }
                else
                {
                    if (direction.x == 1) direction.x = -1;
                    else if (direction.x == -1) direction.x = 1;
                    if (direction.y == 1) direction.y = -1;
                    else if (direction.y == -1) direction.y = 1;
                    Collision collisionSecondary = GetCollision(direction);
                    if (!collisionSecondary.hasOccurred())
                    {
                        yield return StartCoroutine(MoveByCoroutine(direction));
                    }
                    else
                    {
                        if (directionAlternative.x == 1) directionAlternative.x = -1;
                        else if (directionAlternative.x == -1) directionAlternative.x = 1;
                        if (directionAlternative.y == 1) directionAlternative.y = -1;
                        else if (directionAlternative.y == -1) directionAlternative.y = 1;
                        collisionSecondary = GetCollision(directionAlternative);
                        if (!collisionSecondary.hasOccurred())
                        {
                            yield return StartCoroutine(MoveByCoroutine(directionAlternative));
                        }
                    }
                }
            }
        }

        protected IEnumerator WaitAndTrackPlayer()
        {
            //Debug.Log("Player position : " + target.position.x + " " + target.position.y + " Easy Enemy Position :" + transform.position.x + " " + transform.position.y);
            //Debug.Log("In Range");
            if (Math.Abs(target.position.x - transform.position.x) <= 2 && Math.Abs(target.position.y - transform.position.y) <= 2)
            {
                //Debug.Log("In Range");
                //Debug.Log(Math.Abs(target.position.x - transform.position.x) + " " + Math.Abs(target.position.y - transform.position.y));
                StartCoroutine(TrackPlayer());
            }
            else
            {
                yield return null; //If both the path wont work i.e already standing in best position then do nothing
            }
        }
    }
}
