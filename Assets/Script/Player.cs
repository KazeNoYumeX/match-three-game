using UnityEditor;

namespace Script
{
	public class Player
	{
		private int _score;
		
		public Player(int score = 0){
			_score = score;
		}
		
		public int getScore(){
			return _score;
		}
		
		public void addScore(int score){
			_score += score;
		}
	}
}
