﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class INO_Drone : InteractiveObject {
    private const float FLY_SPEED = 1000, FLY_DISTANCE = 1000;
    private const float WING_ROTATE_SPEED = 1000;

    private GameObject wingsObj, wingsMovingObj;
    private GameObject[] movingWingObjs = new GameObject[4];
    private float totalMoveAmount;

    private GameObject floorViewObjectPrefab;

    private enum State {
        IDLE, FLY, END
    };
    private State state = State.IDLE;

    private void Awake() {
        conditionText = "주변에 구조맨 존재";
    }

    protected override void Start() {
        base.Start();

        floorViewObjectPrefab = Resources.Load<GameObject>("Prefabs/Tiles/FloorView");

        // 날개 오브젝트 Load
        wingsObj = transform.Find("Wings").gameObject;
        wingsMovingObj = transform.Find("MovingWings").gameObject;

        wingsObj.SetActive(true);
        wingsMovingObj.SetActive(false);
    }

    private void Update() {
        if (state == State.FLY) {
            float moveAmount = FLY_SPEED * Time.deltaTime;
            totalMoveAmount += moveAmount;

            transform.position += Vector3.up * moveAmount;

            if (totalMoveAmount >= FLY_DISTANCE) {
                state = State.END;

                GetComponent<SpriteRenderer>().enabled = false;
                wingsMovingObj.SetActive(false);

                INO_DroneFloorView floorView = Instantiate(floorViewObjectPrefab).GetComponent<INO_DroneFloorView>();
                floorView.SetFloor(tilePos.z);

                TileMgr.Instance.RemoveDrone(tilePos, floor);
            }
        }
    }

    public override bool IsAvailable() {
        if (!base.IsAvailable()) return false;

        List<Player> players = GameMgr.Instance.GetAroundPlayers(tilePos, floor, 2);
        foreach (Player player in players) {
            if (player.OperatorNumber == Rescuer.OPERATOR_NUMBER)
                return true;
        }
        return false;
    }

    public override void Activate() {
        if (!IsAvailable()) return;
        base.Activate();

        state = State.FLY;
        totalMoveAmount = 0;

        wingsObj.SetActive(false);
        wingsMovingObj.SetActive(true);
    }
}
