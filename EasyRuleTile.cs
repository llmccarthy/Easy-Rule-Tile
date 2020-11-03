using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using static DirectionAPI;
using static ByteAPI;
using static TextureManager;

[CreateAssetMenu(fileName = "EasyRuleTile", menuName = "EasyRuleTile", order = 1)]
public class EasyRuleTile : Tile
{
   
    public string tileClass;
    public Sprite[] textures;

    /// <summary>
    /// Determine if this Easy Rule Tile has the same tile class as another inputed Easy Rule Tile. If the inputted tile is null, then false is returned. If the inputted tile
    /// is of type TileBase but not an Easy Rule Tile, true is return iff both tiles share the same name.
    /// </summary>
    /// <param name="tile"> The inputted tile we are checking against </param>
    /// <returns> Whether the two tiles share the same tile class</returns>
    public bool SameTileClass(TileBase tile)
    {
        if (tile == null) return false;
        if (tile is EasyRuleTile) return ((EasyRuleTile)tile).tileClass == tileClass;
        else return tile.name == name;
    }

    //Reloads the textures in the asset database to the textures array before the first frame.
    public void Awake()
    {
        for (int i = 0; i < textures.Length; i++)
            textures[i] = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures/" + name + "_" + i.ToString() + ".png");
    }

    // This refreshes itself and other RoadTiles that are laterally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (SameTileClass(tilemap.GetTile(position)))
                    base.RefreshTile(position, tilemap);
            }
    }

    // Set's the current tile's texture based on the surrounding tiles.
    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        Tilemap t = tilemap.GetComponent<Tilemap>();
        
        if (textures.Length == 47)
        {
            int index = GetTextureIndex(location, tilemap);
            //tileData.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures/" + name + "_" + index.ToString() + ".png");
            tileData.sprite = textures[GetTextureIndex(location, tilemap)];
        }
        else Debug.LogError("Array is empty");
    }

    /// <summary>
    /// Checks if a neighbor in a given direction of a tile at a given position is occupied by a tile of the same tile class.
    /// </summary>
    /// <param name="position"> The position of the tile whose neighbor's we are searching </param>
    /// <param name="tilemap"> The tilemap we are searching</param>
    /// <param name="direction"> The directional ID for direction we are searching for </param>
    /// <returns> Whether or not the searched tile is occuppied by a tile of the same tile class</returns>
    public bool Occupied(Vector3Int position, ITilemap tilemap, int direction)
    {
        int horizontal = GetHorizontalDirection(direction);
        int vertical = GetVerticalDirection(direction);
        int xOffset = horizontal == RIGHT ? 1 : horizontal == LEFT   ? -1 : 0;
        int yOffset = vertical   == TOP   ? 1 : vertical   == BOTTOM ? -1 : 0;
        if(horizontal == VERTICAL & vertical == HORIZONTAL)
        Debug.Log(horizontal == VERTICAL & vertical == HORIZONTAL);
        TileBase tile = tilemap.GetTile(new Vector3Int(position.x + xOffset, position.y + yOffset, position.z));
        return SameTileClass(tile);
    }

    //Texture Generation and Retrieval
    #region

    /// SLICE IDS:
    /// 
    /// Each slice array is named after which type of wall tile it contains slices of, as well as which corner of those tiles the slices are from, (the 
    /// direction this corner is in is henceforth referred to as the slice's base direction). For example, the array for the top right slices of corner
    /// tiles is called "cornerSlicesTopRight".11
    /// 
    /// Slice IDs correspond to the index of slice arrays. They are named after which type of tile they are slices of followed by 5 characters
    /// that describe whether there exists neighbors from 2 directions counterclockwise of the base direction to 2 directions clockwise of the base
    /// direction.
    /// 
    /// The first character corresponds to the direction 1 step counterclockwise from the base direction
    /// The second character corresponds to the slice array's base direction
    /// The thrid character corresponds to the direction 1 steps clockwise from the base direction
    /// 
    /// And these 5 characters can be either a 1, 0, or X, meaning:
    /// 
    /// 0 means there does not exist a neighbor in that direction
    /// 1 means there exists a neighbor in that direction
    /// X means there can either exist or not exist a neighbor in that direction
    ///
    /// These are the 5 slice ids for Easy Rule Tiles:

    public const int slice_0X0 = 0;
    public const int slice_0X1 = 1;
    public const int slice_1X0 = 2;
    public const int slice_111 = 3;
    public const int slice_101 = 4;

    /// TEXTURE INDEXS:
    /// There are a total of 47 possible different textures an easy rule tile can have
    /// A texture index can be found from a texture's slice ids. 
    /// 
    /// 00 Cases:
    /// [ 0 0 ] [ d c b a ]
    /// If the first two bits are 00, then no corner can have 101 as a slice ID, and:
    /// If a == 1, there is another rule tile of the same class in the tile in the top    neighbor position, but if no such tile exists in that position, a == 0
    /// If b == 1, there is another rule tile of the same class in the tile in the left   neighbor position, but if no such tile exists in that position, b == 0
    /// If c == 1, there is another rule tile of the same class in the tile in the bottom neighbor position, but if no such tile exists in that position, c == 0
    /// If d == 1, there is another rule tile of the same class in the tile in the right  neighbor position, but if no such tile exists in that position, d == 0
    /// 
    /// 
    /// 01 Cases:
    /// [ 0 1 ] [ c d ] [ a b ]    
    /// If the first two bits are 01, then exactly two orthogonal neighbors positions must have another rule tile of the same class with two adjacent corners that
    ///                               have 0X1 and 1X0 as slice IDs, and the remaining corners must have either 101 or 0X0 as a slice ID, and:
    /// If ab == 00, there isn't another rule tile of the same class in the tile in the top    neighbor position, but all other orthogonal neighbor positions do
    /// If ab == 01, there isn't another rule tile of the same class in the tile in the right  neighbor position, but all other orthogonal neighbor positions do
    /// If ab == 10, there isn't another rule tile of the same class in the tile in the bottom neighbor position, but all other orthogonal neighbor positions do
    /// If ab == 11, there isn't another rule tile of the same class in the tile in the left   neighbor position, but all other orthogonal neighbor positions do
    /// Additionally,
    /// If cd == 00, the slice ID of the corner 3 steps counterclockwise from the direction indicated by ab is 101, while the corner 3 steps clockwise is 101
    /// If cd == 01, the slice ID of the corner 3 steps counterclockwise from the direction indicated by ab is 101, while the corner 3 steps clockwise is 111
    /// If cd == 10, the slice ID of the corner 3 steps counterclockwise from the direction indicated by ab is 111, while the corner 3 steps clockwise is 101
    /// 
    /// 0111 Cases:
    /// [ 0 1 1 1 ] [ a b ]    
    /// If the first four bits are 0111, then there are only 2 rule tiles of the same class adjacent to the given tile, and the direction of the
    ///                                  adjacent tiles are 2 steps apart and not diagonal, and:
    /// If ab == 00, there is another rule tile of the same class in the tile in the top    neighbor position and the right  position
    /// If ab == 01, there is another rule tile of the same class in the tile in the right  neighbor position and the bottom position
    /// If ab == 10, there is another rule tile of the same class in the tile in the bottom neighbor position and the left   position
    /// If ab == 11, there is another rule tile of the same class in the tile in the left   neighbor position and the top    position
    /// 
    /// 10 Cases:
    /// [ 1 0 ] [ d c b a ]
    /// If the first two bits are 10, then each corner must have either 111 or 101 as a slice ID, and:
    /// If a == 1, the top    right corner has 111 as a slice ID, otherwise it has 101 as a slice ID
    /// If b == 1, the bottom right corner has 111 as a slice ID, otherwise it has 101 as a slice ID
    /// If c == 1, the bottom left  corner has 111 as a slice ID, otherwise it has 101 as a slice ID
    /// If d == 1, the top    left  corner has 111 as a slice ID, otherwise it has 101 as a slice ID
    /// Note: 101111 is not a valid case, as it would be identical to the already existing 001111 case.


    //Textures Inputs    
    public Sprite standalone;
    public Sprite surrounded;
    public Sprite horizontal;
    public Sprite vertical;
    public Sprite intersection;

    public const string PATH_TO_RULE_TILE_TEXTURES = "Assets/Textures/Map Tiles";
    public const string PATH_TO_RULE_TILE_TEXTURES_WITHOUT_ASSETS = "/Textures/Map Tiles";


    ///////////////////////////
    //// TEXTURE RETRIEVAL ////
    ///////////////////////////

    /// <summary>
    /// Determine which slice ID is necessary for an inputted quadrant of an inputted tile by examining the neighboring positions of that tile 
    /// </summary>
    /// <param name="position"> The position of the tile we are searching </param>
    /// <param name="tilemap"> The tilemap that the tile we are searching for is on </param>
    /// <param name="cornerDirection"> The direction ID corresponding to the quadrant of the tile's texture we wish to determine the slice ID of</param>
    /// <returns> The slice ID of the tile's quadrant </returns>
    public int GetSliceIDFromNeighbors(Vector3Int position, ITilemap tilemap, int cornerDirection)
    {      
        if (Occupied(position, tilemap, GetClockwiseDirection(cornerDirection, -1)))
            if (Occupied(position, tilemap, GetClockwiseDirection(cornerDirection)))
                if (Occupied(position, tilemap, cornerDirection)) /*---------------*/ return slice_111;
                else /*------------------------------------------------------------*/ return slice_101;
            else /*----------------------------------------------------------------*/ return slice_1X0;
        else if (Occupied(position, tilemap, GetClockwiseDirection(cornerDirection))) return slice_0X1;
            else /*----------------------------------------------------------------*/ return slice_0X0;
    }

    /// <summary>
    /// Gets the necessary texture index for an inputted Easy Rule Tile based on it's neighbors
    /// </summary>
    /// <param name="position"> The position of the tile whose texture index we wish to determine </param>
    /// <param name="tilemap"> The tilemap that the tile whose texture index we wish to determine is on </param>
    /// <returns> The necessary texture index for an inputted Easy Rule Tile </returns>
    public byte GetTextureIndex(Vector3Int position, ITilemap tilemap)
    {
        bool Occupied  (int direction) { return this.Occupied(position, tilemap, direction); }
        bool IsSlice0X0(int direction) { return GetSliceIDFromNeighbors(position, tilemap, direction) == slice_0X0; }
        bool IsSlice0X1(int direction) { return GetSliceIDFromNeighbors(position, tilemap, direction) == slice_0X1; }
        bool IsSlice1X0(int direction) { return GetSliceIDFromNeighbors(position, tilemap, direction) == slice_1X0; }
        bool IsSlice111(int direction) { return GetSliceIDFromNeighbors(position, tilemap, direction) == slice_111; }
        bool IsSlice101(int direction) { return GetSliceIDFromNeighbors(position, tilemap, direction) == slice_101; }
        int BoolToInt(bool b) { return b ? 1 : 0; }

        bool         topOccupied = Occupied(TOP);
        bool       rightOccupied = Occupied(RIGHT);
        bool      bottomOccupied = Occupied(BOTTOM);
        bool        leftOccupied = Occupied(LEFT);
        bool    topRightOccupied = Occupied(TOP_RIGHT);
        bool bottomRightOccupied = Occupied(BOTTOM_RIGHT);
        bool  bottomLeftOccupied = Occupied(BOTTOM_LEFT);
        bool     topLeftOccupied = Occupied(TOP_LEFT);

        int  lateralNeighbors = BoolToInt(topOccupied)      + BoolToInt(rightOccupied)       + BoolToInt(bottomOccupied)     + BoolToInt(leftOccupied);
        int diagonalNeighbors = BoolToInt(topRightOccupied) + BoolToInt(bottomRightOccupied) + BoolToInt(bottomLeftOccupied) + BoolToInt(topLeftOccupied);
        int cornersWith0X0IDs = BoolToInt(IsSlice0X0(TOP_RIGHT)) + BoolToInt(IsSlice0X0(BOTTOM_RIGHT)) + BoolToInt(IsSlice0X0(BOTTOM_LEFT)) + BoolToInt(IsSlice0X0(TOP_LEFT));
        int cornersWith0X1IDs = BoolToInt(IsSlice0X1(TOP_RIGHT)) + BoolToInt(IsSlice0X1(BOTTOM_RIGHT)) + BoolToInt(IsSlice0X1(BOTTOM_LEFT)) + BoolToInt(IsSlice0X1(TOP_LEFT));
        int cornersWith1X0IDs = BoolToInt(IsSlice1X0(TOP_RIGHT)) + BoolToInt(IsSlice1X0(BOTTOM_RIGHT)) + BoolToInt(IsSlice1X0(BOTTOM_LEFT)) + BoolToInt(IsSlice1X0(TOP_LEFT));
        int cornersWith111IDs = BoolToInt(IsSlice111(TOP_RIGHT)) + BoolToInt(IsSlice111(BOTTOM_RIGHT)) + BoolToInt(IsSlice111(BOTTOM_LEFT)) + BoolToInt(IsSlice111(TOP_LEFT));
        int cornersWith101IDs = BoolToInt(IsSlice101(TOP_RIGHT)) + BoolToInt(IsSlice101(BOTTOM_RIGHT)) + BoolToInt(IsSlice101(BOTTOM_LEFT)) + BoolToInt(IsSlice101(TOP_LEFT));

        //00 Cases
        if (cornersWith101IDs == 0)
            return GetByteFromBits(false, false, false, false, leftOccupied, bottomOccupied, rightOccupied, topOccupied);

        //01 Cases
        if (lateralNeighbors == 3)
        {
            (bool, bool) ab = (!bottomOccupied | !leftOccupied, !rightOccupied | !leftOccupied);
            int abDir = GetOrthogonalDirectionalIDFrom2BitDirectionalID(ab.Item1, ab.Item2);
            bool c = Occupied(GetClockwiseDirection(abDir, -3));
            bool d = Occupied(GetClockwiseDirection(abDir,  3));
            return GetByteFromBits(false, false, false, true, c, d, ab.Item1, ab.Item2);
        }

        //0111 Cases
        if (lateralNeighbors == 2 && cornersWith101IDs == 1)
            return GetByteFromBits(false, false, false, true, true, true, leftOccupied, (rightOccupied ^ !bottomOccupied));

        //10 Cases
        if (cornersWith0X0IDs == 0 & cornersWith0X1IDs == 0 & cornersWith1X0IDs == 0)
            return GetByteFromBits(false, false, true, false, IsSlice111(TOP_LEFT), IsSlice111(BOTTOM_LEFT), IsSlice111(BOTTOM_RIGHT), IsSlice111(TOP_RIGHT));

        Debug.LogError("ERROR: If you're reading this, something is wrong with the code");
        Debug.LogError(topOccupied.ToString() + " " + rightOccupied.ToString() + " " + bottomOccupied.ToString() + " " + leftOccupied.ToString() + " " +
                       topRightOccupied.ToString() + " " + bottomRightOccupied.ToString() + " " + bottomLeftOccupied.ToString() + " " + topLeftOccupied.ToString());
       

        return 0b_11111111;
    }

    ////////////////////////////
    //// TEXTURE GENERATION ////
    ////////////////////////////
    /// <summary>
    /// Get the slice ID of a given quadrant of a texture determined by a given texture index.
    /// </summary>
    /// <param name="direction"> A diagonal direction ID indicating which quadrant of the texture indicated by the texture ID we examine</param>
    /// <param name="textureIndex"> The texture ID corresponding to specific texture configuration </param>
    /// <returns> The slice ID of a quadrant of a texture indicated by the texture Index </returns>
    public static int GetSliceIDFromTextureIndex(int direction, int textureIndex)
    {
        bool bit6 = GetBitAtByteIndex((byte)textureIndex, 5);
        bool bit5 = GetBitAtByteIndex((byte)textureIndex, 4);
        bool bit4 = GetBitAtByteIndex((byte)textureIndex, 3);
        bool bit3 = GetBitAtByteIndex((byte)textureIndex, 2);
        bool bit2 = GetBitAtByteIndex((byte)textureIndex, 1);
        bool bit1 = GetBitAtByteIndex((byte)textureIndex, 0);

        byte firstTwoBits = GetByteFromBits(false, false, false, false, false, false, bit6, bit5);

        switch (firstTwoBits)
        {
            case 0b_00:
                bool ccwDirectionBit = direction == TOP_RIGHT ? bit1 : direction == BOTTOM_RIGHT ? bit2 : direction == BOTTOM_LEFT ? bit3 : bit4;
                bool cwDirectionBit = direction == TOP_RIGHT ? bit2 : direction == BOTTOM_RIGHT ? bit3 : direction == BOTTOM_LEFT ? bit4 : bit1;
                return ccwDirectionBit ? cwDirectionBit ? slice_111 : slice_1X0 :
                                         cwDirectionBit ? slice_0X1 : slice_0X0;
            case 0b_01:
                int rotationDirection = bit4 && bit3 ? GetDiagonalDirectionalIDFrom2BitDirectionalID(bit2, bit1) : GetOrthogonalDirectionalIDFrom2BitDirectionalID(bit2, bit1);
                int steps = GetStepsBetweenDirection(rotationDirection, direction);
                if (!(bit4 && bit3))
                {
                    int ccw3Slice = bit4 ? slice_111 : slice_101;
                    int cw3Slice = bit3 ? slice_111 : slice_101;
                    return steps == -3 ? ccw3Slice : steps == -1 ? slice_1X0 : steps == 1 ? slice_0X1 : cw3Slice;
                }
                else return steps == -2 ? slice_0X1 : steps == 0 ? slice_101 : steps == 2 ? slice_1X0 : slice_0X0;
            case 0b_10:
                bool directionBit = direction == TOP_RIGHT ? bit1 : direction == BOTTOM_RIGHT ? bit2 : direction == BOTTOM_LEFT ? bit3 : bit4;
                return directionBit ? slice_111 : slice_101;

        }

        // Invalid Case
        Debug.Log("Warning: Invalid Case");
        return -1;
    }

    /// <summary>
    /// Return's a quadrant of one of the inputted textures
    /// </summary>
    /// <param name="direction"> A diagonal direction ID indicating which quadrant of the desired texture that will be returned </param>
    /// <param name="sliceID"> The slice ID that determines which of the inputted textures we intend to get the quadrant from</param>
    /// <returns> A texture 2D of one of the </returns>
    public Texture2D GenerateSlice(int direction, int sliceID)
    {
        Sprite  CWEdge = direction == TOP_RIGHT || direction == BOTTOM_LEFT ? vertical   : horizontal;
        Sprite CCWEdge = direction == TOP_RIGHT || direction == BOTTOM_LEFT ? horizontal : vertical;

        switch (sliceID)
        {
            case slice_0X0: return CropTexture(standalone.texture,   direction);
            case slice_0X1: return CropTexture(CCWEdge.texture,      direction);
            case slice_1X0: return CropTexture(CWEdge.texture,       direction);
            case slice_111: return CropTexture(surrounded.texture,   direction);
            case slice_101: return CropTexture(intersection.texture, direction);
        }

        // Invalid Case
        Debug.LogError("ERROR: Invalid Slice ID");
        return null;
    }

    /// <summary>
    /// Create a new texture based on a texture index
    /// </summary>
    /// <param name="textureIndex"> The texture index of the texture we intend to creature </param>
    /// <returns> The texture 2D of the new texture </returns>
    public Texture2D GenerateTexture(int textureIndex)
    {
        Texture2D topRightSlice    = GenerateSlice(TOP_RIGHT,    GetSliceIDFromTextureIndex(TOP_RIGHT,    textureIndex));
        Texture2D bottomRightSlice = GenerateSlice(BOTTOM_RIGHT, GetSliceIDFromTextureIndex(BOTTOM_RIGHT, textureIndex));
        Texture2D bottomLeftSlice  = GenerateSlice(BOTTOM_LEFT,  GetSliceIDFromTextureIndex(BOTTOM_LEFT,  textureIndex));
        Texture2D topLeftSlice     = GenerateSlice(TOP_LEFT,     GetSliceIDFromTextureIndex(TOP_LEFT,     textureIndex));

        return MergeTextures(topRightSlice, bottomRightSlice, bottomLeftSlice, topLeftSlice);
    }

    /// <summary>
    /// Creates all textures in the asset database and initializes them to the textures array
    /// </summary>
    /// <returns> The Easy Rule Tile's texture array </returns>
    public Sprite[] GenerateTextures()
    {
        Rect CreateRect(Texture2D texture) { return new Rect(0, 0, texture.width, texture.height); }
        Vector2 CreatePivot(Texture2D texture) { return new Vector2(.5f, .5f); }
        Sprite CreateSprite(Texture2D texture) { return Sprite.Create(texture, CreateRect(texture), CreatePivot(texture), GlobalManager.PIXELS_PER_UNIT); }

        Sprite[] textures = new Sprite[0b_101111];


        string[] folders = AssetDatabase.GetSubFolders(PATH_TO_RULE_TILE_TEXTURES);
        bool folderExists = false;
        foreach (string folder in folders)
            if (folder == PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures") folderExists = true;
        if (!folderExists) AssetDatabase.CreateFolder(PATH_TO_RULE_TILE_TEXTURES, name + " Textures");

        //Sprite sprite;
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = CreateSprite(GenerateTexture(i));

            //Creates the textures in the Asset Database
            byte[] bytes = textures[i].texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + PATH_TO_RULE_TILE_TEXTURES_WITHOUT_ASSETS + "/" + name + " Textures/" + name + "_" + i.ToString() + ".png", bytes);

            // Code to create the textures in the Asset Database that doesn't work; kept it here because it interesting in how it doesn't work. Comment out above code and uncomment
            // this code to see what I'm referring to.
            //AssetDatabase.CreateAsset(textures[i], PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures/" + name + "_" + i.ToString() + ".png");
            //AssetDatabase.CreateAsset(textures[i], PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures/" + name + "_" + i.ToString() + ".asset");
            //textures[i] = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_TO_RULE_TILE_TEXTURES + "/" + name + " Textures/" + name + "_" + i.ToString() + ".png");

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return textures;
    }


    /// <summary>
    /// Convert a directional ID into a 2-bit directional ID.
    ///    TOP       = (0, 0)
    ///    LEFT      = (0, 1)
    ///    BOTTOM    = (1, 0)
    ///    RIGHT     = (1, 1)
    ///    TOP_RIGHT = (0, 0)
    /// BOTTOM_RIGHT = (0, 1)
    /// BOTTOM_LEFT  = (1, 0)
    ///    TOP_LEFT  = (1, 1) 
    /// </summary>
    /// <param name="directionID"> The integer directional ID we wish to convert to a 2-bit directional ID. </param>
    /// <returns> The 2-bit directional ID, where false = 0 and true = 1. </returns>
    public static (bool, bool) Get2BitDirectionalID(int directionID)
    {
        bool bit2 = directionID == BOTTOM_LEFT  || directionID == TOP_LEFT || directionID == BOTTOM || directionID == LEFT;
        bool bit1 = directionID == BOTTOM_RIGHT || directionID == TOP_LEFT || directionID == RIGHT  || directionID == LEFT;
        return (bit2, bit1);
    }

    public static int GetDiagonalDirectionalIDFrom2BitDirectionalID(bool bit2, bool bit1)
    {
        return bit2 ? bit1 ? TOP_LEFT : BOTTOM_LEFT :
                      bit1 ? BOTTOM_RIGHT : TOP_RIGHT;
    }

    public static int GetOrthogonalDirectionalIDFrom2BitDirectionalID(bool bit2, bool bit1)
    {
        return bit2 ? bit1 ? LEFT : BOTTOM :
                      bit1 ? RIGHT : TOP;
    }

#endregion


}
