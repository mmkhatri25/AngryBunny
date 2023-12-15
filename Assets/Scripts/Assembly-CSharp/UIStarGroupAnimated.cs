using System.Collections;
using UnityEngine;

public class UIStarGroupAnimated : UIStarGroup
{
	[SerializeField]
	private float m_delay = 0.1f;

	public PlayerController playercntroler;
	public LevelController levelController;

    private IEnumerator DisplayStar(GameObject anim, float delay)
	{
		float timer = 0f;
		while (timer < delay)
		{
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		anim.SetActive(true);
	}


    public override void SetStatus(int count)
	{

		//int a = playercntroler.ProjectilesParent.transform.childCount;
		//int b = levelController.CurrentLevel.projectileTypes.Count;
		
  //      Debug.Log("total bunny - "+b + " , remaining bunny -  "+a + ", Show " + count + " stars.");
  //      count = CalculateStarRating(b,a);
        print("showing  final star count is - "+ count);
        count = Mathf.Clamp(count, 0, m_stars.Count);
		for (int i = 0; i < m_stars.Count; i++)
		{
			m_stars[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < count; j++)
		{
			StartCoroutine(DisplayStar(m_stars[j].gameObject, (float)j * m_delay));
		}
	}
    public int CalculateStarRating(int bunnyCount, int remainingBunnies)
    {
        int starRating = 0;

        switch (bunnyCount)
        {
            case 1:
                starRating = 3;
                break;

            case 2:
                if (remainingBunnies == 1) starRating = 3;
                else if (remainingBunnies == 0) starRating = 2;
                break;
            case 3:
                if (remainingBunnies == 0) starRating = 1;
                else if (remainingBunnies == 1) starRating = 2;
                else if (remainingBunnies == 2) starRating = 3;
                break;
            case 4:
                if (remainingBunnies == 2 || remainingBunnies == 3) starRating = 3;
                else if (remainingBunnies == 1) starRating = 2;
                else if (remainingBunnies == 0) starRating = 1;
                break;
            case 5:
                if (remainingBunnies == 3 || remainingBunnies == 4) starRating = 3;
                else if (remainingBunnies == 1 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 0) starRating = 1;
                break;
            case 6:
                if (remainingBunnies == 5 || remainingBunnies == 4) starRating = 3;
                else if (remainingBunnies == 1 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 0 ) starRating = 1;
                break;
            case 7:
                if (remainingBunnies == 6 || remainingBunnies == 5) starRating = 3;
                else if (remainingBunnies == 4 || remainingBunnies == 3 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 1 || remainingBunnies == 0) starRating = 1;
                break;
            case 8:
                if (remainingBunnies == 7 || remainingBunnies == 6 || remainingBunnies == 5) starRating = 3;
                else if (remainingBunnies == 4 || remainingBunnies == 3 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 1 || remainingBunnies == 0) starRating = 1;
                break;
        }

        return starRating;
    }
}
