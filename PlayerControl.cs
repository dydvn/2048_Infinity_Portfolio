using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Move_player : MonoBehaviour
{
    public Game_master game_master;
    public Animator anim;

    AudioSource as_Temp;

    Rigidbody2D rb2d_Plyer;
    public Vector2 v2_Power;

    public int animTriggerID_Fly = Animator.StringToHash("fly");
    public int animTriggerID_Die = Animator.StringToHash("die");


    void Start()
    {
        as_Temp = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
        rb2d_Plyer = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb2d_Plyer.gravityScale = Master.nFlyCount == 0 ? 0 : 1;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Master.nFlyCount == 0)
            {
                game_master.go_Arrow.SetActive(false);
                rb2d_Plyer.gravityScale = 1;
                game_master.box_Spawn();
            }
            Master.nFlyCount++;
            PlayerPrefs.SetInt("nFlyCount", Master.nFlyCount);
            rb2d_Plyer.velocity = Vector2.zero;
            rb2d_Plyer.AddForce(v2_Power);
            game_master.as_SfxJump.Play();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(Master.SCENE_PLAY);
            Time.timeScale = 1;
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (Master.nFlyCount == 0)
                    {
                        game_master.go_Arrow.SetActive(false);
                        rb2d_Plyer.gravityScale = 1;
                        game_master.box_Spawn();
                    }
                    Master.nFlyCount++;
                    PlayerPrefs.SetInt("nFlyCount", Master.nFlyCount);
                    rb2d_Plyer.velocity = Vector2.zero;
                    rb2d_Plyer.AddForce(v2_Power);
                    game_master.as_SfxJump.Play();

                    if (Master.nFlyCount >= 5000)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 100, null);
                    else if (Master.nFlyCount >= 4500)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 90, null);
                    else if (Master.nFlyCount >= 4000)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 80, null);
                    else if (Master.nFlyCount >= 3500)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 70, null);
                    else if (Master.nFlyCount >= 3000)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 60, null);
                    else if (Master.nFlyCount >= 2500)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 50, null);
                    else if (Master.nFlyCount >= 2000)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 40, null);
                    else if (Master.nFlyCount >= 1500)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 30, null);
                    else if (Master.nFlyCount >= 1000)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 20, null);
                    else if (Master.nFlyCount >= 500)
                        Social.ReportProgress(GPGSIds.achievement_just_fly, 10, null);
                }
            }
        }
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (game_master.isDead)
        {
            return;
        }

        if (other.tag == "ground")
        {
            Death();
        }
        else if(Master.boostMode)
        {
            game_master.as_SfxEat.Play();
            game_master.nScore_2nd += 1;
            other.gameObject.SetActive(false);
        }
        else if (other.name == "Portal(Clone)")
        {
            as_Temp.clip = game_master.ac_Warp;
            as_Temp.Play();
            SceneManager.LoadScene(Master.SCENE_BLACKHOLE);
        }
        else
        {
            int tempValue = other.GetComponent<Move_box>().GetValue();

            if (tempValue > 0 && tempValue == game_master.nScore)
            {
                other.gameObject.SetActive(false);
                game_master.as_SfxEat.Play();
                game_master.nScore_2nd += 1;
                game_master.nScore = game_master.nScore * 2;

                if (Master.nStage == 1 && game_master.nScore < 2048)
                {
                    game_master.box_Spawn();
                }
            }
            else if (tempValue <= 0)
            {
                game_master.as_SfxEat.Play();
                other.gameObject.SetActive(false);

                game_master.StartCoroutine(game_master.BoostStart());
            }
            else
            {
                Death();
            }
        }
    }

    void Death()
    {
        if (Master.nStage == 2)
            game_master.StopCoroutine(game_master.co_BoxRespone);
        game_master.BoxClear();
        game_master.isDead = true;
        anim.ResetTrigger(animTriggerID_Fly);
        anim.SetTrigger(animTriggerID_Die);
        Master.nDeathCount++;
        Master.nPlayCount++;
        if (Master.nPlayCount == 5)
        {
            Master.nPlayCount = 0;
            Manager_Admob.Instance.ShowInterstitialAd();
        }
        PlayerPrefs.SetInt("nDeathCount", Master.nDeathCount);

#if UNITY_EDITOR

#elif UNITY_ANDROID

        if (Master.nDeathCount >= 1)
            Social.ReportProgress(GPGSIds.achievement_first_die, 100, null);


        if (Master.nDeathCount >= 200)
            Social.ReportProgress(GPGSIds.achievement_just_die, 100, null);
        else if (Master.nDeathCount >= 180)
            Social.ReportProgress(GPGSIds.achievement_just_die, 90, null);
        else if (Master.nDeathCount >= 160)
            Social.ReportProgress(GPGSIds.achievement_just_die, 80, null);
        else if (Master.nDeathCount >= 140)
            Social.ReportProgress(GPGSIds.achievement_just_die, 70, null);
        else if (Master.nDeathCount >= 120)
            Social.ReportProgress(GPGSIds.achievement_just_die, 60, null);
        else if (Master.nDeathCount >= 100)
            Social.ReportProgress(GPGSIds.achievement_just_die, 50, null);
        else if (Master.nDeathCount >= 80)
            Social.ReportProgress(GPGSIds.achievement_just_die, 40, null);
        else if (Master.nDeathCount >= 60)
            Social.ReportProgress(GPGSIds.achievement_just_die, 30, null);
        else if (Master.nDeathCount >= 40)
            Social.ReportProgress(GPGSIds.achievement_just_die, 20, null);
        else if (Master.nDeathCount >= 20)
            Social.ReportProgress(GPGSIds.achievement_just_die, 10, null);
#endif

        game_master.as_Sfx.clip = game_master.ac_Death;
        game_master.as_Sfx.Play();
        if (Master.nStage == 2)
        {
            if (game_master.nScore_2nd > PlayerPrefs.GetInt("nBestScore"))
            {
                PlayerPrefs.SetInt("nBestScore", game_master.nScore_2nd);
                Social.ReportScore(game_master.nScore_2nd, GPGSIds.leaderboard_best_score, (bool bSuccess) =>
                {
                    if (bSuccess)
                    {
                        Debug.Log("ReportLeaderBoard Success");
                    }
                    else
                    {
                        Debug.Log("ReportLeaderBoard Fall");
                    }
                }
                );
            }
        }
        game_master.go_GameOverUI.SetActive(true);
        if (game_master.nStage == 1)
        {
            game_master.go_GameOverScore.SetActive(false);
            game_master.go_GameOverText.SetActive(true);
        }
        else if (game_master.nStage == 2)
        {
            game_master.go_GameOverScore.SetActive(true);
            game_master.go_GameOverText.SetActive(false);
            game_master.txt_Score_GameOver.text = PlayerPrefs.GetInt("nBestScore").ToString();
        }
    }
}
