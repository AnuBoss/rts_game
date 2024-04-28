using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ResourceType
{
    Food,
    Wood,
    Gold,
    Stone
}
public class ResourceSource : MonoBehaviour
{
    [SerializeField] private string rsrcName;
    public string RsrcName { get { return rsrcName; } }

    [SerializeField] private Sprite rsrcPic;
    public Sprite RsrcPic { get { return rsrcPic; } }

    [SerializeField] private ResourceType rsrcType;
    public ResourceType RsrcType { get { return rsrcType; } }

    [SerializeField] private int quantity;
    public int Quantity { get { return quantity; } set { quantity = value; } }

    [SerializeField] private int maxQuantity;
    public int MaxQuantity
    {
        get { return maxQuantity; }
    }

    [SerializeField] private GameObject selectionVisual;
    public GameObject SelectionVisual { get { return selectionVisual; } } 
    
    //Selection Ring
    [SerializeField]
    private UnityEvent onRsrcQuantityChange;
    [SerializeField]
    private UnityEvent onInfoQuantityChange;

    // Start is called before the first frame update
    void Start()
    {
        onRsrcQuantityChange.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (quantity <= 0)
        {
            InfoManager.instance.ClearAllInfo();
            Destroy(gameObject);
        }

    }
    
    //called when a unit gathers the resource
    public void GatherResource(int amountRequest)
    {
        int amountToGive;

        // make sure we don't give more than we have
        if (amountRequest > quantity) //not enough
            amountToGive = quantity;
        else //enough
            amountToGive = amountRequest;

        quantity -= amountToGive;

        onRsrcQuantityChange.Invoke();

        // if we're depleted, delete the resource
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }
    }

    // toggles green selection ring around resource
    public void ToggleSelectionVisual(bool selected)
    {
        if (SelectionVisual != null)
            SelectionVisual.SetActive(selected);
    }

}
