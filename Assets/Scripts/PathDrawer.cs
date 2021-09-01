﻿using UnityEngine;

public class PathDrawer : MonoBehaviour
{
	public LineRenderer lRenderer;
	private bool active = true;
	private Color32 color;
	private int id;
	private bool mode;

	private void Update()
	{
		if (!active && Input.anyKeyDown)
			ActivateColor();

		if (Input.GetKeyDown(KeyCode.Alpha0))
			mode = !mode;

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			DisableColor();

			if (id == (0 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			DisableColor();

			if (id == (1 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			DisableColor();

			if (id == (2 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			DisableColor();

			if (id == (3 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			DisableColor();

			if (id == (4 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			DisableColor();

			if (id == (5 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			DisableColor();

			if (id == (6 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			DisableColor();

			if (id == (7 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			DisableColor();

			if (id == (8 + (mode ? 9 : 0)))
				ActivateColor();
		}

		if (Input.GetKeyDown(KeyCode.A) && id > 9)
			DisableColor();

		if (Input.GetKeyDown(KeyCode.S) && id <= 9)
			DisableColor();

		if (Input.GetKeyDown(KeyCode.D) && (id % 2) == 0)
			DisableColor();

		if (Input.GetKeyDown(KeyCode.F) && (id % 2) == 1)
			DisableColor();
	}

	public void SetParent(int id, GameObject parent, Color32 color32)
	{
		if (parent == null)
			return;

		this.id = id;
		color = color32;

		Vector3[] positions =
		{
			transform.position, parent.transform.position,
		};

		lRenderer.SetPositions(positions);
		lRenderer.startColor = color32;
		lRenderer.endColor = color32;
	}

	private void DisableColor()
	{
		lRenderer.startColor = new Color32(0xff, 0xff, 0xff, 0x00);
		lRenderer.endColor = new Color32(0xff, 0xff, 0xff, 0x00);
		active = false;
	}

	private void ActivateColor()
	{
		lRenderer.startColor = color;
		lRenderer.endColor = color;
		active = true;
	}
}