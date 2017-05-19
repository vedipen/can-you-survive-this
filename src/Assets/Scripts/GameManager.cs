using UnityEngine;
using UnityEngine.UI;

namespace JSS {
	using System.Collections;
	using Characters;
	using Characters.Enemies;

    public class GameManager : MonoBehaviour {

		public static GameManager instance = null;

		private enum State {
			INITIALIZING,
			INITIALIZED,
			RUNNING,
			PAUSED,
			LEVEL_ENDED,
			GAME_OVER
		};

		// Tracks the game's current state
		private State gameState;

		// Prefabs
		public GameObject boardPrefab;
		public GameObject soundManagerPrefab;
		public GameObject playerPrefab;

		// Variables
		public LayerMask blockingLayer;

		// All the times
		public float levelTransitionTime = 2.0f;
		public float afterDeathWaitTime  = 2.0f;
		public float splashScreenTime    = 3.0f;
		public float restartGameTime 	 = 4.0f;
		public float turnTime 			 = 0.8f;

		private Board board;
		private int level = 1;

		private GameObject boardHolder;

		// UI Elements
		private Text healthText;
		private string healthTextPrefix;

		GameObject levelTransitionRoot;
		Text   levelText;
		string levelTextPrefix;

		private SoundManager soundManager;
		private Player       player;

		// Invoked before Start
		void Awake() {
			// Enforce singleton pattern
			if(instance == null) {
				instance = this;
			} else if(instance != this) {
				Destroy(gameObject);
			}

			// Don't destroy this instance when the scene is reloaded
			DontDestroyOnLoad(gameObject);

			StartCoroutine(IntroAndStartGame());
		}

		// Shows the splash screen and then starts the game
		IEnumerator IntroAndStartGame() {
			yield return StartCoroutine(DisplaySplashScreen());
			yield return StartCoroutine(InitAndStartGameCoroutine());
		}

		// Displays splash screen
		IEnumerator DisplaySplashScreen() {

			GameObject splash = null;
			GameObject canvas = GameObject.Find("Canvas");

			// TODO: Figure out why GameObject.Find("SplashScreen")
			//		 doesn't always work here. This is a gross
			// 		 workaround, till we figure that out.
			foreach(Transform child in canvas.transform) {
				if(child.name == "SplashScreen") {
					splash = child.gameObject;
				}
			}

			// Show splash screen
			splash.SetActive(true);

			// Wait
			yield return new WaitForSeconds(splashScreenTime);

			// Hide splash screen
			splash.SetActive(false);
		}

		// A method to wrap these two functions that are invoked together often
		void InitAndStartGame() {
			InitializeGame();
			StartCoroutine(StartGame());
		}

		IEnumerator InitAndStartGameCoroutine() {
			InitAndStartGame();
			yield return null;
		}

		void InitializeGame() {
			gameState = State.INITIALIZING;

			if(boardHolder == null) {
				boardHolder = new GameObject("BoardHolder");
			}

			// Sound Manager
			if(soundManager == null) {
				soundManager = SoundManager.Create(Fabricator.Fabricate(soundManagerPrefab));
			}

			// Board
			if(board == null) {
				board = Board.Create(Fabricator.Fabricate(boardPrefab, boardHolder.transform), soundManager, blockingLayer);
			}
			board.Generate(level);

			// Player
			if(player == null) {
				player = Player.Create(Fabricator.Fabricate(playerPrefab), blockingLayer);

				// Subscribe to the Player's OnReachedExitEvent
				player.OnReachedExitEvent += GoToNextLevel;

				// Subscribe to changes to Player's health
				player.OnHealthUpdatedEvent += UpdateHealthText;

				// Subscribe to the Player's death event
				player.OnDeathEvent += OnPlayerDeath;
			}
			player.Reset();

			// UI
			if(healthText == null) {
				GameObject healthTextObj = GameObject.Find("HealthText");
				healthText       = healthTextObj.GetComponent<Text>();
				healthTextPrefix = healthTextObj.GetComponent<UI.PrefixedText>().prefix;

				// Find display objects
				levelTransitionRoot     = GameObject.Find("LevelTransitionRoot");
				GameObject levelTextObj = GameObject.Find("LevelText");

				// Get the text obj and figure out what the prefix should be
				levelText       = levelTextObj.GetComponent<Text>();
				levelTextPrefix = levelTextObj.GetComponent<UI.PrefixedText>().prefix;
			}

			UpdateHealthText(player.getHealth());

			// Mark the game as initialized
			gameState = State.INITIALIZED;
		}

		// Creates the board and starts the game
		IEnumerator StartGame() {

			// Show transition for a specific level
			yield return StartCoroutine(DisplayLevelTransition(level));

			// Set state to RUNNING
			gameState = State.RUNNING;

			while(gameIsRunning()) {

				// Constantly get input
				InputHandler.Direction input = InputHandler.GetInput();
				Vector2 deltaVector = Adapters.DirectionToVectorAdapter.getVectorFor(input);

				if(player.hasNextTurn()) {

					// Only move if there was input
					if(deltaVector != Vector2.zero) {
						player.MoveBy(deltaVector);
					}

				} else if(player.isWaitingForTurn()) {
					yield return StartCoroutine(ExecEnemiesAI());
					yield return new WaitForSeconds(turnTime);
					// TODO: Move all the Enemies here

					// Back to Player's turn
					player.setNextTurn();
				}

				yield return null;
			}
		}

		// Iterates through the list of Enemies and
		// makes a move for each of them
		IEnumerator ExecEnemiesAI() {
			GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag(Tags.Enemy);

			foreach(GameObject enemyObj in enemyObjs) {
				IEnemy enemy = enemyObj.GetComponent<IEnemy>();
				yield return StartCoroutine(enemy.Move());
				yield return new WaitForSeconds(turnTime);
			}
		}

		IEnumerator DisplayLevelTransition(int level) {
			if(!gameIsInitialized()) {
				throw new System.Exception("Attempted to display level transition when game was not INITIALIZING!");
			}

			// Set the text with the correct level
			levelText.text = levelTextPrefix + level;

			// Display the transition
			levelTransitionRoot.SetActive(true);

			// Wait for the configured amount of time
			yield return new WaitForSeconds(levelTransitionTime);

			// Hide the level transition
			levelTransitionRoot.SetActive(false);

			// TODO: Figure out why we the wait above triggers
			//       the box collider for the Player on the Exit
			// Keep this re-reset here till that's sorted out
			player.Reset();
		}

		// Displays the 'Game Over' screen
		void DisplayGameOverScreen() {
			// Set the text
			levelText.text = "Game Over";

			// Show the screen
			levelTransitionRoot.SetActive(true);
		}

		// Hides the 'Game Over' screen
		void HideGameOverScreen() {
			// Hide the screen
			levelTransitionRoot.SetActive(false);
		}

		// Updates the Health Text label
		void UpdateHealthText(int healthValue) {
			healthText.text = healthTextPrefix + healthValue;
		}

		// Returns true if the game is currently INITIALIZING
		bool gameIsInitializing() {
			return gameState == State.INITIALIZING;
		}

		// Returns true if the game is currently INITIALIZED
		bool gameIsInitialized() {
			return gameState == State.INITIALIZED;
		}

		// Returns true if the game is currently RUNNING
		bool gameIsRunning() {
			return gameState == State.RUNNING;
		}

		// Returns true if the level has ended
		bool levelHasEnded() {
			return gameState == State.LEVEL_ENDED;
		}

		// Returns true if the game is over
		bool gameIsOver() {
			return gameState == State.GAME_OVER;
		}

		// Loads the next level of the game
		void GoToNextLevel() {
			// If the game is not running any more,
			// don't proceed to the next level
			if(!gameIsRunning()) {
				return;
			}

			// Change state to LEVEL_ENDED
			gameState = State.LEVEL_ENDED;

			// Increment level
			level++;

			// Restart the level
			RestartLevel();
		}

		// Resets and then restarts the level, only if it
		// has ended already
		void RestartLevel() {
			// Re-initalize game if it has ended, otherwise this
			// method is being invoked along with `Awake` so we
			// don't want to init the game twice
			if(levelHasEnded()) {
				InitAndStartGame();
			}
		}

		// Disposes off any GameObjects that is
		// owned by the current game
		void DisposeMembers() {
			GameObject.Destroy(boardHolder);
			boardHolder = null;

			GameObject.Destroy(soundManager);
			soundManager = null;

			GameObject.Destroy(board);
			board = null;

			GameObject.Destroy(player);
			player = null;

			// Reset level
			level = 1;
		}

		// Invoked when the Player dies
		void OnPlayerDeath() {
			gameState = State.GAME_OVER;
			StartCoroutine(OnPlayerDeathCoroutine());
		}

		IEnumerator OnPlayerDeathCoroutine() {
			// Wait for bit first to let the death sink in
			yield return new WaitForSeconds(afterDeathWaitTime);

			// Proceed with the game restart
			yield return StartCoroutine(RestartGame());
		}

		// Resets and restarts the game, only
		// if the game is over
		IEnumerator RestartGame() {
			if(gameIsOver()) {
				// Display the game over screen (and hide the board)
				DisplayGameOverScreen();

				// Dispose any objects that this game owns
				DisposeMembers();

				// Re-initialize the game
				InitializeGame();

				yield return new WaitForSeconds(restartGameTime);

				// Hide the game over screen
				HideGameOverScreen();

				// Show the splash screen again
				yield return StartCoroutine(DisplaySplashScreen());

				// Start the game up again
				StartCoroutine(StartGame());
			}
		}
	}
} // End of JSS namespace