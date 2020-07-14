using UnityEngine;
namespace ZZA {
public class Rotator : MonoBehaviour
{
        private static float COUNTS = 0.04f;
    private float delayCounter;
	public float rotateX, rotateY, rotateZ;

    private void Start()
    {
            delayCounter = COUNTS;
    }
	private void Update()
	{
        delayCounter -= Time.deltaTime;
        if (delayCounter < 0 )
        {
                this.transform.Rotate(rotateX * 0.8f, rotateY * 0.8f, rotateZ * 0.8f);
            delayCounter = COUNTS;
        }

	}

}
}