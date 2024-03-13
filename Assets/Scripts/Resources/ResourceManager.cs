using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] woodTreePrefab;

    [SerializeField]
    private Transform woodTreeParent;

    [SerializeField]
    private ResourceSource[] resources;
    public ResourceSource[] Resources
    {
        get { return resources; }
    }

    public static ResourceManager instance;

    void Awake()
    {
        instance = this;
    }

    private void FindAllResource()
    {
        resources = FindObjectsOfType<ResourceSource>();
    }
    void Start()
    {
        FindAllResource();
    }

}
