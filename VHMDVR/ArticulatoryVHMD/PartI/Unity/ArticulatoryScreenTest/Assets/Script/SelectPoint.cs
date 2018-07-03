using UnityEngine;
using System.Collections;

public class SelectPoint : MonoBehaviour {
    
    GameObject[] Target = new GameObject[10];
    string[] targetnames = new string[] { "C1L4", "C2L1", "C3L3", "C4L2", "C5L1", "C6L4", "C7L1", "C8L3", "C9L2", "C10L4" };

    public bool presskey = false;
    bool somethingslctd = false;
    RaycastHit hit;
    Vector3 pointingnorth = new Vector3(0, 0, 10);
    public GameObject pointnow = null;
    GameObject previouspoint = null;

    // Use this for initialization
    void Start () {
        targetnames = MyFisherYatesShffl(targetnames);
        for (int i = 0; i < Target.Length; i++) {
            Target[i] = GameObject.Find(targetnames[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if ((Input.GetKey(KeyCode.KeypadEnter) || Input.GetButton("Fire1")) && !presskey) { 
            presskey = true;
            somethingslctd = true;
            Select(pointnow);
        }

        if(!somethingslctd)
            LookingAt();
    }

    public static string[] MyFisherYatesShffl(string[] StrArray) {
        //Fisher–Yates Shuffle variation
        int ArrayLength = StrArray.Length;

        for (int i = 0; i < ArrayLength; i++) {
            string temp = StrArray[i];
            int randomIndex = UnityEngine.Random.Range(i, ArrayLength - 1);
            if ((i > 0) && (i < ArrayLength - 2)) {
                if (StrArray[i - 1] == StrArray[randomIndex])
                    randomIndex++;
            }
            StrArray[i] = StrArray[randomIndex];
            StrArray[randomIndex] = temp;
        }
        return StrArray;
    }

    void LookingAt() {
        if (Physics.Raycast(transform.position, transform.TransformDirection(pointingnorth), out hit)) {
            pointnow = GameObject.Find(hit.collider.name);

            if (pointnow.tag == "Selectable") {
                pointnow.GetComponent<Renderer>().material.color = Color.yellow;
                if (previouspoint != pointnow && previouspoint != null) MovingOn();
                previouspoint = pointnow;
            } else {
                if (previouspoint != null) MovingOn();
            }
        }
    }

    void MovingOn() {
        previouspoint.GetComponent<Renderer>().material.color = Color.white;
    }

    void Select(GameObject point) {
        print(point.name);
        point.GetComponent<Renderer>().material.color = Color.grey;
    }

    void Deselect() { }
}
