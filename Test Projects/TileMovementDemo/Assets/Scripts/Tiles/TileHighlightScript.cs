using UnityEngine;
using System.Collections;

public class TileHighlightScript : MonoBehaviour 
{
    //Textures
    public Texture selectedTexture;
    public Texture highlightedTexture;
    public Texture defaultTexture;

    //Material reference
    Material mat;

	// Use this for initialization
	void Start () 
    {
	    //Assign material
        mat = renderer.material;

        //Default to unselected texture
        mat.mainTexture = defaultTexture;
	}

    void OnMouseOver()
    {
        //When moused over, show selected texture.
        //HighlightTile();

        //Check for mouse click.  This assumes the tile being clicked is the script holder because of the OnMouseOver method.
        if (Input.GetMouseButtonDown(0))
        {
            //Dim all tiles
            TileFunctions.DimAllTiles();

            //highlight adjacant tiles
            HighlightAdjacentTiles();
        }
    }

    /// <summary>
    /// Determines the 8 adjacent tiles and highlights them.
    /// </summary>
    private void HighlightAdjacentTiles()
    {
        /*
        * Shoot 4 ray casts, one out of each side.  They'll be looking for tile tags. (primary rays)
        * Those tiles will then shoot rays out of certain sides, depending on the position relative to the centre tile. (secondary rays)
            * Tile hit by up ray shoots left and right rays.
            * Tile hit by down ray shoots left and right rays.
            * Tile hit by right ray shoots up and down rays.
            * Tile hit by left ray shoots up and down rays.
         * Arrays are set up as:
         * 
         *      [0]1    [1]2    [2]3
         *      
         *      [3]4    [4]5    [5]6
         *      
         *      [6]7    [7]8    [8]9
         * 
         */

        //Set up array.
        GameObject[] Tiles = new GameObject[9];

        //Primary rays
        Tiles[1] = GetTileInDirection(Vector3.forward);
        Tiles[7] = GetTileInDirection(Vector3.back);
        Tiles[3] = GetTileInDirection(Vector3.left);
        Tiles[5] = GetTileInDirection(Vector3.right);

        //Secondary rays
        Tiles[0] = DetermineSecondaryRays(Tiles[3], Tiles[2]);
        Tiles[2] = DetermineSecondaryRays(Tiles[5], Tiles[1]);
        Tiles[6]=  DetermineSecondaryRays(Tiles[3], Tiles[7]);
        Tiles[8] = DetermineSecondaryRays(Tiles[5], Tiles[7]);
        
        //highlight tiles
        foreach (GameObject t in Tiles)
        {
            if (t != null)
                t.GetComponent<TileHighlightScript>().HighlightTile();
        }

        //Set this tile to the selected texture
        mat.mainTexture = selectedTexture;
    }

    public void HighlightTile()
    {
        mat.mainTexture = highlightedTexture;
    }

    public void DimTile()
    {
        mat.mainTexture = defaultTexture;
    }

    /// <summary>
    /// Fires a ray along the given direction, returning a Tile in the path of the ray.
    /// </summary>
    /// <param name="direction">A direction of forward, back, left, or right.</param>
    /// <returns>A Tile gameobject which was hit by the ray, or null.</returns>
    private GameObject GetTileInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 2.0f))
        {
            if (hit.transform.tag == "Tile")
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Fires a ray along the given direction from given object, returning a Tile in the path of the ray.
    /// </summary>
    /// <param name="source">The object firing the ray.</param>
    /// <param name="direction">A direction of forward, back, left, or right.</param>
    /// <returns>A Tile gameobject which was hit by the ray, or null.</returns>
    private GameObject GetTileInDirection(GameObject source, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(source.transform.position, direction, out hit, 10.0f))
        {
            if (hit.transform.tag == "Tile")
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Resolves diagonal adjacents by comparing provided Tiles.
    /// </summary>
    /// <param name="adjacentX">The Tile to the left or right of the centre Tile</param>
    /// <param name="adjacentY">The Tile above or below the centre Tile</param>
    /// <returns>The Tile adjacent to both provided tiles.</returns>
    private GameObject DetermineSecondaryRays(GameObject adjacentX, GameObject adjacentY)
    {
        //Declaration for each potentially adjacent tile.
        GameObject TUp = null, TDown = null, TLeft = null, TRight = null, TOut = null;

        //Shoot bi-directional rays othagonal to the centre.
        //Fire rays from object adjacent on the X axis.
        if (adjacentX != null)
        {
            TUp = GetTileInDirection(adjacentX, Vector3.forward);
            TDown = GetTileInDirection(adjacentX, Vector3.back);
        }
        //Fire rays from object adjacent on the Y axis.
        if (adjacentY != null)
        {
            TLeft = GetTileInDirection(adjacentY, Vector3.left);
            TRight = GetTileInDirection(adjacentY, Vector3.right);
        }

        //Compare left and top, then right and top, then left and bottom, then right and bottom
        TOut = TileFunctions.CompareTiles(TLeft, TUp);

        if (TOut == null)
        {
            TOut = TileFunctions.CompareTiles(TRight, TUp);
        }

        if (TOut == null)
        {
            TOut = TileFunctions.CompareTiles(TLeft, TDown);
        }

        if (TOut == null)
        {
            TOut = TileFunctions.CompareTiles(TRight, TDown);
        }

        //Finally, return TOut;
        return TOut;
    }
}
