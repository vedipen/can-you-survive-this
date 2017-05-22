using UnityEngine;
using System.Collections;

namespace JSS.Characters.Enemies
{
    public class EasyEnemy : IEnemy
    {


        // Creates an EasyEnemy from the provided arguments
        public static EasyEnemy Create(GameObject enemyObj, LayerMask blockingLayer)
        {
            EasyEnemy enemy = enemyObj.GetComponent<EasyEnemy>();
            enemy.Init(blockingLayer);
            return enemy;
        }

        // Initializes an EasyEnemy's state
        override protected void Init(LayerMask blockingLayer)
        {
            base.Init(blockingLayer);
        }

        // Invoked to tell the AI to make a move
        override public IEnumerator Move()
        {
            // This method invokes IEnemy's MakeRandomMove right now
            //yield return StartCoroutine(MakeRandomMove());

            // However, your submission should use this line instead.
            // (Feel free to rename methods as you think necessary)
            yield return StartCoroutine(WaitAndTrackPlayer());
        }

        // FOR CANDIDATES
        // --------------
        // TODO: Implement an algorithm where the EasyEnemy waits for the Player to come within
        //       a certain configurable tile distance of the Enemy before he starts following
        //	     the Player to attack him.
        //
        //		 Feel free to use `MakeRandomMove` in `IEnemy` as reference as you write this method.

    }
}