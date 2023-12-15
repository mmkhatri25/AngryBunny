using System;
using Localization;
using UnityEngine;

[RequireComponent(typeof(ProjectileController))]
public class ProjectileAbility : MonoBehaviour
{
	[Serializable]
	public struct TutorialStep
	{
		public string messageKey;
	}

	[SerializeField]
	private TutorialStep[] m_tutorialSteps;

	[SerializeField]
	protected int m_applicationSteps;

	[SerializeField]
	protected ProjectileController.State m_applyInState;

	[SerializeField]
	protected ProjectileController.State m_tutorialInState;

	[SerializeField]
	protected bool m_tapToApply;

	[SerializeField]
	protected bool m_tutorialEndApply;

	[SerializeField]
	protected float m_tutorialEntryDelay;

	protected ProjectileController m_controller;

	private PopupMessagePositiveAction m_tutorialPopup;

	private int m_currentStep;

	private int m_applicationStep;

	private static GameplayController m_gameplayController;

	public string tutorialSaveKey = "xaczffvss";

	private static GameplayController GameplayController
	{
		get
		{
			if (!m_gameplayController)
			{
				m_gameplayController = UnityEngine.Object.FindObjectOfType<GameplayController>();
			}
			return m_gameplayController;
		}
	}

	protected bool TutorialDone
	{
		get
		{
			return SaveManager.LoadInt(tutorialSaveKey) == 1;
		}
		set
		{
			SaveManager.SaveInt(tutorialSaveKey, value ? 1 : 0);
		}
	}

	private void Awake()
	{
		Setup();
	}

	protected virtual void Setup()
	{
		m_controller = GetComponent<ProjectileController>();
		ProjectileController controller = m_controller;
		controller.OnStateChanged = (ProjectileController.StateEvent)Delegate.Combine(controller.OnStateChanged, new ProjectileController.StateEvent(ControllerStateChanged));
	}

	protected virtual void ControllerStateChanged(ProjectileController.State state)
	{
		if (state == m_tutorialInState && !TutorialDone)
		{
			Invoke("ShowTutorial", m_tutorialEntryDelay);
		}
		if (state == m_applyInState)
		{
			Apply();
		}
	}

	protected virtual void ShowTutorial()
	{
		TutorialDone = true;
		PopupFactory.PopupInfo popupInfo = new PopupFactory.PopupInfo(PopupFactory.PopupType.MessageTutorial, string.Empty, LanguageManager.Get(m_tutorialSteps[m_currentStep].messageKey));
		m_tutorialPopup = (PopupMessagePositiveAction)PopupFactory.instance.CreatePopup(popupInfo);
		m_tutorialPopup.positiveActionButton.onClick.RemoveAllListeners();
		m_tutorialPopup.positiveActionButton.onClick.AddListener(NextTutorialStep);
		m_tutorialPopup.positiveActionButton.gameObject.SetActive(!m_tapToApply);
	}

	protected virtual void NextTutorialStep()
	{
		m_currentStep++;
		if (m_currentStep >= m_tutorialSteps.Length)
		{
			m_tutorialPopup.Hide();
			if (m_tutorialEndApply)
			{
				Apply();
			}
		}
		else
		{
			m_tutorialPopup.messageHolder.text = LanguageManager.Get(m_tutorialSteps[m_currentStep].messageKey);
		}
	}

	protected virtual void Apply()
	{
		m_applicationStep++;
		Apply(m_applicationStep);
	}

	private void ResetCamera()
	{
		GameplayController.ResetCameraState = true;
		GameplayController.ResetCamera();
	}

	protected virtual bool Apply(int step)
	{
		if (m_applicationSteps < step)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && m_controller.CurrentState >= ProjectileController.State.Airborne)
		{
			if ((bool)m_tutorialPopup)
			{
				NextTutorialStep();
			}
			else if (m_tapToApply)
			{
				Apply();
			}
		}
	}
}
