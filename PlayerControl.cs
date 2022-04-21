using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GoogleMobileAds.Api;
using UnityEngine.EventSystems;

public class Move_player : MonoBehaviour
{
    public Game_master game_master;

    AudioSource as_Temp;

    Rigidbody2D rb2d_Plyer;
    public Vector2 v2_Power;


    //광고
    private InterstitialAd interstitial;

    void Start()
    {
        as_Temp = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
        rb2d_Plyer = GetComponent<Rigidbody2D>();
        if (Master.nFlyCount == 0)
            rb2d_Plyer.gravityScale = 0;
        else
            rb2d_Plyer.gravityScale = 1;

        RequestInterstitial();
    }

    void Update()
    {
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
                        game_master.StartCoroutine(game_master.co_BoxRespone);
                    }
                    if (game_master.isPlay)
                    {
                        rb2d_Plyer.velocity = new Vector2(0, 0);
                        rb2d_Plyer.AddForce(v2_Power);
                        game_master.as_SfxJump.Play();
                        Master.nFlyCount++;
                        PlayerPrefs.SetInt("nFlyCount", Master.nFlyCount);
                        
                        
                        //업적
                        if (Master.nFlyCount >= 5000)
                            Social.ReportProgress(GPGSIds.JustFly, 100, null);
                        else if (Master.nFlyCount >= 4500)
                            Social.ReportProgress(GPGSIds.JustFly, 90, null);
                        else if (Master.nFlyCount >= 4000)
                            Social.ReportProgress(GPGSIds.JustFly, 80, null);
                        else if (Master.nFlyCount >= 3500)
                            Social.ReportProgress(GPGSIds.JustFly, 70, null);
                        else if (Master.nFlyCount >= 3000)
                            Social.ReportProgress(GPGSIds.JustFly, 60, null);
                        else if (Master.nFlyCount >= 2500)
                            Social.ReportProgress(GPGSIds.JustFly, 50, null);
                        else if (Master.nFlyCount >= 2000)
                            Social.ReportProgress(GPGSIds.JustFly, 40, null);
                        else if (Master.nFlyCount >= 1500)
                            Social.ReportProgress(GPGSIds.JustFly, 30, null);
                        else if (Master.nFlyCount >= 1000)
                            Social.ReportProgress(GPGSIds.JustFly, 20, null);
                        else if (Master.nFlyCount >= 500)
                            Social.ReportProgress(GPGSIds.JustFly, 10, null);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ground")
        {
            Death();
        }

        if (other.transform.GetComponent<TextMesh>())
        {
            int nTemp = int.Parse(other.transform.GetComponent<TextMesh>().text);

            if (nTemp == game_master.nScore)
            {
                game_master.as_Sfx.clip = game_master.ac_Eat;
                game_master.as_Sfx.Play();
                game_master.nScore_2nd += 1;
                game_master.nScore = game_master.nScore * 2;
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Death();
            }
        }

        if (other.name == "Portal(Clone)")
        {

            as_Temp.clip = game_master.ac_Warp;
            as_Temp.Play();
            SceneManager.LoadScene(Master.SCENE_BLACKHOLE);
        }

    }

    void Death()
    {
        Master.nDeathCount++;
        Master.nPlayCount++;
        if (Master.nPlayCount == 5)
        {
            Master.nPlayCount = 0;
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
        }
        PlayerPrefs.SetInt("nDeathCount", Master.nDeathCount);

        if (Master.nDeathCount >= 1)
            Social.ReportProgress(GPGSIds.FirstDie, 100, null);

        //업적
        if (Master.nDeathCount >= 200)
            Social.ReportProgress(GPGSIds.JustDie, 100, null);
        else if (Master.nDeathCount >= 180)
            Social.ReportProgress(GPGSIds.JustDie, 90, null);
        else if (Master.nDeathCount >= 160)
            Social.ReportProgress(GPGSIds.JustDie, 80, null);
        else if (Master.nDeathCount >= 140)
            Social.ReportProgress(GPGSIds.JustDie, 70, null);
        else if (Master.nDeathCount >= 120)
            Social.ReportProgress(GPGSIds.JustDie, 60, null);
        else if (Master.nDeathCount >= 100)
            Social.ReportProgress(GPGSIds.JustDie, 50, null);
        else if (Master.nDeathCount >= 80)
            Social.ReportProgress(GPGSIds.JustDie, 40, null);
        else if (Master.nDeathCount >= 60)
            Social.ReportProgress(GPGSIds.JustDie, 30, null);
        else if (Master.nDeathCount >= 40)
            Social.ReportProgress(GPGSIds.JustDie, 20, null);
        else if (Master.nDeathCount >= 20)
            Social.ReportProgress(GPGSIds.JustDie, 10, null);

        game_master.isPlay = false;
        game_master.as_Sfx.clip = game_master.ac_Death;
        game_master.as_Sfx.Play();
        if (Master.nStage == 2)
        {
            if (game_master.nScore_2nd > PlayerPrefs.GetInt("nBestScore"))
            {
                PlayerPrefs.SetInt("nBestScore", game_master.nScore_2nd);
                Social.ReportScore(game_master.nScore_2nd, GPGSIds.leaderboard, (bool bSuccess) =>
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
            game_master.go_GameOverText.SetActive(true);
            game_master.go_GameOverScore.SetActive(false);
        }
        else if (game_master.nStage == 2)
        {
            game_master.go_GameOverScore.SetActive(true);
            game_master.go_GameOverText.SetActive(false);
            game_master.txt_Score_GameOver.text = PlayerPrefs.GetInt("nBestScore").ToString();
        }
        Time.timeScale = 0;
    }


    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        //Test ID
        //string adUnitId = "ca-app-pub-****************/**********";
        string adUnitId = "ca-app-pub-****************/**********";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-****************/**********";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.

        this.interstitial.LoadAd(request);
    }

    private void OnDestroy()
    {
        if (interstitial.IsLoaded())
            interstitial.Destroy();
    }
}
