using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemUI : MonoBehaviour {
    // 暫停面板
    public GameObject stopPanel;
    // 暫停面板盡入動畫
    public AnimationClip stopPanelStartAnimation;
    // 暫停面板退出動畫
    public AnimationClip stopPanelStopAnimation;
    // 暫停
    private bool isGame = true;

    public GameObject scorePanel;

    private Animator scoreAnimator;

    public AnimationClip scoreAnimition;

    private SpriteRenderer spriteCompent;

    private IEnumerator PlayScoreAnimition;

    private GameOver gameOver;

    public bool IsGame {
        get { return isGame; }
    }

    private void Awake()
    {
        if (scorePanel != null) {
            gameOver = GetComponent<GameOver>();
            scoreAnimator = scorePanel.GetComponent<Animator>();
            spriteCompent = scorePanel.transform.Find("Score").GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stopPanel == null || gameOver.isGameOver) {
                Application.Quit();
            }
            if (isGame)
            {
                isGame = false;
                stopGame();
            }
            else {
                Application.Quit();
            }
        }
    }

    //退出遊戲
    public void exitGame() {
        Application.Quit();
    }

    //場景跳转
    public void scenceLoad(int i) {
        SceneManager.LoadScene(i);
    }

    public void scenceLoadToGame() {
        SceneManager.LoadScene("Game");
    }

    // 暫停遊戲
    public void stopGame() {
        StartCoroutine(StopGame());
    }

    // 取消暫停
    public void backGame() {
        StartCoroutine(BackGame());
    }

    // 播放獎勵動畫
    public void PlayScore(Sprite scoreSprite) {
        scorePanel.SetActive(false);
        if (PlayScoreAnimition != null) {
            StopCoroutine(PlayScoreAnimition);
        }
        spriteCompent.sprite = scoreSprite;
        PlayScoreAnimition = ScoreAnimation();
        StartCoroutine(PlayScoreAnimition);
    }

    private IEnumerator BackGame() {
        if (stopPanel != null)
        {
            Animator animator = stopPanel.GetComponent<Animator>();
            if (animator)
            {
                animator.Play(stopPanelStopAnimation.name);
                yield return new WaitForSeconds(stopPanelStopAnimation.length);
                stopPanel.SetActive(false);
                isGame = true;
            }
        }
    }

    private IEnumerator StopGame() {
        if (stopPanel != null)
        {
            Animator animator = stopPanel.GetComponent<Animator>();
            if (animator)
            {
                stopPanel.SetActive(true);
                animator.Play(stopPanelStartAnimation.name);
                yield return new WaitForSeconds(stopPanelStartAnimation.length);
            }
        }
    }

    private IEnumerator ScoreAnimation() {
        if (!scorePanel.activeSelf) {
            scorePanel.SetActive(true);
        }
        if (scoreAnimator)
        {
            scoreAnimator.Play(scoreAnimition.name , 0 , 0);
            yield return new WaitForSeconds(scoreAnimition.length) ;
            scorePanel.SetActive(false);
        }
    }
}
