using UnityEngine;
using System.Collections;

public class TileHighlightScript : MonoBehaviour 
{
    //Textures
    public Texture selectedTexture;
    public Texture highlightedTexture;
    public Texture defaultTexture;

    //Player occupied flag
    public bool playerStartTile = false;

    //Material reference
    Material mat;

    //Player reference
    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    PlayerMovementScript pScript;

	// Use this for initialization
	void Start () 
    {
	    //Assign material
        mat = renderer.material;

        //Retrieve tilefunctions script.
        player = GameObject.Find("Player");
        pScript = player.GetComponent<PlayerMovementScript>();

        //Set default textures
        if (playerStartTile)
        {
            mat.mainTexture = selectedTexture;
            player.GetComponent<PlayerMovementScript>().occupiedTile = gameObject;
        }
        else
            mat.mainTexture = defaultTexture;
	}

    void OnMouseOver()
    {
        //Check for mouse click.  This assumes the tile being clicked is the script holder because of the OnMouseOver method.
        if (Input.GetMouseButtonDown(0) && isHighlighted())
        {
            if (pScript.clickedTile == null)
                pScript.clickedTile = gameObject;
        }
    }

    public void setTileState(TileState state)
    {
       switch (state)
       {
           case TileState.DEFAULT:
               mat.mainTexture = defaultTexture;
               break;
           case TileState.HIGHLIGHTED:
               mat.mainTexture = highlightedTexture;
               break;
           case TileState.OCCUPIED:
               mat.mainTexture = selectedTexture;
               break;

       }
    }

    public bool isHighlighted()
    {
        if (mat.mainTexture == highlightedTexture)
            return true;

        else return false;
    }
}
