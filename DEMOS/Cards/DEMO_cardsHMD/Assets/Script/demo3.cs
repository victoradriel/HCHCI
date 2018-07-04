using UnityEngine;
using System.Collections;

public class demo3 : MonoBehaviour {
    RaycastHit hit;
    Vector3 direction = new Vector3(0, 0, 3);

    Vector3 selectSpeed = new Vector3(0, 0, -1F);
    Vector3 deselectSpeed = new Vector3(0, 0, 1F);
    int rot = 0;
    public int lastSelected = 33;

    private float timer = 0f;
    private float totalDuration = 1f;

    public GameObject[] Cards = new GameObject[5];

    Vector3 oldPos0 = new Vector3(7.073F, 2.15F, -8.938F);
    Vector3 newPos0 = new Vector3(7.073F - 0.8F, 2.15F, -8.938F - 0.8F);
    Vector3 oldPos1 = new Vector3(7.563F, 2.15F, -9.994F);
    Vector3 newPos1 = new Vector3(7.563F - 1, 2.15F, -9.994F);
    Vector3 oldPos2 = new Vector3(6.009F, 2.15F, -8.509F);
    Vector3 newPos2 = new Vector3(6.009F, 2.15F, -8.509F - 1);
    Vector3 oldPos3 = new Vector3(4.47F, 2.15F, -10.002F);
    Vector3 newPos3 = new Vector3(4.47F + 1, 2.15F, -10.002F);
    Vector3 oldPos4 = new Vector3(4.946F, 2.15F, -8.916F);
    Vector3 newPos4 = new Vector3(4.946F + 0.8F, 2.15F, -8.916F - 0.8F);

    // Use this for initialization
    void Start() {
        PlayerPrefs.SetInt("Answer", 0);
        Cards[0] = GameObject.Find("CardQ");
        Cards[1] = GameObject.Find("CardJ");
        Cards[2] = GameObject.Find("CardJoker");
        Cards[3] = GameObject.Find("CardK");
        Cards[4] = GameObject.Find("CardA");
    }

    void RotateCard()
    {
        //float t = 0.5F;
        timer += Time.deltaTime;
        float t = timer / totalDuration;

        switch (lastSelected)
        {
            case 0:
                Cards[0].transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 225, 0), t);
                break;
            case 1:
                Cards[1].transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 270, 0), t);
                break;
            case 2:
                Cards[2].transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 180, 0), t);
                PlayerPrefs.SetInt("Answer", 1);
                break;
            case 3:
                Cards[3].transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 270, 0), Quaternion.Euler(0, 90, 0), t);
                break;
            case 4:
                Cards[4].transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 315, 0), Quaternion.Euler(0, 135, 0), Time.time * t);
                break;
        }
    }


    void SelectCard(int Cardidx)
    {

        float t = 0.5F;

        if (rot != 1)
            lastSelected = Cardidx;

        switch (Cardidx)
        {
            case 0:
                Cards[0].transform.position = Vector3.Slerp(oldPos0, newPos0, t);
                break;
            case 1:
                Cards[1].transform.position = Vector3.Slerp(oldPos1, newPos1, t);
                break;
            case 2:
                Cards[2].transform.position = Vector3.Slerp(oldPos2, newPos2, t);
                break;
            case 3:
                Cards[3].transform.position = Vector3.Slerp(oldPos3, newPos3, t);
                break;
            case 4:
                Cards[4].transform.position = Vector3.Slerp(oldPos4, newPos4, t);
                break;
        }
    }

    void DeselectCards()
    {
        float t = 0.5F;
        if (lastSelected != 33)
        {
            switch (lastSelected)
            {
                case 0:
                    Cards[0].transform.position = oldPos0;
                    lastSelected = 33;
                    break;
                case 1:
                    Cards[1].transform.position = oldPos1;
                    lastSelected = 33;
                    break;
                case 2:
                    Cards[2].transform.position = oldPos2;
                    lastSelected = 33;
                    break;
                case 3:
                    Cards[3].transform.position = oldPos3;
                    lastSelected = 33;
                    break;
                case 4:
                    Cards[4].transform.position = oldPos4;
                    lastSelected = 33;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetButton("Fire1")) {
            rot = 1;
            PlayerPrefs.SetInt("Selected", 1);
        }

        if (rot == 1)
            RotateCard();


        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit))
        {
            if (hit.collider.name == "CardQ")
                SelectCard(0);
            else if (hit.collider.name == "CardJ")
                SelectCard(1);
            else if (hit.collider.name == "CardJoker")
                SelectCard(2);
            else if (hit.collider.name == "CardK")
                SelectCard(3);
            else if (hit.collider.name == "CardA")
                SelectCard(4);
            else if (rot == 0)
            {
                DeselectCards();
            }
        }
        else if (rot == 0)
        {

            DeselectCards();
        }
    }
    //Debug.DrawRay(transform.position, transform.TransformDirection(direction), Color.green);
}
