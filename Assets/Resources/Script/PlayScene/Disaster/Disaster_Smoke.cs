﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disaster_Smoke : DisasterObject {

	private static Sprite[] ObjectSprites = null;
	private const string RESOURCE_PATH = "Sprite/Disaster/Smoke";

	protected override void Start() {
		if (ObjectSprites == null)
			ObjectSprites = Resources.LoadAll<Sprite>(RESOURCE_PATH);

		sprites = ObjectSprites;
		frameInterval = 0.5f;
		base.Start();
	}

	protected override void Active() {
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				Vector3Int targetPos = pos + new Vector3Int(x, y, 0);
				if (TileMgr.Instance.RescueTargets.ContainsKey(targetPos)) {
					TileMgr.Instance.RescueTargets[targetPos].AddO2(-30);
				}
			}
		}

		foreach (Player player in GameMgr.Instance.Comp_Players) {
			if ((player.currentTilePos - pos).magnitude < 2)
				player.AddO2(-30);
		}
	}
}
