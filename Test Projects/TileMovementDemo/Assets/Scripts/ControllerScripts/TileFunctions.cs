using UnityEngine;
using System.Collections;

public static class TileFunctions 
{
    public static void DimAllTiles()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Tile"))
        {
            go.GetComponent<TileHighlightScript>().DimTile();
        }
    }

    public static GameObject CompareTiles(GameObject Tile1, GameObject Tile2)
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