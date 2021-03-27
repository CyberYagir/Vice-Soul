using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkTable : MonoBehaviour
{
	public bool triggered;
	public GameObject SATANA, SKELETON;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (triggered)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				var ui = FindObjectOfType<PlayerUI>();
				var stats = FindObjectOfType<PlayerStats>();
				if (stats.localPlayer.inventory[ui.hotBarItems[ui.selectedCase]].secondName == "_DarkBottle_")
				{
					ui.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName("_DarkBottle_"));
					Instantiate(SATANA);
				}
				if (FindObjectOfType<WorldManager>().firstBoss == true)
				{
					if (stats.localPlayer.inventory[ui.hotBarItems[ui.selectedCase]].secondName == "_StHell_")
					{
						ui.RemoveItem(FindObjectOfType<WorldManager>().GetItemBySecondName("_StHell_"));
						Instantiate(SKELETON);
					}
				}
			}
		}
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.transform.tag == "Player")
			triggered = true;

	}
	void OnTriggerExit2D(Collider2D other) {
		if (other.transform.tag == "Player")
			triggered = false;
	}
}
