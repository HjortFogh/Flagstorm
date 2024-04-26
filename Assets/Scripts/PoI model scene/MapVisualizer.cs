using UnityEngine;

public class MapVisualizer : MonoBehaviour
{
    public GameObject groundPrefab;
    public GameObject unWalkablePrefab;

    public MapTrain map;
    private void VisualizeMap()
    {
        Vector3 mapVisualizerLocalPosition = transform.parent.localPosition;

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                Vector3 localPosition = new Vector3(x, 0, y) + mapVisualizerLocalPosition;

                GameObject cellPrefab;
                switch (map.mapArray[x, y])
                {
                    case 0:
                        cellPrefab = Instantiate(unWalkablePrefab, localPosition, Quaternion.identity, transform);
                        break;
                    case 1:
                        cellPrefab = Instantiate(groundPrefab, localPosition, Quaternion.identity, transform);
                        break;
                    default:
                        cellPrefab = Instantiate(groundPrefab, localPosition, Quaternion.identity, transform);
                        break;
                }

                // No need to set parent again, as it's already set in the Instantiate call
            }
        }
    }

    private void Start()
    {
        VisualizeMap();
    }
}
