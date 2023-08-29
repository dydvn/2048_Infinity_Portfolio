...

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

    ...
