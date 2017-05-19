

#### GAME DESIGN SPEC

JSS is a turn-based, survival, dungeon-crawler game. On each level, you start at the bottom-left corner of the board and have to find your way to the exit at the top-left corner of the board. Along the way, you will find food or soda, that increases your health. You will also encounter zombie enemies that will attack you, and you can attack back too. Finally, you can also break walls down.

There are two types of enemies. Easy enemies are not quite aware of their surroundings, and hence only attack the player when you come within two tiles of them. Hard enemies will attempt to find you from anywhere on the board.

#### PROBLEM STATEMENT

Your task for this project is to write two separate AIs for the Easy enemies and the hard enemies.

A basic, playable version of the game is already written for you. Once you have Unity installed, you should be able to play the game and move around and complete levels etc.

I have already given both types of enemies a default random behavior AI. You can use that code as a template to write your own AI.

You can find the Enemy files at `Assets/Scripts/Characters/Enemies/`.

**To summarize:**

 1. Write the AI for the Hard Enemy to always move towards the player each turn, and attack him if possible.
 2. Write the AI for the Easy Enemy to only start following the player if the player comes within two tiles of the Easy Enemy
 3. Bonus:
    - Add any other features you think of that will improve the game or how it feels to play it. If you do add any features, please let me know.
    - Fix any bugs you encounter. Again, do let me know if you find and fix any bugs.

**Page 1 of 2**

<span class="break"></span>

#### Unity Installation

I've used Unity version 5.4.0f3, so you will need either that version, or a newer version to run this game.

You can find all the versions of Unity here - [https://unity3d.com/get-unity/download/archive](https://unity3d.com/get-unity/download/archive).

#### EVALUATION CRITERIA

 1. Functionality - First and foremost, the submitted code should work and meet the spec.
 2. Modularity - An ideal solution will be modular and be able to adapt to possible
    changes in spec
 3. Formatting - Code should be formatted and readable. i.e.:
      - Good documentation
      - Good use of whitespace
      - Lines are not too long (80 chars is a good rule of thumb)
      - Variable names are meaningful
      - There are no hardcoded 'magic' numbers
      - There should be no 'dead code' - code that is unused
 4. Attention to detail:
      - Failure conditions and errors are handled
      - Memory is handled properly - anything that is allocated is also disposed

**Page 2 of 2**

<style type="text/css">
@media print {
    .break { display: block; page-break-before: always; }
}
.markdown-body code:before, .markdown-body code:after {
    content: "";
}
</style>
