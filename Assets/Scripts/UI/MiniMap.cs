using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
   
    [SerializeField] private RectTransform viewPort;
    private Transform corner1, corner2;

    private Vector2 worldSize;

    private RectTransform miniMapRect;
    public GameObject cameraRig; //Main Camera Rig

    public GameObject blipPrefab;
    public GameObject blipParent;

    public static MiniMap instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        corner1 = CameraController.instance.Corner1;
        corner2 = CameraController.instance.Corner2;

        //Getting worldSize from the actual map
        worldSize = CalculateWorldSize();

        //Actual MiniMap
        miniMapRect = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateViewPort();
    }

    private Vector2 CalculateWorldSize()
    {
        Vector2 wSize = new Vector2(corner2.position.x - corner1.position.x, corner2.position.z - corner1.position.z);

        return wSize;
    }

    public Vector2 worldPosToMinimapPos(Vector3 worldPos)
    {
        float posX = worldPos.x / worldSize.x * miniMapRect.rect.width;
        float posY = worldPos.z / worldSize.y * miniMapRect.rect.height;

        //Debug.Log("posX: " + posX);
        //Debug.Log("posY: " + posY);

        Vector2 minimapPos = new Vector2(posX + miniMapRect.rect.width / 2,
                                        posY + miniMapRect.rect.height / 2);

        return minimapPos;
    }

    private void UpdateViewPort()
    {
        //Convert World Camera Pos to MiniMap's ViewPort Pos
        Vector3 position = worldPosToMinimapPos(cameraRig.transform.position);

        //In case there is a canvas scale with screen size
        viewPort.position = MainUI.instance.ScalePosition(position);
    }
}
