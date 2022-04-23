/// <summary>
/// 게임 로비 Scene에서 옵션 버튼, 업적 버튼 등의 기능들을 모아놓은 스크립트입니다.
/// 사운드 옵션의 경우 사용자가 설정한 부분을 PlayerPrefs를 이용해 기기에 저장한 뒤 게임 재시작을 해도 그대로 적용되게끔 해놓았습니다.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_master : MonoBehaviour
{
    public bool bStageReset;
    public GameObject go_Sound;
    public GameObject go_Option;
    public GameObject go_Exit;
    public GameObject go_UI_MainObject;
    public GameObject go_UI_Ranking;
    public GameObject go_UI_Achivement;
    public GameObject go_LoadingBar;
    public Image img_ProgressBar;
    public Text txt_BestScore;
    public Text txt_Tip;

    public Sprite[] sprite_Option = new Sprite[4];

    AudioSource as_BGM;

    public Image img_BGM;
    public Image img_SFX;

    public AudioClip ac_MainBGM;
    public AudioClip ac_BGM2;

    private GameObject go_SountClone;

    public string[] strTip = new string[3];

    private void Start()
    {
        if (bStageReset)
        {
            Master.nFlyCount = 0;
            PlayerPrefs.SetInt("isStage1Clear", 0);
        }

        if (!GameObject.Find("BGM(Clone)"))
        {
            go_SountClone = Instantiate(go_Sound);
            as_BGM = go_SountClone.GetComponent<AudioSource>();
            DontDestroyOnLoad(go_SountClone);
        }
        else
        {
            as_BGM = GameObject.Find("BGM(Clone)").GetComponent<AudioSource>();
        }

        if (PlayerPrefs.GetInt("BGM") == 0)
            Master.isBGM = true;
        else
            Master.isBGM = false;   

        if (PlayerPrefs.GetInt("SFX") == 0)
            Master.isSFX = true;
        else
            Master.isSFX = false;


        GameObject[] go_Temp = GameObject.FindGameObjectsWithTag("BGM");

        int nSIze = go_Temp.Length;
        if (nSIze > 1)
        {
            for (int i = 0; i < nSIze - 1; i++)
            {
                Destroy(go_Temp[i]);
            }
        }


        as_BGM.enabled = Master.isBGM;

        txt_BestScore.text = PlayerPrefs.GetInt("nBestScore").ToString();

        Master.nClearCount = PlayerPrefs.GetInt("nClearCount");
        Master.nFlyCount = PlayerPrefs.GetInt("nFlyCount");
        Master.nDeathCount = PlayerPrefs.GetInt("nDeathCount");

        go_Exit.SetActive(false);

        if (bStageReset)
        {
            Master.nFlyCount = 0;
            PlayerPrefs.SetInt("isStage1Clear", 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameExitUI();
        }
    }

    public void GameStart()
    {
        if (PlayerPrefs.GetInt("isStage1Clear") == 0)
            Master.nStage = 1;
        else
        {
            Master.nStage = 2;
        }
        go_UI_MainObject.SetActive(false);
        go_UI_Ranking.SetActive(false);
        go_UI_Achivement.SetActive(false);
        go_LoadingBar.SetActive(true);
        txt_Tip.text = strTip[Random.Range(0, 3)];
        StartCoroutine(Loading(Master.SCENE_PLAY));
    }

    public void SoundOnOff(int nSoundCode)
    {
        if (nSoundCode == 0)
        {
            if (PlayerPrefs.GetInt("BGM") == 1)
            {   //BGM 켜기
                PlayerPrefs.SetInt("BGM", 0);
                Master.isBGM = true;
                as_BGM.enabled = Master.isBGM;
                img_BGM.sprite = sprite_Option[0];
            }
            else
            {   //BGM 끄기
                PlayerPrefs.SetInt("BGM", 1);
                Master.isBGM = false;
                as_BGM.enabled = Master.isBGM;
                img_BGM.sprite = sprite_Option[1];
            }
        }
        if (nSoundCode == 1)
        {
            if (PlayerPrefs.GetInt("SFX") == 1)
            {   //SFX 켜기
                PlayerPrefs.SetInt("SFX", 0);
                Master.isSFX = true;
                img_SFX.sprite = sprite_Option[2];
            }
            else
            {   //SFX 끄기
                PlayerPrefs.SetInt("SFX", 1);
                Master.isSFX = false;
                img_SFX.sprite = sprite_Option[3];
            }
        }
    }

    public void OptionOnOff()
    {
        go_Option.SetActive(!go_Option.activeSelf);
        if (Master.isBGM)
            img_BGM.sprite = sprite_Option[0];
        else
            img_BGM.sprite = sprite_Option[1];

        if (Master.isSFX)
            img_SFX.sprite = sprite_Option[2];
        else
            img_SFX.sprite = sprite_Option[3];

    }

    public void ShowLeaderBoard()
    {
        Social.ShowLeaderboardUI();
    }
    public void ShowAchivement()
    {
        Social.ShowAchievementsUI();
    }

    public void GameExitUI()
    {
        go_Exit.SetActive(!go_Exit.activeSelf);
    }
    public void GameExit()
    {
        Application.Quit();
    }

    IEnumerator Loading(int nNextScene)
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nNextScene);
        op.allowSceneActivation = false;

        float fTimer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            fTimer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                img_ProgressBar.fillAmount = Mathf.Lerp(img_ProgressBar.fillAmount, op.progress, fTimer);

                if (img_ProgressBar.fillAmount >= op.progress)
                {
                    fTimer = 0.0f;
                }
            }
            else
            {
                img_ProgressBar.fillAmount = Mathf.Lerp(img_ProgressBar.fillAmount, 1f, fTimer);

                if (img_ProgressBar.fillAmount >= 1.0f)
                {
                    if (Master.nStage == 2)
                    {
                        as_BGM.clip = ac_BGM2;
                        as_BGM.Play();
                    }
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
