using UnityEngine;

namespace JSS
{

    using System;
    using System.Collections.Generic;
    using Random = UnityEngine.Random;
    using Characters.Enemies;

    public class Board : MonoBehaviour
    {

        [Serializable]
        public class Range
        {
            public int minimum;
            public int maximum;

            public Range(int min, int max)
            {
                minimum = min;
                maximum = max;
            }

            // Returns a random number within this range (inclusive)
            public int GetRandom()
            {
                return Random.Range(minimum, maximum + 1);
            }
        }

        public int numRows = 8;
        public int numCols = 8;

        public Range numWalls = new Range(5, 9);
        public Range numFood = new Range(1, 5);

        public GameObject exit;
        public GameObject player;

        public GameObject[] floorTiles;
        public GameObject[] foodTiles;
        public GameObject easyEnemyTile;
        public GameObject hardEnemyTile;
        public GameObject[] enemyTiles;
        public GameObject[] wallTiles;
        public GameObject[] outerWallTiles;

        private SoundManager soundManager;

        private LayerMask blockingLayer;

        public static Board Create(GameObject boardObj, SoundManager soundManager, LayerMask blockingLayer)
        {
            Board board = boardObj.GetComponent<Board>();
            board.Init(soundManager, blockingLayer);

            return board;
        }

        void Init(SoundManager soundManager, LayerMask blockingLayer)
        {
            this.soundManager = soundManager;
            this.blockingLayer = blockingLayer;

            enemyTiles = new GameObject[2];
            enemyTiles[0] = easyEnemyTile;
            enemyTiles[1] = hardEnemyTile;

            int leftEdgeIndex = -1;
            int rightEdgeIndex = numRows;

            int bottomEdgeIndex = -1;
            int topEdgeIndex = numCols;

            // Create the basic board
            for (int x = leftEdgeIndex; x <= rightEdgeIndex; x++)
            {
                for (int y = bottomEdgeIndex; y <= topEdgeIndex; y++)
                {
                    GameObject objectTemplate;

                    if (x == leftEdgeIndex || x == rightEdgeIndex || y == bottomEdgeIndex || y == topEdgeIndex)
                    {
                        objectTemplate = GetRandomObjectFromList(outerWallTiles);
                    }
                    else
                    {
                        objectTemplate = GetRandomObjectFromList(floorTiles);
                    }

                    GameObject instance = Instantiate(objectTemplate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(transform);
                }
            }
        }

        GameObject GetRandomObjectFromList(GameObject[] list)
        {
            return list[Random.Range(0, list.Length)];
        }

        // Returns a list of all available grid positions
        List<Vector3> GetAllGridPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            for (int x = 1; x < numCols - 1; x++)
            {
                for (int y = 1; y < numRows - 1; y++)
                {
                    positions.Add(new Vector3(x, y, 0f));
                }
            }

            return positions;
        }

        Vector3 GetRandomItemFromList(ref List<Vector3> list)
        {
            int randomIndex = Random.Range(0, list.Count);
            Vector3 randomItem = list[randomIndex];
            list.RemoveAt(randomIndex);

            return randomItem;
        }

        void LayoutObjectAtRandom(ref List<Vector3> availablePositions, GameObject[] tileArray, Range range)
        {
            int numObjects = range.GetRandom();
            LayoutObjectAtRandom(ref availablePositions, tileArray, numObjects);
        }

        void LayoutObjectAtRandom(ref List<Vector3> availablePositions, GameObject[] tileArray, int numObjects)
        {
            for (int i = 0; i < numObjects; i++)
            {
                Vector3 randomAvailablePosition = GetRandomItemFromList(ref availablePositions);

                GameObject tileChoice = GetRandomObjectFromList(tileArray);

                GameObject obj = Instantiate(tileChoice, randomAvailablePosition, Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);

                // Check if it's a wall, and set it up with more information
                Wall wall = obj.GetComponent<Wall>();
                if (wall)
                {
                    wall.Init(soundManager);
                }

                // Check if it's an easy enemy
                if (tileChoice == easyEnemyTile)
                {
                    EasyEnemy.Create(obj, blockingLayer);

                    // Check if it's a hard enemy
                }
                else if (tileChoice == hardEnemyTile)
                {
                    HardEnemy.Create(obj, blockingLayer);
                }
            }
        }

        // Generate the board for the specified level
        public void Generate(int level)
        {
            Reset();

            List<Vector3> gridPositions = GetAllGridPositions();

            LayoutObjectAtRandom(ref gridPositions, wallTiles, numWalls);
            LayoutObjectAtRandom(ref gridPositions, foodTiles, numFood);

            int numEnemies = (int)Math.Log(level, 2f);
            LayoutObjectAtRandom(ref gridPositions, enemyTiles, numEnemies);

            Vector3 exitPosition = new Vector3(numCols - 1, numRows - 1, 0f);
            GameObject instance = Instantiate(exit, exitPosition, Quaternion.identity) as GameObject;
            instance.transform.SetParent(transform);
        }

        // Reset by removing all the pieces within the basic board
        private void Reset()
        {
            foreach (Transform child in transform)
            {
                if (child.tag != Tags.BasicBoardTile)
                {
                    GameObject obj = child.gameObject;
                    Destroy(obj);
                }
            }
        }

    }
}