﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class menu : MonoBehaviour
{
	public Canvas quitMenu;
	public Button startText;
	public Button exitText;

	// Use this for initialization
	void Start ()
	{
		quitMenu = quitMenu.GetComponent<Canvas> ();
		startText = startText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
		quitMenu.enabled = false;
	}

	public void ExitPress ()
	{
		quitMenu.enabled = true;
		startText.enabled = false;
		exitText.enabled = false;
	}

	public void NoPress ()
	{
		quitMenu.enabled = false;
		startText.enabled = true;
		exitText.enabled = true;
	}

	public void StartLevel ()
	{
		//one for the start level
		Application.LoadLevel (1);
	}
	public void StartLevelsSim()
	{
		//one for the start level
		Application.LoadLevel (2);
	}
	public void ExitGame ()
	{
		Application.Quit ();
	}
}
