﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Disaster_Flashover : DisasterObject {

	private static Sprite[] ObjectSprites = null;
	private const string RESOURCE_PATH = "Sprite/Disaster/Flashover";

	[SerializeField]
	private TileBase fireTile = null;

	protected override void Start() {
		if (ObjectSprites == null)
			ObjectSprites = Resources.LoadAll<Sprite>(RESOURCE_PATH);

		sprites = ObjectSprites;
		frameInterval = 0.5f;
		base.Start();
	}

	protected override void Active() {
		for (int x = 0; x <= 1; x++) {
			for (int y = 0; y <= 1; y++) {
				Vector3Int targetPos = pos + new Vector3Int(x, y, 0);
				if (GameMgr.Instance.Obstacle.GetTile(targetPos) == null)
					GameMgr.Instance.Obstacle.SetTile(targetPos, fireTile);
			}
		}
	}
}
