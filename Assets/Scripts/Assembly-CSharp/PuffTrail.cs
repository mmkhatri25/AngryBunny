using UnityEngine;

public class PuffTrail : MonoBehaviour
{
	[SerializeField]
	private float m_interval = 0.1f;

	[SerializeField]
	private Vector2 m_sizeRange;

	[SerializeField]
	private ParticleSystem m_particleSystem;

	private ParticleSystem.Particle[] m_particles;

	private float timer;

	private int m_index;

	private int m_count;

	private const int SIZE = 100;

	public ParticleSystem System
	{
		get
		{
			return m_particleSystem;
		}
	}

	private void Awake()
	{
		m_particles = new ParticleSystem.Particle[100];
	}

	private void Start()
	{
		AddParticle();
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > m_interval)
		{
			timer = 0f;
			AddParticle();
		}
	}

	private void AddParticle()
	{
		m_particles[m_index].position = base.transform.position;
		m_particles[m_index].startSize = Random.Range(m_sizeRange.x, m_sizeRange.y);
		m_particles[m_index].startColor = Color.white;
		m_index = ((m_index < 99) ? (m_index + 1) : 0);
		m_count = ((m_count < 99) ? (m_count + 1) : m_count);
		m_particleSystem.SetParticles(m_particles, m_count);
	}
}
