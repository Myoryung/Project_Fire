﻿using System.Collections;
using UnityEngine;

public class HammerMan : Player {

    public const int OPERATOR_NUMBER = 1;
    public override int OperatorNumber {
        get { return OPERATOR_NUMBER; }
    }
    protected override void Awake()
    {
        base.Awake();
        cutSceneIlust = Resources.Load<Sprite>("Sprite/PlayScene/UI/CutScene/Ultimate_hammer_man");
        ultName = "아직 멈출 수 없다";
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void ActiveSkill() {
        if (CurrentO2 >= GetSkillUseO2())
            StartCoroutine(RescueHammer()); // 스킬 발동
    }

    public override void ActiveUltSkill()
    {
        base.ActiveUltSkill();
        if (isUsedUlt)
            return;
        Action oldact = playerAct;
        StartCoroutine(ShowCutScene());
        AddO2(50.0f);
        playerAct = oldact;
        isUsedUlt = true;
    }

    IEnumerator RescueHammer() {
        Vector3Int oPos = currentTilePos; // 갱신용 old Pos
        UI_Actives.SetActive(false); // UI 숨기기
        _anim.SetBool("IsUsingActive", true);
        
        while (true) { // 클릭 작용시까지 반복
            RenderInteractArea(ref oPos);
            if (Input.GetMouseButtonDown(0))
            {
                if (TileMgr.Instance.ExistTempWall(oPos, floor))
                { // 클릭 좌표에 장애물이 있다면 제거
                    SoundManager.instance.PlayWallCrash();
                    _anim.SetTrigger("ActiveSkillTrigger");
                    yield return new WaitForSeconds(1.7f);
                    TileMgr.Instance.RemoveTempWall(oPos, floor);
                    overcomeTraumaCount++;
                    if (overcomeTraumaCount >= 5)
                        isOverComeTrauma = true;
                    AddO2(-GetSkillUseO2());
                    if (isOverComeTrauma)
                        AddO2(10.0f);
                    else if (GameMgr.Instance.GetSurvivorAt(oPos + (oPos - TileMgr.Instance.WorldToCell(transform.position, floor)), floor))
                        playerAct = Action.Panic; // 턴제한 추가 필요
                }
                break;
            }
            else if (IsMoving) {
                break;
            }
            yield return null;
        }
        _anim.SetBool("IsUsingActive", false);

        TileMgr.Instance.RemoveEffect(oPos, floor);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        switch (other.tag)//해머맨 피격소리 재생
        {
            case "Fire":
                SoundManager.instance.PlayHammermanHurt();
                break;
            case "Ember":
                SoundManager.instance.PlayHammermanHurt();
                break;
            case "Electric":
            case "Water(Electric)":
                SoundManager.instance.PlayHammermanHurt();
                break;
        }
    }
}