using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerDirection {
        Left,
        Right,
        Up,
        Down
    }

//This makes an extension for the PlayerDirection enum and allows us to do calculated values.
static class PlayerDirectionMethods {
    public static PlayerDirection Opposite(this PlayerDirection direction) {
        PlayerDirection retVal;
        switch (direction){
            case PlayerDirection.Left:
            retVal = PlayerDirection.Right;
            break;
            case PlayerDirection.Right:
            retVal = PlayerDirection.Left;
            break;
            case PlayerDirection.Up:
            retVal = PlayerDirection.Down;
            break;
            case PlayerDirection.Down:
            retVal = PlayerDirection.Up;
            break;
            default: retVal = PlayerDirection.Left; break;
        }
        return retVal;
    }
}

public class StarFieldScript : MonoBehaviour
{
     /**
    To get a Sprite we need to add a spriterenderer to the scene
    and then create a sprite with the texture and add it to the spriterenderer

      x 1 2 3
    y   -----
    1 | 1 2 3
    2 | 4 5 6
    3 | 7 8 9

    Row-major
    */
    public Texture2D[] starImages; // the png's of the start

    public int gridSize; //width and height of starfield

    public GameObject starPlate; // the plate of the field made from prefabs

    public Camera satelliteCamera;

    public int LayersCount = 1; // the number of layers of grids of stars to dislpay

    public Text debugText;

    private bool _forceAllPositionsCheck = false;

    /// Hold the layers of 2d star field arrays
    private List<StarFieldLayer> starLayers;

    /**
    This is the nearest starfield, everything else is placed behind.

    0 == camera z
    >0 further away
    */
    public float nearField;

    ///Hold the assembled sprites of star
    private Sprite[] starImagesprites;

    ///This holds the old camera position
    private Vector2 _oldCamPosition;

    /// This returns a tuple with the edge count and the scale ( from 1.0 > ) of the sprites that will hold the textures
    (int gridSize, float scale) layerFactors(int layerIndex) {
        return (gridSize + layerIndex, 1.0f);
    }

    ///This returns the z depth of a given layer
    float layerZ(int layerIndex) {
        return (float)layerIndex * 6.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        starLayers = new List<StarFieldLayer>();
        
        _oldCamPosition = new Vector2(satelliteCamera.transform.position.x, satelliteCamera.transform.position.y);

        starImagesprites = new Sprite[starImages.Length];
        int spi = 0;
        // First get the library of star sprites.
        foreach (Texture2D starTexture in starImages) {
            starImagesprites[spi] = Sprite.Create(starImages[spi], new Rect(0.0f, 0.0f, starImages[spi].width, starImages[spi].height), new Vector2(0.5f, 0.5f), 100.0f);
            spi ++;
        }

        for (int layer = 0; layer < LayersCount; layer++)
        {
            (int, float)_layerFactors = layerFactors(layer);
            Vector2 _fullEdgeWidth = new Vector2((float)_layerFactors.Item2 * starImages[0].width, (float)_layerFactors.Item2 * starImages[0].height);
            StarFieldLayer newLayer = new StarFieldLayer();
            for (int i = 0; i < (_layerFactors.Item1 * _layerFactors.Item1); i++)
            {
                GameObject newObject = Instantiate(starPlate);
                newObject.layer = LayerMask.NameToLayer("stars");
                int randomIndex = putRandomFieldInObject(newObject);
                
                Vector2 gridPos = new Vector2((float)(i / _layerFactors.Item1), (float)(i % _layerFactors.Item1));
                if (i == 0)
                {
                    newLayer.fullEdgeWidth = _fullEdgeWidth;
                    newLayer.pixelsPerUnit = starImagesprites[randomIndex].pixelsPerUnit;
                    newLayer.fullEdgeWidth.x /= newLayer.pixelsPerUnit; // the position is in normalised units so make the fullEdgeWidth normalised on the first sprite
                    newLayer.fullEdgeWidth.y /= newLayer.pixelsPerUnit;
                }
                newObject.transform.position = new Vector3((newLayer.fullEdgeWidth.x * gridPos.x) - ((newLayer.fullEdgeWidth.x * (float)_layerFactors.Item1) / 2) + (newLayer.fullEdgeWidth.x / 2), (newLayer.fullEdgeWidth.y * gridPos.y) - ((newLayer.fullEdgeWidth.y * (float)_layerFactors.Item1) / 2) + (newLayer.fullEdgeWidth.y / 2), layerZ(layer));

                newLayer.starGrid.Add(newObject);
            }
            starLayers.Add(newLayer);
        }
    }

    int putRandomFieldInObject(GameObject anObj) {
        SpriteRenderer newSR = anObj.GetComponent<SpriteRenderer>() as SpriteRenderer;
        int randomIndex = (int)Random.Range(0, starImages.Length - 1);
        newSR.sprite = starImagesprites[randomIndex];
        return randomIndex;
    }

    /// Returns the top, bottom, left, right most positions of the star field in the layer indicated by `layerIndex`
    (float top, float bottom, float left, float right) getSideValue(int layerIndex)
    {
        GameObject starObject = starLayers[layerIndex].starGrid[0];
        float top = starObject.transform.position.y;
        float bottom = starObject.transform.position.y;
        float left = starObject.transform.position.x;
        float right = starObject.transform.position.x;
        for (int i = 0; i < starLayers[layerIndex].starGrid.Count; i++)
        {
            starObject = starLayers[layerIndex].starGrid[i];
            if (starObject.transform.position.x < left) { left = starObject.transform.position.x; }
            if (starObject.transform.position.y < bottom) { bottom = starObject.transform.position.y; }
            if (starObject.transform.position.x > right) { right = starObject.transform.position.x; }
            if (starObject.transform.position.y > top) { top = starObject.transform.position.y; }
        }
        return (top, bottom, left, right);
    }

    //Check which side we're biased towards
    void checkPositionBias(PlayerDirection checkDirection) {
        List<int> edgePlatesIndicies = new List<int>(); //this will store the edge plate indicies
        int layerIndex = 0;
        
        // find the plates on the `edge` edge
        void getEdgePlates(PlayerDirection edge, int lIndex, (float top, float bottom, float left, float right) sides) {
            edgePlatesIndicies.Clear();
            int layerGridSize = layerFactors(lIndex).gridSize;
            for (int i = 0; i < layerGridSize * layerGridSize; i++) {
                PlateScript starObject = starLayers[lIndex].starGrid[i].GetComponent<PlateScript>();
                switch (edge) {
                    case PlayerDirection.Left:
                    if (starObject.transform.position.x > sides.left)
                        {
                            continue;
                        }
                        break;
                    case PlayerDirection.Right:
                    if (starObject.transform.position.x < sides.right)
                        {
                            continue;
                        }
                    break;

                    case PlayerDirection.Up:
                    if (starObject.transform.position.y < sides.top)
                        {
                            continue;
                        }
                        break;
                    case PlayerDirection.Down:
                    if (starObject.transform.position.y > sides.bottom)
                        {
                            continue;
                        }
                    break;
                }
                edgePlatesIndicies.Add(i);
            }
        }

        Vector3 newPos = new Vector3(0,0,0);
        foreach (StarFieldLayer starLayer in starLayers)
        {
            int layerGridSize = layerFactors(layerIndex).gridSize;
            (float top, float bottom, float left, float right) sides = getSideValue(layerIndex);
            Vector2 midPoint = new Vector2(((sides.right - sides.left) / 2) + sides.left, ((sides.top - sides.bottom)/2) + sides.bottom);
            float fullEdgeWidth = starLayer.fullEdgeWidth.x;
            float fullEdgeHeight = starLayer.fullEdgeWidth.y;
                getEdgePlates(checkDirection, layerIndex, sides);
                foreach (int index in edgePlatesIndicies) {
                    newPos = starLayer.starGrid[index].transform.position;
                    switch (checkDirection)
                    {
                        case PlayerDirection.Left:
                        if (satelliteCamera.transform.position.x > midPoint.x) {
                        newPos = new Vector3(starLayer.starGrid[index].transform.position.x + (fullEdgeWidth * (float)layerGridSize), starLayer.starGrid[index].transform.position.y, starLayer.starGrid[index].transform.position.z);
                        }
                        break;
                        case PlayerDirection.Right:
                        if (satelliteCamera.transform.position.x < midPoint.x) {
                        newPos = new Vector3(starLayer.starGrid[index].transform.position.x - (fullEdgeWidth * (float)layerGridSize), starLayer.starGrid[index].transform.position.y, starLayer.starGrid[index].transform.position.z);
                        }
                        break;
                        case PlayerDirection.Up:
                        if (satelliteCamera.transform.position.y < midPoint.y) {
                        newPos = new Vector3(starLayer.starGrid[index].transform.position.x, starLayer.starGrid[index].transform.position.y - (fullEdgeHeight * (float)layerGridSize), starLayer.starGrid[index].transform.position.z);
                        }
                        break;
                        case PlayerDirection.Down:
                        if (satelliteCamera.transform.position.y > midPoint.y) {
                        newPos = new Vector3(starLayer.starGrid[index].transform.position.x, starLayer.starGrid[index].transform.position.y + (fullEdgeHeight * (float)layerGridSize), starLayer.starGrid[index].transform.position.z);
                        }
                        break;
                        default: break;
                    }
                    starLayer.starGrid[index].transform.position = newPos;
                }
            layerIndex ++;
        }
    }

    public void largeMoveTo(Vector3 newPosition) {
        foreach (StarFieldLayer layer in starLayers) {
            foreach (GameObject obj in layer.starGrid) {
                obj.transform.parent = transform;
            }
        }
        transform.position = newPosition;
        foreach (StarFieldLayer layer in starLayers) {
            foreach (GameObject obj in layer.starGrid) {
                obj.transform.parent = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_forceAllPositionsCheck) {
            checkPositionBias(PlayerDirection.Left);
            checkPositionBias(PlayerDirection.Right);
            checkPositionBias(PlayerDirection.Up);
            checkPositionBias(PlayerDirection.Down);
        } else {
            float xDelta = satelliteCamera.transform.position.x - _oldCamPosition.x; // -ve left, +ve right
            if (Mathf.Abs(xDelta) > 0.0 || _forceAllPositionsCheck) { checkPositionBias(xDelta < 0 ? PlayerDirection.Right : PlayerDirection.Left);
            
            float yDelta = satelliteCamera.transform.position.y - _oldCamPosition.y; // +ve up, -ve down
            if (Mathf.Abs(yDelta) > 0.0 || _forceAllPositionsCheck) { checkPositionBias(yDelta < 0 ? PlayerDirection.Up : PlayerDirection.Down); }
        }

        _oldCamPosition = new Vector2(satelliteCamera.transform.position.x, satelliteCamera.transform.position.y);
        _forceAllPositionsCheck = false;
    }
    }

    public void updateAllPositions() {
        _forceAllPositionsCheck = true;
    }

}

//In order to hold layers of starImages, we'll make a holding class just for the starImages

public class StarFieldLayer {
    public List<GameObject> starGrid = new List<GameObject>();  //this is the grid of stars in the layer
    public float pixelsPerUnit; // this is used for positioning the sprites. Each sprite has this and we store it for speed
    public Vector2 fullEdgeWidth; //the size of the width and height of the starfield so we can easily put new images at the opposite ends
    public Vector3 relativePositionFromPlayer; // the relative vector from the player so we can make a large step with player.
}
