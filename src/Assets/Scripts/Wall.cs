using UnityEngine;

namespace JSS {
	public class Wall : MonoBehaviour {

		private SoundManager soundManager;
		public  AudioClip[]  sfxClips;

		public  int maxHitPoints = 2;
		private int hitPoints;

		private SpriteRenderer spriteRenderer;
		public  Sprite damagedWallSprite;

		public void Init(SoundManager soundManager) {
			this.soundManager   = soundManager;
			this.spriteRenderer = GetComponent<SpriteRenderer>();
			this.hitPoints      = maxHitPoints;
		}

		// The wall takes the specified amount of damage and updates
		// its sprite accordingly
		public void TakeDamage(int amount) {
			if(amount > 0 && hitPoints > 0) {
				spriteRenderer.sprite = damagedWallSprite;
				hitPoints -= amount;

				// Play a random sfx clip from the ones available
				soundManager.PlayRandomSFXClip(sfxClips);

				// If the wall has lost all its hitpoints
				if(hitPoints <= 0) {
					// Disable the game object
					gameObject.SetActive(false);
				}
			}
		}
	}
}