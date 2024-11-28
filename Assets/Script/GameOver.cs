using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    //  剩餘時間
    private int gameTime = 0;

    private MatchUnityGame _matchUnityGameComponent;
    // 分數
    public Text scoreText;
    // 時間
    public Text timeText;
    // 最終分數
    public Text finelScoreText;

    public GameObject OverPanel;

    public AnimationClip overAnimation;

    public AnimationClip TextAnimation;

    public bool isGameOver = false;

    private void OnGUI()
    {
        if(_matchUnityGameComponent != null)
        {
            int score = _matchUnityGameComponent.getScore();
            scoreText.text = score + "";
        }
        
        timeText.text = gameTime + "";
    }

    public void initGameOver(int gametime , MatchUnityGame matchUnityGame) {
        gameTime = gametime;
        _matchUnityGameComponent = matchUnityGame;


        if (matchUnityGame.mode == MatchGameMode.TimeMode) {
            StartCoroutine(timeConteller());
        }
        else
        {
            // disable time text
            timeText.gameObject.SetActive(false);
            
            // get time label and disable it
            GameObject.Find("Time").SetActive(false);
        }
        
    }

    private IEnumerator timeConteller() {
        while (gameTime > 0) {
            if (_matchUnityGameComponent.getSystemUI.IsGame) {
                gameTime -= 1;
                if (gameTime <= 0) {
                    gameOver();
                    break;
                }
            }
            yield return new WaitForSeconds(1); 
        }
    }

    public void AddScore(int s) {
        _matchUnityGameComponent.addScore(s);
        scoreText.GetComponent<Animator>().Play(TextAnimation.name);
    }

    public void AddTime(int t) {
        gameTime += t;
        timeText.GetComponent<Animator>().Play(TextAnimation.name);
    }
    
    public void gameOver() {
        isGameOver = true;
        int score = _matchUnityGameComponent.getScore();
        finelScoreText.text = score + "";
        OverPanel.SetActive(true);
        OverPanel.GetComponent<Animator>().Play(overAnimation.name);
    }
}
