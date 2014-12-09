using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// manage tile grid
/// </summary>
public class SlotMachine : MonoBehaviour {
    // tile prefab
    public GameObject cellItemPrefab;

    // tile lines
    Transform[] lines;

    // tile items
    public Tile[] items;

    // auto clear match tiles game
    public bool isAuto = false;

    // tile size
    public Vector3 cellSize = new Vector3(1.6f, 1.4f, 1f);

    // tile scale
    public Vector3 cellScale = new Vector3(1.4f, 1f, 1.4f);

    public AudioSource spinSound, stopSound, cashSound;

    public HighlightLine highlightLine;
    public CoinDrop coinDrop, winBox;

    Line[] lineList;

    int totalLine = 5;
    int totalCell = 7;
    int money = 10000;
    int lanes = 25;
    int betIndex = 0;
    int bet = 1;
    int totalBet = 25;

    public UILabel moneyLabel, lineBetLabel, betLabel, winLabel, freeSpinsLabel;
    public GameObject freeSpinsObject, fxTitleObject;

    //Transform cam;

    bool isInput = true;

    int totalSymbols;
    int scatterIndex;
    int wildIndex;
    int totalFreeSpins;

    void Start()
    {
        totalSymbols = PayData.totalSymbols; // PayData.table.GetLength(0);
        scatterIndex = PayData.scatterIndex;
        wildIndex = PayData.wildIndex;
        totalFreeSpins = 0;

        //cam = Camera.main.transform;
        //Vector3 pos = cam.position;
        //pos.y = 8f;

        InitLabels();
        InitArena();
        isInput = true;
    }

    public void MinusBet()
    {
        //if (betIndex < 1) return;
        betIndex = (betIndex - 1 + PayData.betting.Length) % PayData.betting.Length;
        SetBet();
    }

    public void PlusBet()
    {
        //if (PayData.betting.Length - 2 < betIndex) return;
        betIndex = (betIndex + 1) % PayData.betting.Length;
        SetBet();
    }

    void SetBet()
    {
        totalBet = PayData.betting[betIndex];
        bet = totalBet / lanes;
        lineBetLabel.text = bet.ToString("#,##0");
        betLabel.text = totalBet.ToString("#,##0");
    }

    void SetMoney(int val)
    {
        tweenMoney = money;
        money = val;
        //moneyLabel.text = val.ToString("#,##0");
        TweenMoney(money, EaseType.EaseOutQuad);
    }

    void TweenMoneyUpdate()
    {
        moneyLabel.text = tweenMoney.ToString("#,##0");
    }

    public int tweenMoney;
    void TweenMoney(int val, EaseType easeType)
    {
        TweenParms parms = new TweenParms().Prop("tweenMoney", val).Ease(easeType).OnUpdate(TweenMoneyUpdate);
        HOTween.To(this, 0.5f, parms);
    }

    void PlusMoney(int val)
    {
        SetMoney(money + val);
    }

    void SetWin(int val)
    {
        winLabel.text = val.ToString("#,##0");
    }

    void SetFreeSpins(int val)
    {
        freeSpinsLabel.text = val.ToString();
    }

    void DisplayFreeSpins(bool isON)
    {
        NGUITools.SetActive(freeSpinsObject, isON);
    }

    void DisplayFxTitle(bool isON)
    {
        NGUITools.SetActive(fxTitleObject, isON);
    }

    void InitLabels()
    {
        SetFreeSpins(0);
        DisplayFreeSpins(false);
        DisplayFxTitle(false);
        SetBet();
        SetMoney(10000);
        SetWin(0);
    }

    public void ToggleAuto()
    {
        isAuto = !isAuto;
        if (isAuto && isInput) Spin();
    }

    // init game arena, draw tile grid
    void InitArena()
    {
        lines = new Transform[totalLine];
        lineList = new Line[totalLine];

        // tile line loop
        for (int i = 0; i < totalLine; i++)
        {
            GameObject pgo = new GameObject();
            pgo.name = "Base" + (i+1).ToString("000");
            pgo.transform.parent = transform;
            GameObject go = new GameObject();
            go.transform.parent = pgo.transform;
            Line script = go.AddComponent<Line>();
            script.idx = i;
            Transform tf = go.transform;
            lines[i] = tf;
            tf.parent = pgo.transform;
            //tf.localPosition = (i-4.5f) * Vector3.right * cellSize.x + Vector3.forward * (2 - (i % 2) * 0.5f) * cellSize.y;
            tf.localPosition = (i - 4.5f) * Vector3.right * cellSize.x + Vector3.forward * cellSize.y;
            tf.localScale = Vector3.one;
            go.name = "Line" + (i+1).ToString("000");

            script.items = new Tile[totalCell];
            // tile loop in some line
            for (int j = 0; j < totalCell; j++)
            {
                GameObject g = Instantiate(cellItemPrefab) as GameObject;
                g.name = "Tile" + (j+1).ToString("000");
                Transform t = g.transform;
                Tile c = g.GetComponent<Tile>();
                c.height = cellSize.y;
                c.slotMachine = this;
                c.SetTileType(Random.Range(0, totalSymbols) % totalSymbols);
                c.lineScript = script;
                script.items[j] = c;
                c.idx = j;
                t.parent = tf;
                t.localPosition = Vector3.forward * j * cellSize.y;
                t.localScale = cellScale;
                t.localRotation = Quaternion.identity;
            }
            script.idx = i;
            lineList[i] = script;
        }

        items = GetComponentsInChildren<Tile>();
        ClearChoice();
    }

    // delay method coroutine
    IEnumerator DelayAction(float dTime, System.Action callback)
    {
        yield return new WaitForSeconds(dTime);
        callback();
    }

    IEnumerator RepeatAction(float dTime, int count, System.Action callback1, System.Action callback2, System.Action callback3)
    {
        if (count>1) callback1();
        else callback3();
        if (--count>0) {
            if (count==1) callback2();
            yield return new WaitForSeconds(dTime);
            StartCoroutine( RepeatAction(dTime, count, callback1, callback2, callback3) );
        }
    }

    // auto clear match tiles and go next turn
    void DoRoll()
    {
        for (int i=0;i<totalLine; i++)
        {
            Transform line = lines[i];
            StartCoroutine(RepeatAction(0.1f, 8 + i*3, () =>
            {
                line.SendMessage("RollCells", true, SendMessageOptions.DontRequireReceiver);
            }, () =>
            {
                stopSound.Play();
            }, () =>
            {
                line.SendMessage("RollCells", false, SendMessageOptions.DontRequireReceiver);
            }));
        }
        isInput = false;
        StartCoroutine(DelayAction(2.2f, () =>
        {
            isInput = true;
            FindMatch(0, 2);
        }));
    }

    List<int> bingoList;
    List<int> typeList;
    List<int> countList;

    void FindMatch(int baseX, int baseY)
    {
        bingoList = new List<int>();
        typeList = new List<int>();
        countList = new List<int>();
        int totalScore = 0;
        int totalScatter = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Tile tile = lineList[baseX + j].items[baseY + i];
                int type = tile.GetTileType();
                if (type == scatterIndex)
                    totalScatter++;
            }
        }

        for (int i = 0; i < lanes; i++)
        {
            int count = 0;
            //Tile firstTile = lineList[baseX].items[baseY + PayData.line[i, 0]];
            int firstType = -1;
            bool isWild = false;

            for (int j = 0; j < 5; j++)
            {
                int pos = PayData.line[i, j];
                Tile tile = lineList[baseX + j].items[baseY + pos];
                int type = tile.GetTileType();
                if (type != wildIndex)
                {
                    firstType = type;
                    break;
                }
                else isWild = true;
            }
            if (firstType < 0 && isWild) firstType = wildIndex;
            
            for (int j = 1; j < 5; j++)
            {
                int pos = PayData.line[i, j];
                Tile tile = lineList[baseX + j].items[baseY + pos];
                int type = tile.GetTileType();
                if (type == scatterIndex || (firstType != type && wildIndex != type)) break;
                count++;
            }
            if (count > 0)
            {
                int score = PayData.table[firstType, count];
                if (score > 0)
                {
                    totalScore += score;
                    bingoList.Add(i);
                    typeList.Add(firstType);
                    countList.Add(count+1);
                }
            }
        }
        if (totalFreeSpins < 1) SetMoney(money - totalBet);
        else
        {
            if (totalFreeSpins > 0)
            {
                totalFreeSpins--;
                SetFreeSpins(totalFreeSpins);
                if (totalFreeSpins < 1) DisplayFreeSpins(false);
            }
        }
        SetWin(totalScore * bet);
        PlusMoney(totalScore * bet);

        if (bingoList.Count>0) Debug.Log("-------------------");
        for (int i = 0; i < bingoList.Count; i++)
        {
            int idx = bingoList[i];
            int type = typeList[i];
            int count = countList[i];
            string str = type + " : " + count + " = ";
            for (int ln = 0; ln < count; ln++)
            {
                int pos = PayData.line[idx, ln];
                str += "(" + ln + "," + pos + "), ";
                //Tile choiceTile = lineList[ln+baseX].items[pos+baseY];
                //choiceTile.SetChoice();
            }
            Debug.Log(str);
        }

        if (bingoList.Count > 0)
        {
            isHighlight = true;
            StartCoroutine(RepeatAction(0.3f, bingoList.Count + 1, () =>
            {
                DisplayHighlight(0, 2);
            }, () =>
            {
            }, () =>
            {
                ClearHighlight();
            }));
        }

        if (isAuto || totalFreeSpins > 0 || totalScatter > 2)
        {
            StartCoroutine(DelayAction(2.2f + 0.3f * (bingoList.Count + 1), () =>
            {
                Spin();
            }));

            Debug.Log("totalScatter : " + totalScatter);

            if (totalScatter > 2)
            {
                totalFreeSpins += PayData.totalFreeSpins;
                SetFreeSpins(totalFreeSpins);
                StartCoroutine(DelayAction(1f + 0.3f * (bingoList.Count + 1), () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Tile tile = lineList[baseX + j].items[baseY + i];
                            int type = tile.GetTileType();
                            if (type == scatterIndex)
                            {
                                tile.ShowChoice(0.2f);
                            }
                        }
                    }
                    DisplayFreeSpins(true);
                    DisplayFxTitle(true);
                }));
                StartCoroutine(DelayAction(3f + 0.3f * (bingoList.Count + 1), () =>
                {
                    DisplayFxTitle(false);
                }));
            }
        }

        if (totalScore > 0)
            StartCoroutine(DelayAction(0.1f, () =>
            {
                cashSound.Play();
                winBox.ShowMotion();
                if (totalScore > totalBet) coinDrop.ShowMotion();
            }));

    }

    int highlightCounter = 0;
    bool isHighlight = false;

    void ClearHighlight()
    {
        isHighlight = false;
        highlightCounter = 0;
        highlightLine.SetVisible(false);
    }

    void DisplayHighlight(int baseX, int baseY)
    {
        if (!isHighlight) return;
        if (bingoList.Count < 1) return;

        int i = highlightCounter % bingoList.Count;
        {
            int idx = bingoList[i];
            //int type = typeList[i];
            int count = countList[i];
            //string str = type + " : " + count + " = ";
            List<Vector3> path = new List<Vector3>();
            for (int ln = 0; ln < 5; ln++)
            {
                int pos = PayData.line[idx, ln];
                Tile choiceTile = lineList[ln + baseX].items[pos + baseY];
                path.Add(choiceTile.transform.position);
            }
            highlightLine.path = path.ToArray();
            highlightLine.DrawLines();

            for (int ln = 0; ln < count; ln++)
            {
                int pos = PayData.line[idx, ln];
                Tile choiceTile = lineList[ln + baseX].items[pos + baseY];
                choiceTile.ShowChoice(0.2f);
            }
        }
        highlightCounter++;
    }

    void ClearChoice()
    {
        ClearHighlight();
        foreach (Line lScript in lineList)
            foreach (Tile tScript in lScript.items) tScript.UnSetChoice();
    }

    void Spin()
    {
        if (!isInput) return;
        if (money < totalBet) return;
        ClearChoice();
        DoRoll();
        spinSound.Play();
    }

    public void DoSpin()
    {
        if (isAuto) return;
        Spin();
    }

    public void GoMenu()
    {
        Application.LoadLevel("Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (isInput && Input.GetKeyDown(KeyCode.Return))
        {
            if (!isAuto) Spin();
        }
    }
}
