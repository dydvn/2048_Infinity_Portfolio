using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

using Random = UnityEngine.Random;

public class Game_master : MonoBehaviour
{
    AudioSource as_BGM;
    public Move_player move_Player;

    public GameObject go_newbox;
    public GameObject go_Portal;
    public GameObject go_ClearText;
    public GameObject go_ScoreText;
    public GameObject go_GameOverUI;
    public GameObject go_GameOverScore;
    public GameObject go_GameOverText;
    public GameObject go_Player;
    public GameObject go_ScoreValue;
    public GameObject[] go_Background;
    public GameObject[] go_Ground;
    public GameObject go_Arrow;
    public GameObject go_Particle;
    public GameObject go_PauseBTN;
    public GameObject go_PausePanel;
    public GameObject go_GroundLine;
    public GameObject go_Phoenix;
    public GameObject go_BoostEffect;
    public Vector3 v3_PortalPos;
    public Text txt_Score;
    public TextMesh txt_ScoreUnderPlayer;
    public Text txt_Score_GameOver;

    [Header("Text")]
    public Text txt_ADCount;

    public AudioSource as_Sfx;
    public AudioSource as_SfxJump;
    public AudioSource as_SfxEat;
    public AudioClip ac_Clear;
    public AudioClip ac_Death;
    public AudioClip ac_Warp;
    public AudioClip ac_BGM1;
    public AudioClip ac_BGM2;
    public AudioClip ac_BGM_Boost;
    public SpriteRenderer sr_BG;
    public Sprite[] sprt_BG;
    public Sprite sprt_NormalBox;
    public Sprite sprt_AnswerBox;
    public ParticleSystem go_BoosterParticle;
    public int nScore;
    public int nScore_2nd;
    public int nStage;
    public int nClearCount;
    public float ftime;
    public float fTime2;
    public bool isDead = false;

    GameObject[] go_Box = new GameObject[4];
    List<GameObject> boxPool = new();

    public IEnumerator co_BoxRespone;
    public IEnumerator co_BoxBoost;

    private int adCount;
    private bool beforeBoost;
    public float boostPercent;
    private Vector3[] box_pos = new Vector3[4];

    private void Start()
    {
        nStage = Master.nStage;
        go_ClearText.GetComponent<MeshRenderer>().sortingLayerName = "C_Layer";
        txt_ScoreUnderPlayer.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "C_Layer";
        SetADCount(nStage - 1);
        Master.boxSpeed = nStage + 1.0f;

        float fTemp = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x + 0.79f;
        for (int i = 0; i < 4; i++)
        {
            box_pos[i].x = fTemp;
        }
        box_pos[0].y = 2.17f;
        box_pos[1].y = 0.92f;
        box_pos[2].y = -0.4f;
        box_pos[3].y = -1.79f;

        go_Background[0].SetActive(nStage == 1);
        go_Background[1].SetActive(nStage == 2);
        go_Ground[0].SetActive(nStage == 1);
        go_Ground[1].SetActive(nStage == 2);
        go_ScoreText.SetActive(nStage == 2);
        go_ScoreValue.SetActive(nStage == 2);
        nScore = 2;
        txt_Score.text = txt_ScoreUnderPlayer.text = nScore.ToString();
        boxPool.Add(Instantiate(go_newbox));

        if (!as_BGM)
        {
            as_BGM = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
        }
        co_BoxBoost = box_boost();
        go_BoosterParticle.gameObject.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x, 0, 0);

        //게임 초기화
        if (nStage == 1)
        {   //1탄이면

            if (Master.nFlyCount == 0)
                go_Arrow.SetActive(true);
            else
                box_Spawn();
        }
        else if (nStage == 2)
        {   //2탄이면

            // 값 세팅
            beforeBoost = true;
            boostPercent = 0;
            go_Particle.SetActive(true);
            go_Particle.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(0.9f, 0.5f)).x, go_Particle.transform.position.y, 0);
            co_BoxRespone = box_re2();
            if (PlayerPrefs.GetInt("isStage1Clear") == 0)
            {
                PlayerPrefs.SetInt("isStage1Clear", 1);
            }
            Master.nFlyCount = Master.nFlyCount == 0 ? 1 : Master.nFlyCount;


            // 배경 세팅
            sr_BG.sprite = sprt_BG[Random.Range(0, 3)];
            var bgLeft = new Vector3(sr_BG.transform.position.x - sr_BG.sprite.rect.width * 0.01f, 0, 0);
            var bgRight = new Vector3(sr_BG.transform.position.x + sr_BG.sprite.rect.width * 0.01f, 0, 0);
            var left = Instantiate(sr_BG.gameObject);
            var right = Instantiate(sr_BG.gameObject);
            left.transform.position = bgLeft;
            left.transform.localScale = new Vector3(-1, 1, 1);
            right.transform.position = bgRight;
            right.transform.localScale = new Vector3(-1, 1, 1);



            // 실행
            StartCoroutine(co_BoxRespone);
            if (!Manager_Admob.Instance.CanShowRewardedAd())
                Manager_Admob.Instance.LoadRewardedAd();
            if (!Manager_Admob.Instance.CanShowInterstitialAd())
                Manager_Admob.Instance.LoadInterstitialAd();
        }
        

        nScore_2nd = 0;
        go_ClearText.SetActive(false);


        as_Sfx.mute = !Master.isSFX;
        as_SfxJump.mute = !Master.isSFX;
        as_SfxEat.mute = !Master.isSFX;

        Manager_Admob.Instance.LoadBannerAd();
    }

    private void Update()
    {
        txt_Score.text = nScore.ToString();
        txt_ScoreUnderPlayer.text = txt_Score.text;

        if (nStage == 1)
        {
            if (nScore >= 2048)
            {
                Master.nClearCount++;
                PlayerPrefs.SetInt("nClearCount", Master.nClearCount);

                Social.ReportProgress(GPGSIds.achievement_stage_1_clear, 100, null);

                StartCoroutine(Finish("Congratulation!\n2048!", nStage));
                Instantiate(go_Portal, new Vector3(9, box_pos[Random.Range(0, 3)].y, 0), Quaternion.identity);
            }
        }
        else if (nStage == 2)
        {
            txt_Score.text = nScore_2nd.ToString();
            if (nScore >= 2048)
            {
                Master.nClearCount++;
                PlayerPrefs.SetInt("nClearCount", Master.nClearCount);

                if (Master.nClearCount >= 50)
                    Social.ReportProgress(GPGSIds.achievement_2048, 100, null);
                else if (Master.nClearCount >= 45)
                    Social.ReportProgress(GPGSIds.achievement_2048, 90, null);
                else if (Master.nClearCount >= 40)
                    Social.ReportProgress(GPGSIds.achievement_2048, 80, null);
                else if (Master.nClearCount >= 35)
                    Social.ReportProgress(GPGSIds.achievement_2048, 70, null);
                else if (Master.nClearCount >= 30)
                    Social.ReportProgress(GPGSIds.achievement_2048, 60, null);
                else if (Master.nClearCount >= 25)
                    Social.ReportProgress(GPGSIds.achievement_2048, 50, null);
                else if (Master.nClearCount >= 20)
                    Social.ReportProgress(GPGSIds.achievement_2048, 40, null);
                else if (Master.nClearCount >= 15)
                    Social.ReportProgress(GPGSIds.achievement_2048, 30, null);
                else if (Master.nClearCount >= 10)
                    Social.ReportProgress(GPGSIds.achievement_2048, 20, null);
                else if (Master.nClearCount >= 5)
                    Social.ReportProgress(GPGSIds.achievement_2048, 10, null);

                StartCoroutine(Finish("Congratulation!\n2048!"));
            }
        }
    }

    public IEnumerator Finish(string _str, int stageNum = 2)
    {
        as_Sfx.clip = ac_Clear;
        as_Sfx.Play();
        SetClearText(_str);
        nScore = 2;
        if (stageNum == 2)
            StopCoroutine(co_BoxRespone);
        BoxClear();

        yield return new WaitForSeconds(3);

        go_ClearText.SetActive(false);
        if (stageNum == 2 && !isDead)
            StartCoroutine(co_BoxRespone);
    }

    public void SetClearText(string _str)
    {
        go_ClearText.GetComponent<TextMesh>().text = _str;
        go_ClearText.SetActive(true);
    }

    public void BoxClear()
    {
        foreach(var obj in boxPool)
        {
            obj.SetActive(false);
        }
    }

    public void box_Spawn()
    {
        int nAnswerBox;

        for (int i = 0; i < 4; i++)
        {
            go_Box[i] = GetBoxFromPool();
            go_Box[i].SetActive(true);
            go_Box[i].transform.position = box_pos[i];
            go_Box[i].GetComponent<SpriteRenderer>().sprite = sprt_NormalBox;
        }

        nAnswerBox = Random.Range(0, 4);
        go_Box[nAnswerBox].GetComponent<Move_box>().SetValue(nScore.ToString());
        go_Box[nAnswerBox].GetComponent<SpriteRenderer>().sprite = sprt_AnswerBox;

        int[] nTemp_ = new int[4];
        int nTemp;

        for (int j = 0; j < 4; j++)
        {
            if (j == nAnswerBox)
                continue;

            do
            {
                nTemp = (int)Mathf.Pow(2, Random.Range(1, 11));
                nTemp_[j] = nTemp;
                for (int k = 0; k < 4; k++)
                {
                    if (k == j)
                        continue;
                    while (nTemp == nTemp_[k])
                    {
                        nTemp = (int)Mathf.Pow(2, Random.Range(1, 11));
                    }
                    nTemp_[j] = nTemp;
                }
            } while (nTemp == nScore || nTemp == nScore * 2);


            go_Box[j].GetComponent<Move_box>().SetValue(nTemp.ToString());
        }
    }

    public IEnumerator box_re2()
    {
        float fTempA = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x - 0.01f;
        float fTempB = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x + 5.99f;
        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                box_pos[i].x = Random.Range(fTempA, fTempB);
                go_Box[i] = GetBoxFromPool();
                go_Box[i].SetActive(true);
                go_Box[i].transform.position = box_pos[i];
                go_Box[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            int nAnswerBox = Random.Range(0, 4);
            int nAnswerBox2 = 3 - nAnswerBox;

            go_Box[nAnswerBox].GetComponent<Move_box>().SetValue(nScore.ToString());
            go_Box[nAnswerBox2].GetComponent<Move_box>().SetValue((nScore * 2).ToString());

            int n3rd = 0;
            int nTemp;

            bool isBoost = GetAlphabet(boostPercent) & !beforeBoost;
            beforeBoost = isBoost;

            for (int j = 0; j < 4; j++)
            {
                if (j == nAnswerBox || j == nAnswerBox2)
                    continue;


                do
                {
                    nTemp = (int)Mathf.Pow(2, Random.Range(1, 11));
                } while (nTemp == nScore || nTemp == nScore * 2 || nTemp == n3rd);

                if (n3rd == 0)
                {
                    n3rd = nTemp;
                }
                else
                {
                    if (isBoost)
                    {
                        go_Box[j].transform.GetChild(0).gameObject.SetActive(true);
                        go_Box[j].transform.GetChild(1).gameObject.SetActive(false);
                        nTemp = -1;
                    }
                }
                go_Box[j].GetComponent<Move_box>().SetValue(nTemp.ToString());
            }

            boostPercent += 1;

            yield return new WaitForSeconds(fTime2);
        }

        bool GetAlphabet(float _percent)
        {
            int ran = Random.Range(0, 100);
            if (ran < _percent)
                return true;

            return false;
        }
    }

    public IEnumerator BoostStart()
    {
        Master.boostMode = true;
        BoxClear();
        Master.boxSpeed = 10;
        boostPercent = 0;
        StopCoroutine(co_BoxRespone);
        StartCoroutine(co_BoxBoost);
        go_GroundLine.SetActive(true);
        go_BoosterParticle.Play();
        as_BGM.clip = ac_BGM_Boost;
        as_BGM.Play();
        go_Phoenix.transform.position = move_Player.gameObject.transform.position;
        go_Phoenix.SetActive(true);
        go_PauseBTN.SetActive(false);
        StartCoroutine(CameraZoomOut());
        txt_ScoreUnderPlayer.gameObject.SetActive(false);

        IEnumerator CameraZoomOut()
        {
            while(Camera.main.orthographicSize < 7f)
            {
                Camera.main.orthographicSize += Time.deltaTime * 3;
                yield return null;
            }
            Camera.main.orthographicSize = 7;
        }



        yield return new WaitForSeconds(18);




        StartCoroutine(CameraZoomIn());
        IEnumerator CameraZoomIn()
        {
            while (Camera.main.orthographicSize > 5)
            {
                Camera.main.orthographicSize -= Time.deltaTime * 3;
                yield return null;
            }
            Camera.main.orthographicSize = 5;
        }

        txt_ScoreUnderPlayer.gameObject.SetActive(true);
        as_BGM.clip = ac_BGM2;
        as_BGM.Play();
        go_BoosterParticle.Stop();
        StopCoroutine(co_BoxBoost);
        go_PauseBTN.SetActive(true);
        go_GroundLine.SetActive(false);
        go_BoostEffect.SetActive(false);
        Master.boxSpeed = 3;
        Master.boostMode = false;
        StartCoroutine(Finish("Again 2048!"));
    }

    public IEnumerator box_boost()
    {
        float fTempA = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x;
        float fTempB = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x + 2;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                box_pos[i].x = Random.Range(fTempA, fTempB);
                go_Box[i] = GetBoxFromPool();
                go_Box[i].SetActive(true);
                go_Box[i].transform.position = box_pos[i];
                go_Box[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                go_Box[i].GetComponent<Move_box>().SetValue(Mathf.Pow(2, Random.Range(1, 11)).ToString());
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    public void Regame()
    {
        SceneManager.LoadScene(Master.SCENE_PLAY);
    }

    public void Home()
    {
        SceneManager.LoadScene(Master.SCENE_MAIN);
        if (nStage == 2)
        {
            as_BGM.clip = ac_BGM1;
            as_BGM.Play();
        }
        Time.timeScale = 1;
    }

    public void BTN_SS()
    {
        StartCoroutine("BTN_ScreenshotShare");
    }

    public IEnumerator BTN_ScreenshotShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("Subject goes here").SetText("Hello world!").Share();
    }

    public void BTN_Pause()
    {
        go_PausePanel.SetActive(Time.timeScale > 0);
        Time.timeScale = Convert.ToInt32(!(Time.timeScale > 0));
    }

    public void BTN_AD()
    {
        if (adCount < 1)
            return;

        Manager_Admob.Instance.ShowRewardedAd(reward);

        //재시작
        bool reward()
        {
            isDead = false;
            go_GameOverUI.SetActive(false);
            go_Player.transform.SetPositionAndRotation(new Vector3(-1.76f, 0.33f, 0), Quaternion.identity);
            go_Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            move_Player.anim.ResetTrigger(move_Player.animTriggerID_Die);
            move_Player.anim.SetTrigger(move_Player.animTriggerID_Fly);
            if (Master.nStage == 1)
                box_Spawn();
            else
                StartCoroutine(BoostStart());
            SetADCount(0);
            return true;
        }
    }

    public void SetADCount(int _adCount)
    {
        adCount = _adCount;
        txt_ADCount.text = string.Format($"AD {_adCount}/1");
    }

    private GameObject GetBoxFromPool()
    {
        foreach(var obj in boxPool)
        {
            if (!obj.activeSelf)
                return obj;
        }
        var newObject = Instantiate(go_newbox);
        boxPool.Add(newObject);
        return newObject;
    }
}
