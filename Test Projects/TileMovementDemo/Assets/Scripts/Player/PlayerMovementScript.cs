using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour 
{
    //Tile clicked by the user
    [HideInInspector]
    public GameObject clickedTile = null;

    //Player occupied tile
    [HideInInspector]
    public GameObject occupiedTile;

    //The player's intended position on a tile.  This is the tile's x and z coords, with y as 1.2f
    Vector3 targetPlayerPos;

    //The distance between the player's tile and the target tile.
    float distanceToTargetTile;

    //Time at lerp start
    float startTime;

    //Player's movement speed
    public float moveSpeed;

	// Use this for initialization
	void Start () 
    {
        //Highlight surrounding tiles.
        HighlightAdjacentTiles(occupiedTile);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (clickedTile != null)
        {
            //Get the clicked tile's tilescript.
            TileHighlightScript tScript = clickedTile.GetComponent<TileHighlightScript>();

            //Before moving to the target tile.
            if (Input.GetMouseButtonDown(0) && tScript.isHighlighted())
            {
                MoveToTile(clickedTile);
            }

            //While moving towards the target tile.
            if (transform.position != targetPlayerPos)
            {
                float covered = (Time.time - startTime) * moveSpeed;
                float fractionOfDistanceCovered = covered / distanceToTargetTile;
                transform.position = Vector3.Lerp(transform.position, targetPlayerPos, fractionOfDistanceCovered);
            }

            //After moving and arriving on the tile.
            if (transform.position == targetPlayerPos)
            {
                //Set occupied tile
                occupiedTile = clickedTile;
                tScript.setTileState(TileState.OCCUPIED);

                //Reset clicked tile.
                clickedTile = null;

                //Highlight other tiles
                HighlightAdjacentTiles(occupiedTile);
            }
        }
	}

    void MoveToTile(GameObject tile)
    {
        //Set the target tile
        targetPlayerPos = new Vector3(tile.transform.position.x, 1.2f, tile.transform.position.z);

        //Set distance to target tile.
        distanceToTargetTile = Vector3.Distance(occupiedTile.transform.position, tile.transform.position);

        //Set the start time for the lerp.
        startTime = Time.time;
    }

    private void HighlightAdjacentTiles(GameObject origin)
    {
        //Reset all tiles, except the occupied one.
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Tile"))
        {
            if (go != occupiedTile)
                go.GetComponent<TileHighlightScript>().setTileState(TileState.DEFAULT);
        }

        //Set up array.
        GameObject[] Tiles = new GameObject[9];  

        //Fire Primary rays
        Tiles[1] = GetTileInDirection(origin, Vector3.forward);
        Tiles[7] = GetTileInDirection(origin, Vector3.back);
        Tiles[3] = GetTileInDirection(origin, Vector3.left);
        Tiles[5] = GetTileInDirection(origin, Vector3.right);

        //Fire Secondary rays
        Tiles[0] = DetermineSecondaryRays(Tiles[3], Tiles[2]);
        Tiles[2] = DetermineSecondaryRays(Tiles[5], Tiles[1]);
        Tiles[6] = DetermineSecondaryRays(Tiles[3], Tiles[7]);
        Tiles[8] = DetermineSecondaryRays(Tiles[5], Tiles[7]);

        //highlight Tiles
        foreach (GameObject t in Tiles)
        {
            if (t != null)
                t.GetComponent<TileHighlightScript>().setTileState(TileState.HIGHLIGHTED);
        }
    }

    private GameObject GetTileInDirection(GameObject origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin.transform.position, direction, out hit, 2.0f))
        {
            if (hit.transform.tag == "Tile")
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

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
        TOut = CompareTiles(TLeft, TUp);

        if (TOut == null)
            TOut = CompareTiles(TRight, TUp);

        if (TOut == null)
            TOut = CompareTiles(TLeft, TDown);

        if (TOut == null)
            TOut = CompareTiles(TRight, TDown);

        //Finally, return TOut;
        return TOut;
    }

    private GameObject CompareTiles(GameObject Tile1, GameObject Tile2)
    {
        //Handle null Tiles first
        if (Tile2 == null)
            return Tile1;

        if (Tile1 == null)
            return Tile2;

        //Then resolve non-null cases.  Don't need to check for not-null because it's implied that neither are null
        if (Tile1.transform.position == Tile2.transform.position)
        {
            return Tile1; //Could also return T2 here, since they're the same Tile.
        }
        else return null;
    }
}