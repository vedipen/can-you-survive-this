using UnityEngine;
using System.Collections.Generic; 

namespace JSS {
	namespace Adapters {
		using Direction = InputHandler.Direction;

		public class DirectionToVectorAdapter {

			private static Dictionary<Direction, Vector2> directionMap = new Dictionary<Direction, Vector2> {
				{ Direction.None,       Vector2.zero	   },
				{ Direction.SwipeUp,    new Vector2(0, 1)  },
				{ Direction.SwipeDown,  new Vector2(0, -1) },
				{ Direction.SwipeLeft,  new Vector2(-1, 0) },
				{ Direction.SwipeRight, new Vector2(1, 0)  }
			};

			public static Vector2 getVectorFor(Direction direction) {
				return directionMap[direction];
			}
		}
	}
}