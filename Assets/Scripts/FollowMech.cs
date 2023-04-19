using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMech : MonoBehaviour
{
    public GameObject mech;
    private Vector3 offset = new Vector3(0, 0, 0);
    private Quaternion offsetRot;
    MechControllerRB mechController;

    private void Awake()
    {
        mechController = mech.GetComponent<MechControllerRB>();
    }
    // Start is called before the first frame update
    void Start()
    {
        offsetRot = Quaternion.identity;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = mech.transform.position + offset;

        if (mechController.quickTurn)
        {
            offsetRot = Quaternion.Inverse(mech.transform.rotation)*transform.rotation;
        }
        else
        {
            transform.rotation = mech.transform.rotation*offsetRot;
        }
    }
}
