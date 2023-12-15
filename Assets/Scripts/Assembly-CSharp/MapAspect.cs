using System;
using UnityEngine;

public class MapAspect : MonoBehaviour
{
	public enum Type
	{
		Forest = 0,
		WhiteForest = 1,
		Hills = 2,
		Earth =3
	}

	[Serializable]
	public class Aspect
	{
		public Sprite foreground;

		public Texture2D[] backgrounds;

		public Color cameraColor;

		public IAudioEvent ambientEvents;

		public AudioClip ambientLoop;

		public float groundHeight;
	}

	public Aspect[] aspects;

	[Header("References")]
	public Renderer[] m_backgroundsMaterials;

	public SpriteRenderer foreground;

	private Camera m_camera;

	private Vector3 m_initForegroundPosition;

	private void Awake()
	{
		m_camera = UnityEngine.Object.FindObjectOfType<Camera>();
		m_initForegroundPosition = foreground.transform.position;
	}

	public void ApplyAspect(Type type)
	{
		Aspect aspect = aspects[(int)type];
		for (int i = 0; i < m_backgroundsMaterials.Length; i++)
		{
			m_backgroundsMaterials[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < aspect.backgrounds.Length; j++)
		{
			m_backgroundsMaterials[j].gameObject.SetActive(true);
			m_backgroundsMaterials[j].material.mainTexture = aspect.backgrounds[j];
		}
		m_camera.backgroundColor = aspect.cameraColor;
		foreground.sprite = aspect.foreground;
		foreground.transform.position = m_initForegroundPosition + Vector3.up * aspect.groundHeight;
		AudioController.SetSourceClip(3, aspect.ambientLoop);
	}
}
