using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {

	public int nbBrickX;
	public int nbBrickY;

	public GameObject brick;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < nbBrickY; ++i) {
			for (int j = 0; j < nbBrickX; ++j) {
				genBrick (j,i);
			}
		}
	}


	void genBrick(int i, int j){
		if (j == nbBrickY - 1 && (i == nbBrickX - 1))
			return;
		GameObject clone = Instantiate (brick, transform) as GameObject;
		clone.transform.SetParent (transform);
		clone.transform.name = "Brick_" + i + "x" + j;

		if (j % 2 == 0)
			clone.transform.position = transform.position + new Vector3 (i * brick.transform.lossyScale.x, j * brick.transform.lossyScale.y, 0.0f);
		else
			clone.transform.position = transform.position + new Vector3 (((float)i - 0.5f) * brick.transform.lossyScale.x, (float)j * brick.transform.lossyScale.y, 0.0f);

	}
}
