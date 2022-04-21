using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using GoogleMobileAds.Api;

public class Game_master : MonoBehaviour
{
    AudioSource as_BGM;

    public GameObject go_newbox;
    public GameObject go_Portal;
    public GameObject go_ClearText;
    public GameObject go_ScoreText;
    public GameObject go_GameOverUI;
    public GameObject go_GameOverScore;
    public GameObject go_ScoreTextUnderPlayer;
    public GameObject go_GameOverText;
    public GameObject go_Player;
    public GameObject go_ScoreValue;
    public GameObject[] go_Background;
    public GameObject[] go_Ground;
    public GameObject[] go_Cloud;
    public GameObject go_Arrow;
    public GameObject go_Particle;
    public GameObject go_PausePanel;
    public Vector3 v3_PortalPos;
    public Text txt_Score;
    public TextMesh txt_ScoreUnderPlayer;
    public Text txt_Score_GameOver;
    public AudioSource as_Sfx;
    public AudioSource as_SfxJump;
    public AudioClip ac_Clear;
    public AudioClip ac_Death;
    public AudioClip ac_Warp;
    public AudioClip ac_Eat;
    public AudioClip ac_BGM1;
    public AudioClip ac_BGM2;
    public SpriteRenderer sr_BG;
    public Sprite[] sprt_BG;
    public Sprite sprt_AnswerBox;
    public int nScore;
    public int nScore_2nd;
    public int nStage;
    public int nClearCount;
    public float ftime;
    public float fTime2;
    public bool isPlay;

    GameObject[] go_Box = new GameObject[4];
    GameObject go_NewPortal;
    TextMesh[] txt_Box = new TextMesh[4];

    public IEnumerator co_BoxRespone;
    public IEnumerator co_BoxRespone2;

    private bool isFisrt;
    private Vector3[] box_pos = new Vector3[4];

    private Vector3 Temp;

    private void Start()
    {
        nStage = Master.nStage;
        isFisrt = true;
        isPlay = true;

        //게임 초기화
        if (Master.nStage == 1)
        {   //1탄이면
            go_Background[0].SetActive(true);
            go_Background[1].SetActive(false);
            go_Ground[0].SetActive(true);
            go_Ground[1].SetActive(false);
            go_ScoreText.SetActive(false);
            go_ScoreValue.SetActive(false);
            go_ScoreTextUnderPlayer.SetActive(true);
            txt_ScoreUnderPlayer.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "C_Layer";
            for (int i = 0; i < 3; i++)
            {
                go_Cloud[i].SetActive(true);
            }
            if (!as_BGM)
            {
                as_BGM = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
            }
        }
        else if (Master.nStage == 2)
        {   //2탄이면
            nScore = 2;
            go_Background[0].SetActive(false);
            go_Background[1].SetActive(true);
            go_Ground[0].SetActive(false);
            go_Ground[1].SetActive(true);
            go_ScoreText.SetActive(true);
            go_ScoreTextUnderPlayer.SetActive(false);
            go_Particle.SetActive(true);
            go_ScoreValue.SetActive(true);
            sr_BG.sprite = sprt_BG[Random.Range(0, 3)];
            for (int i = 0; i < 3; i++)
            {
                go_Cloud[i].SetActive(false);
            }
            if (PlayerPrefs.GetInt("isStage1Clear") == 0)
            {
                PlayerPrefs.SetInt("isStage1Clear", 1);
            }

            if (!as_BGM)
            {
                as_BGM = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
            }
        }
        else
            print("이러면 안 돼");

        float fTemp = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0.5f)).x + 0.79f;
        for (int i = 0; i < 4; i++)
        {
            box_pos[i].x = fTemp;
        }
        box_pos[0].y = 2.17f;
        box_pos[1].y = 0.92f;
        box_pos[2].y = -0.4f;
        box_pos[3].y = -1.79f;

        nScore_2nd = 0;
        go_ClearText.SetActive(false);
        co_BoxRespone = box_re();
        co_BoxRespone2 = box_re2();

        if (nStage == 1 && Master.nFlyCount != 0)
            StartCoroutine(co_BoxRespone);
        else if (nStage == 2)
            StartCoroutine(co_BoxRespone2);
        else
            go_Arrow.SetActive(true);

        as_Sfx.enabled = Master.isSFX;
    }

    private void Update()
    {
        if (nStage == 1)
        {
            txt_Score.text = nScore.ToString();
            txt_ScoreUnderPlayer.text = txt_Score.text;
            if (nScore >= 2048)
            {
                Master.nClearCount++;
                PlayerPrefs.SetInt("nClearCount", Master.nClearCount);

                Social.ReportProgress(GPGSIds.stage1Clear, 100, null);

                as_Sfx.clip = ac_Clear;
                as_Sfx.Play();
                go_ClearText.SetActive(true);
                nScore = 2;
                StopCoroutine(co_BoxRespone);
                go_NewPortal = Instantiate(go_Portal, new Vector3(9, box_pos[Random.Range(0, 3)].y, 0), Quaternion.identity);
                StartCoroutine(StageOneCorou());
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
                    Social.ReportProgress(GPGSIds.Make2048, 100, null);
                else if (Master.nClearCount >= 45)
                    Social.ReportProgress(GPGSIds.Make2048, 90, null);
                else if (Master.nClearCount >= 40)
                    Social.ReportProgress(GPGSIds.Make2048, 80, null);
                else if (Master.nClearCount >= 35)
                    Social.ReportProgress(GPGSIds.Make2048, 70, null);
                else if (Master.nClearCount >= 30)
                    Social.ReportProgress(GPGSIds.Make2048, 60, null);
                else if (Master.nClearCount >= 25)
                    Social.ReportProgress(GPGSIds.Make2048, 50, null);
                else if (Master.nClearCount >= 20)
                    Social.ReportProgress(GPGSIds.Make2048, 40, null);
                else if (Master.nClearCount >= 15)
                    Social.ReportProgress(GPGSIds.Make2048, 30, null);
                else if (Master.nClearCount >= 10)
                    Social.ReportProgress(GPGSIds.Make2048, 20, null);
                else if (Master.nClearCount >= 5)
                    Social.ReportProgress(GPGSIds.Make2048, 10, null);

                as_Sfx.clip = ac_Clear;
                as_Sfx.Play();
                go_ClearText.SetActive(true);
                nScore = 2;
                StopCoroutine(co_BoxRespone2);
                StartCoroutine(StageTwoCorou());
                GameObject[] go_Temp = GameObject.FindGameObjectsWithTag("Box");
                int nSize = go_Temp.Length;
                for (int i = 0; i < nSize; i++)
                {
                    Destroy(go_Temp[i]);
                }
            }
        }
    }

    public IEnumerator box_re()
    {
        yield return new WaitForSeconds(2);

        int nAnswerBox = Random.Range(0, 4);

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                go_Box[i] = Instantiate(go_newbox, box_pos[i], Quaternion.identity);
                txt_Box[i] = go_Box[i].transform.GetChild(0).GetComponent<TextMesh>();
            }

            if (!isFisrt)
                nAnswerBox = Random.Range(0, 4);
            txt_Box[nAnswerBox].text = nScore.ToString();
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
                } while (nTemp == nScore);


                txt_Box[j].text = (nTemp).ToString();
            }
            if (isFisrt)
                isFisrt = false;

            yield return new WaitForSeconds(ftime);
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
                go_Box[i] = Instantiate(go_newbox, box_pos[i], Quaternion.identity);
                txt_Box[i] = go_Box[i].transform.GetChild(0).GetComponent<TextMesh>();
                go_Box[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            int nAnswerBox = Random.Range(0, 4);
            int nAnswerBox2 = 3 - nAnswerBox;

            txt_Box[nAnswerBox].text = nScore.ToString();
            txt_Box[nAnswerBox2].text = (nScore * 2).ToString();

            int n3rd = 0;

            for (int j = 0; j < 4; j++)
            {
                if (j == nAnswerBox || j == nAnswerBox2)
                    continue;

                int nTemp;

                do
                {
                    nTemp = (int)Mathf.Pow(2, Random.Range(1, 11));
                } while (nTemp == nScore || nTemp == nScore * 2 || nTemp == n3rd);

                if (n3rd == 0)
                    n3rd = nTemp;


                txt_Box[j].text = (nTemp).ToString();
            }

            yield return new WaitForSeconds(fTime2);
        }
    }

    public void Regame()
    {
        isPlay = true;
        if (nStage == 1)
        {
            SceneManager.LoadScene(Master.SCENE_PLAY);
            Time.timeScale = 1;
        }
        else if (nStage == 2)
        {
            SceneManager.LoadScene(Master.SCENE_PLAY);
            Time.timeScale = 1;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(Master.SCENE_MAIN);
        if (Master.nStage == 2)
        {
            as_BGM.clip = ac_BGM1;
            as_BGM.Play();
        }
        Time.timeScale = 1;
    }

    IEnumerator StageOneCorou()
    {
        yield return new WaitForSeconds(3);
        go_ClearText.SetActive(false);
    }
    IEnumerator StageTwoCorou()
    {
        yield return new WaitForSeconds(3);
        go_ClearText.SetActive(false);
        StartCoroutine(co_BoxRespone2);
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
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            go_PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            go_PausePanel.SetActive(false);
        }
    }
}
