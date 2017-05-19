using UnityEngine;

namespace JSS {
	public class Fabricator : MonoBehaviour {
		
		public static GameObject Fabricate(GameObject prefab) {
			return Fabricate(prefab, Vector3.zero, Quaternion.identity);
		}

		public static GameObject Fabricate(GameObject prefab, Transform transform) {
			return Fabricate(prefab, Vector3.zero, Quaternion.identity, transform);
		}

		public static GameObject Fabricate(GameObject prefab, Vector3 position,
										   Quaternion rotation) {
			return Instantiate(prefab, position, rotation) as GameObject;
		}

		public static GameObject Fabricate(GameObject prefab, Vector3 position,
										   Quaternion rotation, Transform transform) {
			return Instantiate(prefab, position, rotation, transform) as GameObject;
		}
	}
}