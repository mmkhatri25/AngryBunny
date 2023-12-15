using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Angry Bunnies/Projectile")]
public class Projectile : ScriptableObject
{
	public enum ProjectileType
	{
		GreyBunny = 0,
		YellowBunny = 1,
		PinkBunny = 2
	}

	public ProjectileType type;

	public GameObject prefab;
}
