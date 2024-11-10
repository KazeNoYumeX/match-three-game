using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    //  剩餘時間
    private int gameTime = 0;
    
    private Player player;

    private Grid gridComponent;
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
        if(player != null)
        {
            int score = player.getScore();
            scoreText.text = score + "";
        }
        
        timeText.text = gameTime + "";
    }

    public void initGameOver(int gametime , Grid grid, Player player) {
        this.player = player;
        gameTime = gametime;
        gridComponent = grid;
        StartCoroutine(timeConteller());
    }

    private IEnumerator timeConteller() {
        while (gameTime > 0) {
            if (gridComponent.getSystemUI.IsGame) {
                gameTime -= 1;
                if (gameTime <= 0) {
                    gameOver();
                    break;
                }
            }
            yield return new WaitForSeconds(1); 
        }
    }

    public void addScore(int s) {
        player.addScore(s);
        scoreText.GetComponent<Animator>().Play(TextAnimation.name);
    }

    public void addTime(int t) {
        gameTime += t;
        timeText.GetComponent<Animator>().Play(TextAnimation.name);
    }
    public void gameOver() {
        isGameOver = true;
        int score = player.getScore();
        finelScoreText.text = score + "";
        OverPanel.SetActive(true);
        OverPanel.GetComponent<Animator>().Play(overAnimation.name);
    }
}
