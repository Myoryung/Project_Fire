﻿using UnityEngine;

public class Rescuer : Player {

    public const int OPERATOR_NUMBER = 2;
    public override int OperatorNumber {
        get { return OPERATOR_NUMBER; }
    }

    [SerializeField]
    private GameObject robotDogPrefab = null;
    private GameObject robotDog = null;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        cutSceneIlust = Resources.Load<Sprite>("Sprite/PlayScene/UI/CutScene/Ultimate_engineer");
        robotDog = Instantiate<GameObject>(robotDogPrefab, transform.position, Quaternion.identity);
        //강아지 여러번 생성되는거 방지해야함
        ultName = "도와줘 멍멍아";
    }
    protected override void Start()
    {
        base.Start();
        //강아지 할당 robotDog
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TurnEndActive()
    {
        base.TurnEndActive();
        if (isOverComeTrauma)
            AddO2(5.0f);
        else if (GameMgr.Instance.GetAroundPlayerCount(currentTilePos, floor, 3) <= 0)
            AddO2(-10.0f);
    }

    public override void ActiveSkill() {
        if (CurrentO2 < GetSkillUseO2())
            return;

        int range = 5;
        int offset = -(range / 2);
        Vector3Int nPos = TileMgr.Instance.WorldToCell(transform.position, floor);
        for(int i = offset; i < range + offset; i++) {
            for(int j = offset; j < range + offset; j++) {
                Vector3Int targetPos = nPos + new Vector3Int(i, j, 0);
                Survivor survivor = GameMgr.Instance.GetSurvivorAt(targetPos, floor);
                if (survivor != null)
                    survivor.ActiveSmileMark();
            }
        }

        AddO2(-GetSkillUseO2());
    }

    public override void ActiveUltSkill()
    {
        base.ActiveUltSkill();
        if (isUsedUlt)
            return;
        if (TileMgr.Instance.ExistObject(_currentTilePos + Vector3Int.right, floor))
        {
            return;
        }
        Action oldact = playerAct;
        StartCoroutine(ShowCutScene());
        SpawnRobotDog();
        playerAct = oldact;
        isUsedUlt = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    private void SpawnRobotDog()
    {
        robotDog.transform.position = TileMgr.Instance.CellToWorld(_currentTilePos + Vector3Int.right, floor);
        GameMgr.Instance.InsertRobotDogInPlayerList(robotDog.GetComponent<RobotDog>(), this);
        robotDog.SetActive(true);
    }
}