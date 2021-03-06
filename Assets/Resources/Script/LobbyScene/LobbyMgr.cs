﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyMgr : MonoBehaviour
{
    public GameObject UICanvas;

    public Text newsContents;

    public GameObject StageInfoPopUp;

    private GameObject argosSystem;
    private GameObject athenaSystem;
    private GameObject eagisSystem;
    private GameObject futureTechSystem;

    private Image argosBtn;
    private Image athenaBtn;
    private Image messengerBtn;
    private Image techBtn;

    private GameObject debriefing;
    private GameObject operatorInfos;
    private GameObject operatorLeftInfo;
    private GameObject operatorRightInfo;
    private GameObject selectToolUI;
    private GameObject selectAbilUI;
    private GameObject banners;

    private Image operatorProfileImage;

    private Text operatorNameText;
    private Text operatorBodyText;
    private Text operatorSpecialText;

    private Text hpText;
    private Text o2Text;
    private Text useO2Text;
    private Text recorveryO2Text;

    private int currentOperatorNum = 0;

    private GameObject techDevUI;
    private GameObject techTestUI;

    private int currentSystemIndex = 0;
    private GameObject currentSystem;
    private Image currentSystemBtn;

    private Sprite[] btnImages;
    private Sprite[] profileImages;

    private GameObject CrossPrefab, SmallCrossPrefab;
    private Dictionary<Tool, Sprite> toolSprites;
    private Image[] info_toolImages;
    private Image[] select_toolImages;
    private Text[] select_toolTexts;
    private Text[] select_toolInfos;
    private int selectToolIndex = 0;

    private Dictionary<Ability, Sprite> abilitySprites;
    private Image[] info_abilImages;
    private Text[] select_abilTitles;
    private Text[] select_abilTexts;
    private Image[,] select_abilImages;
    private Image[,] select_abilLines;

    private GameData gameData = new GameData();
    private readonly Color HIGHLIGHT_COLOR = new Color(217/255f, 54/255f, 21/255f), NORMAL_COLOR = Color.white;
    
    private GameObject[] eagisTalks;
    private GameObject currentEagisTalk;
    private Text eagisTalkerNameText;

    private void Start()
    {
        argosSystem = UICanvas.transform.Find("ArgosSystem").gameObject;
        athenaSystem = UICanvas.transform.Find("AthenaSystem").gameObject;
        eagisSystem = UICanvas.transform.Find("EagisSystem").gameObject;
        futureTechSystem = UICanvas.transform.Find("FutureTechSystem").gameObject;

        Transform Btns = UICanvas.transform.Find("Btns");

        argosBtn = Btns.Find("ArgosBtn").GetComponent<Image>();
        athenaBtn = Btns.Find("AthenaBtn").GetComponent<Image>();
        messengerBtn = Btns.Find("MessengerBtn").GetComponent<Image>();
        techBtn = Btns.Find("TechBtn").GetComponent<Image>();

        debriefing = athenaSystem.transform.Find("Debriefing").gameObject;
        operatorInfos = athenaSystem.transform.Find("OperatorInfo").gameObject;
        operatorLeftInfo = operatorInfos.transform.Find("LeftInfo").gameObject;
        operatorRightInfo = operatorInfos.transform.Find("RightInfo").gameObject;
        banners = athenaSystem.transform.Find("SelectBanner").gameObject;

        operatorProfileImage = operatorLeftInfo.transform.Find("ProfileImage").GetComponent<Image>();
        operatorNameText = operatorLeftInfo.transform.Find("OperatorName").GetComponent<Text>();
        operatorBodyText = operatorLeftInfo.transform.Find("OperatorBody").GetComponent<Text>();
        operatorSpecialText = operatorLeftInfo.transform.Find("OperatorSpecial").GetComponent<Text>();

        hpText = operatorRightInfo.transform.Find("HP").GetComponent<Text>();
        o2Text = operatorRightInfo.transform.Find("O2").GetComponent<Text>();
        useO2Text = operatorRightInfo.transform.Find("UseO2").GetComponent<Text>();
        recorveryO2Text = operatorRightInfo.transform.Find("RecorveryO2").GetComponent<Text>();

        selectToolUI = operatorInfos.transform.Find("SelectTool").gameObject;
        selectAbilUI = operatorInfos.transform.Find("SelectAbility").gameObject;

        techDevUI = futureTechSystem.transform.Find("TechDev").gameObject;
        techTestUI = futureTechSystem.transform.Find("TechTest").gameObject;

        // 도구
        info_toolImages = new Image[2] {
            operatorRightInfo.transform.Find("Tool0").Find("Tool").GetComponent<Image>(),
            operatorRightInfo.transform.Find("Tool1").Find("Tool").GetComponent<Image>()
        };
        select_toolImages = new Image[2] {
            selectToolUI.transform.Find("ToolImage0").Find("Tool").GetComponent<Image>(),
            selectToolUI.transform.Find("ToolImage1").Find("Tool").GetComponent<Image>()
        };
        select_toolTexts = new Text[2] {
            selectToolUI.transform.Find("ToolText0").GetComponent<Text>(),
            selectToolUI.transform.Find("ToolText1").GetComponent<Text>()
        };
        select_toolInfos = new Text[2] {
            selectToolUI.transform.Find("SelectedToolInfo").Find("ToolInfo0").GetComponent<Text>(),
            selectToolUI.transform.Find("SelectedToolInfo").Find("ToolInfo1").GetComponent<Text>()
        };
        for (int i = 0; i < 2; i++)
            select_toolTexts[i].color = NORMAL_COLOR;

        // 특성
        CrossPrefab = Resources.Load<GameObject>("Prefabs/UI/Cross");
        SmallCrossPrefab = Resources.Load<GameObject>("Prefabs/UI/SmallCross");

        info_abilImages = new Image[4];
        select_abilTitles = new Text[4];
        select_abilTexts = new Text[4];
        select_abilImages = new Image[4, 2];
        select_abilLines = new Image[4, 2];

        for (int i = 0; i < 4; i++) {
            info_abilImages[i] = operatorRightInfo.transform.Find(string.Format("Abil{0}", i)).GetComponent<Image>();
            select_abilTitles[i] = selectAbilUI.transform.Find("SelectedAbilInfo").Find(string.Format("Abil{0}", i)).GetComponent<Text>();
            select_abilTexts[i] = selectAbilUI.transform.Find("SelectedAbilInfo").Find(string.Format("AbilInfo{0}", i)).GetComponent<Text>();

            for (int j = 0; j < 2; j++) {
                select_abilImages[i, j] = selectAbilUI.transform.Find("AbilTree").Find(string.Format("AbilImage{0}-{1}", i, j)).GetComponent<Image>();
                select_abilLines[i, j] = selectAbilUI.transform.Find("AbilTree").Find(string.Format("AbilLine{0}-{1}", i, j)).GetComponent<Image>();

                int level = i, index = j;
                select_abilImages[i, j].GetComponent<Button>().onClick.AddListener(() => ChangeAbility(level, index));
            }

            // 미클리어 레벨 특성 X 표시
            if (!gameData.IsCleared(i)) {
                Instantiate(CrossPrefab, info_abilImages[i].transform.position, Quaternion.identity, info_abilImages[i].transform);
                Instantiate(SmallCrossPrefab, select_abilImages[i, 0].transform.position, Quaternion.identity, select_abilImages[i, 0].transform);
                Instantiate(SmallCrossPrefab, select_abilImages[i, 1].transform.position, Quaternion.identity, select_abilImages[i, 1].transform);
            }
        }
        for (int i = 0; i < 3; i++) {
            Image select_abilCenterLine = selectAbilUI.transform.Find("AbilTree").Find(string.Format("AbilLine{0}", i)).GetComponent<Image>();
            select_abilCenterLine.color = (gameData.IsCleared(i+1)) ? HIGHLIGHT_COLOR : NORMAL_COLOR;
        }


        btnImages = new Sprite[12];

        btnImages[0] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/01.Argos_Non select");
        btnImages[1] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/01.Argos_Non select_notice");
        btnImages[2] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/01.Argos_select");
        btnImages[3] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/02.Athena_Non select");
        btnImages[4] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/02.Athena_Non select_notice");
        btnImages[5] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/02.Athena_select");
        btnImages[6] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/03.Messenger_Non select");
        btnImages[7] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/03.Messenger_Non select_notice");
        btnImages[8] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/03.Messenger_select");
        btnImages[9] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/04.Technology_Non select");
        btnImages[10] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/04.Technology_Non select_notice");
        btnImages[11] = Resources.Load<Sprite>("Sprite/LobbyScene/Icon/04.Technology_select");

        profileImages = new Sprite[4];

        profileImages[0] = Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Athena-02_Operator Information-profile-01");
        profileImages[1] = Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Athena-02_Operator Information-profile-02");
        profileImages[2] = Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Athena-02_Operator Information-profile-03");
        profileImages[3] = Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Athena-02_Operator Information-profile-04");

        toolSprites = new Dictionary<Tool, Sprite>();
        toolSprites.Add(Tool.FIREWALL, Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/ToolIcon/Firewall1"));
        toolSprites.Add(Tool.FIRE_EX, Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/ToolIcon/FireExtinguisher1"));
        toolSprites.Add(Tool.FLARE, Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/ToolIcon/Flare1"));
        toolSprites.Add(Tool.O2_CAN, Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/ToolIcon/OxygenRespirator1"));
        toolSprites.Add(Tool.STICKY_BOMB, Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/ToolIcon/StickyBomb1"));

        abilitySprites = new Dictionary<Ability, Sprite>();
        abilitySprites.Add(Ability.None,            Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Athena-02_Operator Information-Icon box"));
        abilitySprites.Add(Ability.Cardio,          Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Cardio"));
        abilitySprites.Add(Ability.DoubleHeart,     Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Double heart"));
        abilitySprites.Add(Ability.EquipMini,       Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Equipment miniaturization"));
        abilitySprites.Add(Ability.Fitness,         Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Fitness"));
        abilitySprites.Add(Ability.IntervalTrain,   Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Interval training"));
        abilitySprites.Add(Ability.MacGyver,        Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/MacGyver"));
        abilitySprites.Add(Ability.Nightingale,     Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Nightingale"));
        abilitySprites.Add(Ability.OxygenTank,      Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Oxygen tank"));
        abilitySprites.Add(Ability.PartsLightweight,Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Parts lightweight"));
        abilitySprites.Add(Ability.RedundancyOxygen,Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Redundancy Oxygen"));
        abilitySprites.Add(Ability.Refuel,          Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Refuel"));
        abilitySprites.Add(Ability.ReinforceParts,  Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Reinforce parts"));
        abilitySprites.Add(Ability.SteelBody,       Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Steel body"));
        abilitySprites.Add(Ability.SurvivalPriority,Resources.Load<Sprite>("Sprite/LobbyScene/02_Athena_System/02_Operator Information/Icon/Survival Priority"));

        currentSystemIndex = 0;
        currentSystem = argosSystem;
        currentSystemBtn = argosBtn;

        eagisTalks = new GameObject[4];
        for(int i=0; i<eagisTalks.Length; i++)
        {
            eagisTalks[i] = eagisSystem.transform.Find("Talk" + i).Find("Talks").gameObject;
        }
        currentEagisTalk = eagisTalks[0];
        eagisTalkerNameText = eagisSystem.transform.Find("TalkerName").GetComponent<Text>();
    }

    private void Update()
    {
        NewsFlow();
    }

    void NewsFlow()
    {
        newsContents.rectTransform.Translate(-1.0f * Time.deltaTime * 15, 0, 0);
        if (newsContents.rectTransform.position.x < -250.0f)
            newsContents.rectTransform.position = new Vector3(1160.0f, 1053, 0);
    }

    public void SystemSelectBtn(int index)
    {
        if (currentSystemIndex == index)
            return;
        currentSystem.SetActive(false);
        switch(currentSystemIndex)
        {
            case 0:
                currentSystemBtn.sprite = btnImages[0];
                StageInfoPopUp.SetActive(false);
                break;
            case 1:
                debriefing.SetActive(false);
                operatorInfos.SetActive(false);
                banners.SetActive(false);
                currentSystemBtn.sprite = btnImages[3];
                break;
            case 2:
                currentSystemBtn.sprite = btnImages[6];
                break;
            case 3:
                currentSystemBtn.sprite = btnImages[9];
                break;
        }
        switch (index)
        {
            case 0:
                currentSystem = argosSystem;
                currentSystemBtn = argosBtn;
                currentSystemBtn.sprite = btnImages[2];
                break;
            case 1:
                currentSystem = athenaSystem;
                currentSystemBtn = athenaBtn;
                banners.SetActive(true);
                currentSystemBtn.sprite = btnImages[5];
                break;
            case 2:
                currentSystem = eagisSystem;
                currentSystemBtn = messengerBtn;
                currentSystemBtn.sprite = btnImages[8];
                break;
            case 3:
                currentSystem = futureTechSystem;
                currentSystemBtn = techBtn;
                currentSystemBtn.sprite = btnImages[11];
                break;
        }
        currentSystemIndex = index;
        currentSystem.SetActive(true);
    }

    public void SwapUI(string btnName)
    {
        switch (btnName)
        {
            case "Debriefing":
                debriefing.SetActive(true);
                banners.SetActive(false);
                break;
            case "OperatorInfo":
                operatorInfos.SetActive(true);
                operatorRightInfo.SetActive(true);
                selectAbilUI.SetActive(false);
                selectToolUI.SetActive(false);
                banners.SetActive(false);
                
                LoadToolIcon();
                LoadAbilityIcon();
                break;
            case "SelectTool":
                operatorRightInfo.SetActive(false);
                selectAbilUI.SetActive(false);
                selectToolUI.SetActive(true);

                selectToolIndex = -1;
                break;
            case "SelectAbil":
                operatorRightInfo.SetActive(false);
                selectAbilUI.SetActive(true);
                selectToolUI.SetActive(false);
                break;
            case "TechDev":
                techTestUI.SetActive(false);
                techDevUI.SetActive(true);
                break;
            case "TechTest":
                techDevUI.SetActive(false);
                techTestUI.SetActive(true);
                break;
        }
    }

    public void EnableStageInfoPopUp()
    {
        StageInfoPopUp.SetActive(true);
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene("Scenes/PlayScene");
    }

    public void ChangeOperatorInfo(bool isLeft)
    {
        if (isLeft && currentOperatorNum > 0)
        {
            currentOperatorNum--;
        }
        else if(!isLeft && currentOperatorNum < 3)
        {
            currentOperatorNum++;
        }
        switch (currentOperatorNum)
        {
            case 0:
                operatorNameText.text = "01. 신화준";
                operatorBodyText.text = "175cm 73kg";
                operatorSpecialText.text = "(구)소방청 출신\n" +
                    "소방청 근무 당시 뛰어난 활약을 보임\n" +
                    "과묵하고 신중한 성격을 가짐\n" +
                    "팀원에 대한 책임감이 강함";
                o2Text.text = "55";
                hpText.text = "30";
                useO2Text.text = "2";
                recorveryO2Text.text = "20";
                break;
            case 1:
                operatorNameText.text = "02. 빅토르";
                operatorBodyText.text = "195cm 145kg";
                operatorSpecialText.text = "(구)소방청 출신\n" +
                    "20세 때 불곰과 싸워 이겼다는 소문이 있음\n" +
                    "러시아 삼보 청소년 국가대표 출신\n" +
                    "술을 좋아하나 잘 못하니 주의가 필요";
                o2Text.text = "60";
                hpText.text = "70";
                useO2Text.text = "2";
                recorveryO2Text.text = "20";
                break;
            case 2:
                operatorNameText.text = "03. 레    오";
                operatorBodyText.text = "176cm 65kg";
                operatorSpecialText.text = "이지스 출신\n" +
                    "레트로한 일본 전자기기를 선호함\n" +
                    "사고로 부모님과 팔을 잃음 여동생과 거주중\n" +
                    "성격이 쾌활하며 붙임성이 좋다";
                o2Text.text = "70";
                hpText.text = "80";
                useO2Text.text = "3";
                recorveryO2Text.text = "25";
                break;
            case 3:
                operatorNameText.text = "04. 시노에";
                operatorBodyText.text = "167cm 54kg";
                operatorSpecialText.text = "대형 병원 의사 출신\n" +
                    "소방청 근무 중이던 약혼자가 있었지만 사망함\n" +
                    "이중인격이 의심 됨\n" +
                    "현장에서 매우 민감하게 반응함";
                o2Text.text = "45";
                hpText.text = "60";
                useO2Text.text = "1";
                recorveryO2Text.text = "35";
                break;
        }
        operatorProfileImage.sprite = profileImages[currentOperatorNum];

        LoadToolIcon();
        LoadAbilityIcon();
        selectToolIndex = -1;
    }

    private void LoadAbilityIcon() {
        for (int i = 0; i < 4; i++) {
            // 특성 이미지 적용
            for (int j = 0; j < 2; j++) {
                Ability ability = PlayerAbilityMgr.GetAbility(currentOperatorNum, i, j);
                select_abilImages[i, j].sprite = abilitySprites[ability];
            }

            // 선택된 특성 강조
            int selectedIndex = gameData.GetAbilityIndex(currentOperatorNum, i);
            if (!gameData.IsCleared(i) || selectedIndex == -1) {
                select_abilImages[i, 0].color = NORMAL_COLOR;
                select_abilImages[i, 1].color = NORMAL_COLOR;
                select_abilLines[i, 0].color = NORMAL_COLOR;
                select_abilLines[i, 1].color = NORMAL_COLOR;

                select_abilTitles[i].text = (gameData.IsCleared(i)) ? PlayerAbilityMgr.ToString(Ability.None) : "";
                select_abilTexts[i].text = "";

                info_abilImages[i].sprite = abilitySprites[Ability.None];
            }
            else {
                Ability ability = PlayerAbilityMgr.GetAbility(currentOperatorNum, i, selectedIndex);
                select_abilImages[i, selectedIndex].color = HIGHLIGHT_COLOR;
                select_abilLines[i, selectedIndex].color = HIGHLIGHT_COLOR;
                info_abilImages[i].sprite = abilitySprites[ability];

                select_abilTitles[i].text = PlayerAbilityMgr.ToString(ability);
                select_abilTexts[i].text = PlayerAbilityMgr.GetInfo(ability);

                int otherIndex = (selectedIndex + 1) % 2;
                select_abilImages[i, otherIndex].color = NORMAL_COLOR;
                select_abilLines[i, otherIndex].color = NORMAL_COLOR;
            }
        }

    }
    public void ChangeAbility(int level, int index) {
        if (!gameData.IsCleared(level)) return;

        gameData.SetAbilityIndex(currentOperatorNum, level, index);
        gameData.Save();

        LoadAbilityIcon();
    }

    private void LoadToolIcon() {
        for (int i = 0; i < 2; i++) {
            Tool tool = gameData.GetTool(currentOperatorNum, i);
            info_toolImages[i].sprite = toolSprites[tool];
            select_toolImages[i].sprite = toolSprites[tool];
            select_toolTexts[i].text = PlayerToolMgr.ToString(tool);
            select_toolInfos[i].text = PlayerToolMgr.ToString(tool) + " | " + PlayerToolMgr.GetInfo(tool);
            select_toolTexts[i].color = NORMAL_COLOR;
        }
    }
    public void ChangeToolIndex(int index) {
        if (selectToolIndex != -1)
            LoadToolIcon();

        if (selectToolIndex == index) {
            selectToolIndex = -1;
            return;
        }

        selectToolIndex = index;
        select_toolTexts[selectToolIndex].text = "선택 중";
        select_toolTexts[selectToolIndex].color = HIGHLIGHT_COLOR;
    }
    public void ChangeTool(string toolName) {
        if (selectToolIndex == -1) return;

        Tool tool = Tool.FIREWALL;
        switch (toolName) {
        case "FIREWALL":    tool = Tool.FIREWALL;   break;
        case "FIRE_EX":     tool = Tool.FIRE_EX;    break;
        case "FLARE":       tool = Tool.FLARE;      break;
        case "O2_CAN":      tool = Tool.O2_CAN;     break;
        case "STICKY_BOMB": tool = Tool.STICKY_BOMB;break;
        }

        int otherIndex = (selectToolIndex+1) % 2;
        if (gameData.GetTool(currentOperatorNum, otherIndex) == tool)
            gameData.SetTool(currentOperatorNum, otherIndex, gameData.GetTool(currentOperatorNum, selectToolIndex));

        select_toolTexts[selectToolIndex].color = NORMAL_COLOR;

        gameData.SetTool(currentOperatorNum, selectToolIndex, tool);
        gameData.Save();

        LoadToolIcon();
        selectToolIndex = -1;
    }

    public void ChangeEagisTalker(int index)
    {
        currentEagisTalk.SetActive(false);
        currentEagisTalk = eagisTalks[index];
        currentEagisTalk.SetActive(true);
        switch(index)
        {
            case 0:
                eagisTalkerNameText.text = "# 빅 토 르";
                break;
            case 1:
                eagisTalkerNameText.text = "# 시 노 에";
                break;
            case 2:
                eagisTalkerNameText.text = "# 레    오";
                break;
            case 3:
                eagisTalkerNameText.text = "# 선 배 님";
                break;
        }
    }
}