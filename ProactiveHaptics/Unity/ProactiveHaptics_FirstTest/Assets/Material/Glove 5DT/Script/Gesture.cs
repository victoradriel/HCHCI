using UnityEngine;

public class Gesture : MonoBehaviour {

	private float[] data;

	public Gesture ()
	{
		data = null;
	}


	public Gesture (float[] pdata)
	{
		data = new float[pdata.Length];
		pdata.CopyTo (data, 0);
	}

	public void SetData(float [] pdata)
	{
		if (data != null) {
			data = null;		
		}

		data = new float[pdata.Length];
		pdata.CopyTo (data, 0);
	}

	public bool IsEqual(Gesture other, float threshold)
	{
		if (data.Length == other.data.Length) {
			int i;
			for(i=0;i<data.Length;i++){
				if(System.Math.Abs(data[i]-other.data[i])>threshold)
					return false;
			}
			return true;
				} else {
						return false;
				}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
