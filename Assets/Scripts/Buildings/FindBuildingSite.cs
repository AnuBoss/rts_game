using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindBuildingSite : MonoBehaviour
{
    [SerializeField]
    private bool canBuild = false;
    public bool CanBuild { get { return canBuild; } set { canBuild = value; } }

    [SerializeField]
    private MeshRenderer[] modelRdr;
    [SerializeField]
    private MeshRenderer planeRdr;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < modelRdr.Length; i++)
            modelRdr[i].material.color = Color.green;

        //Setup Plane Color
        planeRdr.material.color = Color.green;

        CanBuild = true;

    }

    private void SetCanBuild(bool flag)
    {
        if (flag)
        {
            for (int i = 0; i < modelRdr.Length; i++)
                modelRdr[i].material.color = Color.green;

            planeRdr.material.color = Color.green;
            canBuild = true;
        }
        else
        {
            for (int i = 0; i < modelRdr.Length; i++)
                modelRdr[i].material.color = Color.red;

            planeRdr.material.color = Color.red;
            canBuild = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Resource" || other.tag == "Building" || other.tag == "Unit")
            SetCanBuild(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Resource" || other.tag == "Building" || other.tag == "Unit")
            SetCanBuild(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Resource" || other.tag == "Building" || other.tag == "Unit")
            SetCanBuild(true);
    }

}
