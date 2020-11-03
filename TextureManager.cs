using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using static DirectionAPI;

public static class TextureManager
{
    /// <summary>
    /// Creates a reformatted copy of the inputed texture with the ARG32 Texture Format where the alpha channel is transparency. 
    /// </summary>
    /// <param name="texture"> The original texture </param>
    /// <returns> The reformatted copy of the texture </returns>
    public static Texture2D CreateTextureWithTransparentBackground(Texture2D texture)
    {
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        newTexture.alphaIsTransparency = true;
        newTexture.filterMode = FilterMode.Point;
        newTexture.alphaIsTransparency = true;
        Color[] pixels = texture.GetPixels();
        newTexture.SetPixels(pixels);
        newTexture.Apply();
        return newTexture;
    }

    /// <summary>
    /// Creates new texture with the ARG32 Texture Format where the alpha channel is transparency. 
    /// </summary>
    /// <param name="width"> The width of the new texture </param>
    /// <param name="height"> The height of the new texture </param>
    /// <returns> The new fully transparent texture with the given dimensions </returns>
    public static Texture2D CreateTextureWithTransparentBackground(int width, int height)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        newTexture.alphaIsTransparency = true;
        newTexture.filterMode = FilterMode.Point;
        newTexture.alphaIsTransparency = true;
        Color transparent = new Color(0, 0, 0, 0);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                newTexture.SetPixel(x, y, transparent);
        newTexture.Apply();
        return newTexture;
    }

    /// <summary>
    /// Takes a texture as an input and then uses the other parameters to determine a section of this texture that will be returned in a seperate Texture2D.
    /// </summary>
    /// <param name="texture"> The base texture, the texture to be cropped. </param>
    /// Version 1 Specific Parameters:
    /// <param name="fromX"> The number of pixels from the base texture's left edge to the x-coordinate of the bottom left corner of the cropped section </param>
    /// <param name="toX"> The number of pixels from the base texture's right edge to the x-coordinate of the top right corner of the cropped section </param>
    /// <param name="fromY"> The number of pixels from the base texture's bottom edge to the y-coordinate of the bottom left corner of the cropped section </param>
    /// <param name="toY"> The number of pixels from the base texture's top edge to the y-coordinate of the top right corner of the cropped section </param>
    /// Version 2 Specific Parameters:
    /// <param name="topBoundPercentage"> The percentage of the base texture from the top edge downwards that will be included in the cropped texture. </param>
    /// <param name="rightBoundPercentage"> The percentage of the base texture from the right edge leftwards that will be included in the cropped texture</param>
    /// <param name="bottomBoundPercentage"> The percentage of the base texture from the left edge rightwards that will be included in the cropped texture</param>
    /// <param name="leftBoundPercentage"> The percentage of the base texture from the down edge upwards that will be included in the cropped texture</param>
    /// Version 3 Specific Parameters:
    /// <param name="direction"> The edge of the base direction that the cropped section begins </param>
    /// <param name="cropPercentage"> The percentage of the base texture from the edge determined by 'direction' towards the opposite edge
    ///                               that will be included in the cropped texture </param>
    /// <returns> The cropped texture. </returns>
    public static Texture2D CropTexture(Texture2D texture, int fromX, int toX, int fromY, int toY)
    {
        Texture2D newTexture = CreateTextureWithTransparentBackground(toX - fromX, toY - fromY);
        for (int x = fromX; x < toX; x++)
            for (int y = fromY; y < toY; y++)
                newTexture.SetPixel(x - fromX, y - fromY, texture.GetPixel(x, y));

        newTexture.Apply();

        return newTexture;
    }
    public static Texture2D CropTexture(Texture2D texture, float leftBoundPercentage, float rightBoundPercentage, float bottomBoundPercentage, float topBoundPercentage)
    {
        int width = texture.width;
        int height = texture.height;
        float topBound = (texture.height * topBoundPercentage);
        float bottomBound = (texture.height * bottomBoundPercentage);
        float rightBound = (texture.width * rightBoundPercentage);
        float leftBound = (texture.width * leftBoundPercentage);

        return CropTexture(texture, (int)leftBound, (int)rightBound, (int)bottomBound, (int)topBound);
    }
    public static Texture2D CropTexture(Texture2D texture, int direction, int pixelsToCrop)
    {
        int width = texture.width;
        int height = texture.height;

        switch (direction)
        {
            case TOP: return CropTexture(texture, 0, width, height - pixelsToCrop, height);
            case TOP_RIGHT: return CropTexture(texture, width - pixelsToCrop, width, height - pixelsToCrop, height);
            case RIGHT: return CropTexture(texture, width - pixelsToCrop, width, 0, height);
            case BOTTOM_RIGHT: return CropTexture(texture, width - pixelsToCrop, width, 0, pixelsToCrop);
            case BOTTOM: return CropTexture(texture, 0, width, 0, pixelsToCrop);
            case BOTTOM_LEFT: return CropTexture(texture, 0, pixelsToCrop, 0, pixelsToCrop);
            case LEFT: return CropTexture(texture, 0, pixelsToCrop, 0, height);
            case TOP_LEFT: return CropTexture(texture, 0, pixelsToCrop, height - pixelsToCrop, height);
            case CENTER: return CropTexture(texture, pixelsToCrop, width - pixelsToCrop, pixelsToCrop, height - pixelsToCrop);
            case HORIZONTAL: return CropTexture(texture, 0, width, pixelsToCrop, height - pixelsToCrop);
            case VERTICAL: return CropTexture(texture, pixelsToCrop, width - pixelsToCrop, 0, height);
            default: return null;
        }
    }
    public static Texture2D CropTexture(Texture2D texture, int direction, float cropPercentage = .5f)
    {
        int width = texture.width;
        int height = texture.height;
        int croppedWidth = (int)(texture.width * cropPercentage);
        int croppedHeight = (int)(texture.height * cropPercentage);

        switch (direction)
        {
            case TOP: return CropTexture(texture, 0, width, height - croppedHeight, height);
            case TOP_RIGHT: return CropTexture(texture, width - croppedWidth, width, height - croppedHeight, height);
            case RIGHT: return CropTexture(texture, width - croppedWidth, width, 0, height);
            case BOTTOM_RIGHT: return CropTexture(texture, width - croppedWidth, width, 0, croppedHeight);
            case BOTTOM: return CropTexture(texture, 0, width, 0, croppedHeight);
            case BOTTOM_LEFT: return CropTexture(texture, 0, croppedWidth, 0, croppedHeight);
            case LEFT: return CropTexture(texture, 0, croppedWidth, 0, height);
            case TOP_LEFT: return CropTexture(texture, 0, croppedWidth, height - croppedHeight, height);
            case CENTER: return CropTexture(texture, width - croppedWidth, croppedWidth, height - croppedHeight, croppedHeight);
            case HORIZONTAL: return CropTexture(texture, 0, width, height - croppedHeight, croppedHeight);
            case VERTICAL: return CropTexture(texture, width - croppedWidth, croppedWidth, 0, height);
            default: return null;
        }
    }

    /// <summary>
    /// Returns a ninth of the texture's total area- specially by viewing the texture as a 3x3 grid and returns the texture in one of those rectangles
    /// </summary>
    /// <param name="texture"> The base texture we are attempting to get a cropped image of </param>
    /// <param name="direction"> The location of which rectangles in the 3x3 grid is used, in relation to the center of the texture </param>
    /// <returns> A new texture that copies a ninth of the inputted texture </returns>
    public static Texture2D CropTextureNinth(Texture2D texture, int direction)
    {

        //AssetDatabase.CreateAsset(CreateTextureWithTransparentBackground(texture), "Assets/TestA.asset");
        float fromXPercentage = direction == TOP_LEFT || direction == LEFT || direction == BOTTOM_LEFT || direction == HORIZONTAL ? 0f :
                                direction == TOP || direction == CENTER || direction == BOTTOM || direction == VERTICAL ? 1f / 3f :
                              /*direction == TOP_RIGHT || direction == RIGHT  || direction == BOTTOM_RIGHT                           */ 2f / 3f;
        float toXPercentage = direction == TOP_LEFT || direction == LEFT || direction == BOTTOM_LEFT ? 1f / 3f :
                                direction == TOP || direction == CENTER || direction == BOTTOM || direction == VERTICAL ? 2f / 3f :
                              /*direction == TOP_RIGHT || direction == RIGHT  || direction == BOTTOM_RIGHT || direction == HORIZONTAL*/ 1f;

        float fromYPercentage = direction == BOTTOM_LEFT || direction == BOTTOM || direction == BOTTOM_RIGHT || direction == VERTICAL ? 0f :
                                direction == LEFT || direction == CENTER || direction == RIGHT || direction == HORIZONTAL ? 1f / 3f :
                              /*direction == TOP_LEFT    || direction == TOP    || direction == TOP_RIGHT                              */ 2f / 3f;
        float toYPercentage = direction == BOTTOM_LEFT || direction == BOTTOM || direction == BOTTOM_RIGHT ? 1f / 3f :
                                direction == LEFT || direction == CENTER || direction == RIGHT || direction == HORIZONTAL ? 2f / 3f :
                              /*direction == TOP_LEFT    || direction == TOP    || direction == TOP_RIGHT    || direction == VERTICAL  */ 1f;


        //AssetDatabase.CreateAsset(CropTexture(texture, toYPercentage, toXPercentage, fromYPercentage, fromXPercentage), "Assets/TestB.asset");
        return CropTexture(texture, fromXPercentage, toXPercentage, fromYPercentage, toYPercentage);
    }

    /// <summary>
    /// Creates a new texture by adjoining two textures in a given direction. If the side the textures are adjoined at has a different length for each
    /// texture, then an empty "allignment" margin is added to a copy of the texture with a shorter side, so that the side of the resulting texture has the
    /// same length as the texture  with a longer side. If forceBothComponentsToHaveTheSameDimensions is true and the two textures are inequal in the length
    /// of the other axis, then another margin is added so that both textures have equivalent lengths along both axises.
    /// </summary>
    /// <param name="baseTexture"> The texture that will serve as the base of the merged texture </param>
    /// <param name="textureToAdjoin"> The texture that will be adjoined to the base texture </param>
    /// <param name="directionToAppend"> The direction textureToAdjoin is adjoined to baseTexture in relation to baseTexture </param>
    /// <param name="allignment"> In what direction the allignment margin will be in relation to the shorter texture. If allignment == CENTER, then two
    ///                           margins of equal distance will be added to either side of the shorter texture </param>
    /// <param name="forceBothComponentsToHaveTheSameDimensions"> If true and both textures are of unequal dimensions after the allignmnet margin is added,
    ///                                                           then a new margin with be added to make them equal in the remaining dimension </param>
    /// <returns> A new texture that contains both the baseTexture and the textureToAdjoin </returns>
    public static Texture2D MergeTextures(Texture2D baseTexture, Texture2D textureToAdjoin, int directionToAdjoin, int allignment = CENTER,
                                               bool forceBothComponentsToHaveTheSameDimensions = false)
    {


        bool baseTextureIsTaller = baseTexture.height > textureToAdjoin.height;
        bool baseTextureIsWider = baseTexture.width > textureToAdjoin.width;
        bool appendedInVerticalDirection = directionToAdjoin == TOP || directionToAdjoin == BOTTOM;
        bool baseTextureRequiresAllignmentMargin = (appendedInVerticalDirection && !baseTextureIsWider) || (!appendedInVerticalDirection && !baseTextureIsTaller);
        int directionOfAllignmentMargin = allignment == CENTER ? CENTER : GetOppositeDirection(allignment);

        int lengthOfAllignmentMargin = directionToAdjoin == TOP || directionToAdjoin == BOTTOM ? Mathf.Abs(baseTexture.width - textureToAdjoin.width) :
                                       directionToAdjoin == LEFT || directionToAdjoin == RIGHT ? Mathf.Abs(baseTexture.height - textureToAdjoin.height) :
                                       directionToAdjoin == CENTER && appendedInVerticalDirection ? Mathf.Abs(baseTexture.width - textureToAdjoin.width) / 2 :
                                     /*directionToAppend == CENTER && !appendedInVerticalDirection*/ Mathf.Abs(baseTexture.height - textureToAdjoin.height) / 2;

        //Adding the Allignment Margin
        Texture2D baseTextureCopy = CreateTextureWithTransparentBackground(baseTexture);
        Texture2D textureToAppendCopy = CreateTextureWithTransparentBackground(textureToAdjoin);
        Texture2D modifiedTexture = baseTextureRequiresAllignmentMargin ? baseTextureCopy : textureToAppendCopy;

        if (allignment == CENTER)
        {
            int halfMarginLength = lengthOfAllignmentMargin / 2;
            modifiedTexture = AddMarginToTexture(modifiedTexture, GetClockwiseDirection(directionToAdjoin, 2), halfMarginLength);
            modifiedTexture = AddMarginToTexture(modifiedTexture, GetClockwiseDirection(directionToAdjoin, -2), lengthOfAllignmentMargin - halfMarginLength);
        }
        else
        {
            modifiedTexture = AddMarginToTexture(modifiedTexture, directionOfAllignmentMargin, lengthOfAllignmentMargin);
        }



        if (baseTextureRequiresAllignmentMargin) baseTextureCopy = modifiedTexture;
        else textureToAppendCopy = modifiedTexture;

        //Force Both Components To Have The Same Dimensions
        if (forceBothComponentsToHaveTheSameDimensions && ((baseTextureCopy.height != textureToAppendCopy.height) ||
                                                           (baseTextureCopy.width != textureToAppendCopy.width)))
        {
            int baseTextureLength = baseTextureCopy.height != textureToAppendCopy.height ? baseTextureCopy.height : baseTextureCopy.width;
            int textureToAppendLength = baseTextureCopy.height != textureToAppendCopy.height ? textureToAppendCopy.height : textureToAppendCopy.width;
            int marginLength = Mathf.Abs(baseTextureLength - textureToAppendLength);
            int marginDirection = baseTextureLength > textureToAppendLength ? directionToAdjoin : GetOppositeDirection(directionToAdjoin);

            if (baseTextureLength < textureToAppendLength) baseTextureCopy = AddMarginToTexture(baseTextureCopy, marginDirection, marginLength);
            else textureToAppendCopy = AddMarginToTexture(textureToAppendCopy, marginDirection, marginLength);
        }



        return MergeTexturesHelper(baseTextureCopy, textureToAppendCopy, directionToAdjoin);
    }

    /// <summary>
    /// Merges four textures into a rectangle where each texture forms a quadrent of the merged texture  
    /// </summary>
    /// <param name="topRight"> The texture to fill the merged texture's top right quadrent </param>
    /// <param name="bottomRight"> The texture to fill the merged texture's bottom right quadrent </param>
    /// <param name="bottomLeft"> The texture to fill the merged texture's bottom left quadrent </param>
    /// <param name="topLeft"> The texture to fill the merged texture's top left quadrent </param>
    /// <returns> The merged textured </returns>
    public static Texture2D MergeTextures(Texture2D topRight, Texture2D bottomRight, Texture2D bottomLeft, Texture2D topLeft)
    {
        Texture2D top = MergeTextures(topLeft, topRight, RIGHT, BOTTOM, true);
        Texture2D bottom = MergeTextures(bottomLeft, bottomRight, RIGHT, TOP, true);
        return MergeTextures(top, bottom, BOTTOM, CENTER, true);
    }

    /// <summary>
    /// A helper method for Merge Textures, that creates a new texture by adjoining two textures in a given direction
    /// (Assumes the adjacent sides of baseTexture and textureToAppend are the same length)
    /// </summary>
    /// <param name="baseTexture"></param>
    /// <param name="textureToAdjoin"></param>
    /// <param name="directionToAdjoin"></param>
    /// <returns></returns>
    private static Texture2D MergeTexturesHelper(Texture2D baseTexture, Texture2D textureToAdjoin, int directionToAdjoin)
    {
        int width = directionToAdjoin == LEFT || directionToAdjoin == RIGHT ? baseTexture.width + textureToAdjoin.width : baseTexture.width;
        int height = directionToAdjoin == TOP || directionToAdjoin == BOTTOM ? baseTexture.height + textureToAdjoin.height : baseTexture.height;

        int baseTextureFromX = directionToAdjoin != LEFT ? 0 : textureToAdjoin.width;
        int baseTextureFromY = directionToAdjoin != BOTTOM ? 0 : textureToAdjoin.height;

        int appendedTextureFromX = directionToAdjoin != RIGHT ? 0 : baseTexture.width;
        int appendedTextureFromY = directionToAdjoin != TOP ? 0 : baseTexture.height;

        Texture2D newTexture = new Texture2D(width, height);
        newTexture = OverlayTextures(newTexture, baseTexture, baseTextureFromX, baseTextureFromY);
        return OverlayTextures(newTexture, textureToAdjoin, appendedTextureFromX, appendedTextureFromY);
    }

    /// <summary>
    /// Overwrites the pixels of one texture with another starting from an inputted position
    /// </summary>
    /// <param name="baseTexture"> The orignal texture that will have it's pixels overwritten </param>
    /// <param name="textureToOverlay"> The texture whose pixels will overwrite the other texture </param>
    /// <param name="fromX"> The x position the overwritting of the pixels will began at </param>
    /// <param name="fromY"> The y position the overwritting of the pixels will began at </param>
    /// <returns></returns>
    public static Texture2D OverlayTextures(Texture2D baseTexture, Texture2D textureToOverlay, int fromX, int fromY)
    {
        Texture2D newTexture = CreateTextureWithTransparentBackground(baseTexture);

        int toX = Mathf.Clamp(fromX + textureToOverlay.width, 0, baseTexture.width);
        int toY = Mathf.Clamp(fromY + textureToOverlay.height, 0, baseTexture.height);


        int xOffset = fromX;
        int yOffset = fromY;

        fromX = fromX < 0 ? 0 : fromX;
        fromY = fromY < 0 ? 0 : fromY;

        for (int x = fromX; x < toX; x++)
            for (int y = fromY; y < toY; y++)
                newTexture.SetPixel(x, y, textureToOverlay.GetPixel(x - xOffset, y - yOffset));
        newTexture.Apply();

        return newTexture;
    }

    /// <summary>
    /// Overwrites the pixels of one texture with another starting from an inputted position
    /// </summary>
    /// <param name="baseTexture"> The orignal texture that will have it's pixels overwritten </param>
    /// <param name="textureToOverlay"> The texture whose pixels will overwrite the other texture </param>
    /// <param name="fromX"> The x position the overwritting of the pixels will began at </param>
    /// <param name="fromY"> The y position the overwritting of the pixels will began at </param>
    /// <returns></returns>
    public static Texture2D OverlayTextures(Texture2D baseTexture, Texture2D textureToOverlay, Vector2Int basePivot, Vector2Int overlayPivot)
    {
        return OverlayTextures(baseTexture, textureToOverlay, basePivot.x - overlayPivot.x, basePivot.y - overlayPivot.y);
    }

    /// <summary>
    /// Creates a modified copy of a texture in which the copy has a margin added in a given direction
    /// </summary>
    /// <param name="texture"> The texture to have a margin added to it </param>
    /// <param name="direction"> The direction the margin will be in in relation to the texture </param>
    /// <param name="pixelsToExtend"> The length of the margin in pixels </param>
    /// <returns> The new texture with a transparent margin </returns>
    public static Texture2D AddMarginToTexture(Texture2D texture, int direction, int pixelsToExtend)
    {
        int topMarginPixels = GetVerticalDirection(direction) == TOP || direction == VERTICAL || direction == CENTER ? pixelsToExtend : 0;
        int rightMarginPixels = GetHorizontalDirection(direction) == RIGHT || direction == HORIZONTAL || direction == CENTER ? pixelsToExtend : 0;
        int bottomMarginPixels = GetVerticalDirection(direction) == BOTTOM || direction == VERTICAL || direction == CENTER ? pixelsToExtend : 0;
        int leftMarginPixels = GetHorizontalDirection(direction) == LEFT || direction == HORIZONTAL || direction == CENTER ? pixelsToExtend : 0;

        return AddMarginToTexture(texture, topMarginPixels, rightMarginPixels, bottomMarginPixels, leftMarginPixels);
    }

    /// <summary>
    /// Creates a modified copy of a texture in which the copy has a margin added in all directions 
    /// </summary>
    /// <param name="texture"> The texture to have a margin added to it </param>
    /// <param name="topMarginPixels"> The length of the top margin in pixels </param>
    /// <param name="rightMarginPixels"> The length of the right margin in pixels </param>
    /// <param name="bottomMarginPixels"> The length of the bottom margin in pixels </param>
    /// <param name="leftMarginPixels"> The length of the left margin in pixels </param>
    /// <returns> The new texture with a transparent margin </returns>
    public static Texture2D AddMarginToTexture(Texture2D texture, int topMarginPixels, int rightMarginPixels, int bottomMarginPixels, int leftMarginPixels)
    {

        int leftMarginBegin = leftMarginPixels;
        int rightMarginBegin = leftMarginPixels + texture.width;
        int width = leftMarginPixels + texture.width + rightMarginPixels;

        int bottomMarginBegin = bottomMarginPixels;
        int topMarginEnd = bottomMarginPixels + texture.height;
        int height = bottomMarginPixels + texture.height + topMarginPixels;

        Texture2D newTexture = new Texture2D(width, height);

        SetRegionTransparent(newTexture, 0, leftMarginBegin, 0, height);
        SetRegionTransparent(newTexture, rightMarginBegin, width, 0, height);
        SetRegionTransparent(newTexture, leftMarginBegin, rightMarginBegin, 0, bottomMarginBegin);
        SetRegionTransparent(newTexture, leftMarginBegin, rightMarginBegin, topMarginEnd, height);
        return OverlayTextures(newTexture, texture, leftMarginBegin, bottomMarginBegin);
    }

    /// <summary>
    /// Makes a region of a texture transparent
    /// </summary>
    /// <param name="texture"> The texture to be modified </param>
    /// <param name="fromX"> The x position of the bottom left corner of the transparent region </param>
    /// <param name="toX"> The x position of the top right corner of the transparent region</param>
    /// <param name="fromY"> The y position of the bottom left corner of the transparent region</param>
    /// <param name="toY"> The y position of the top right corner of the transparent region </param>
    public static void SetRegionTransparent(Texture2D texture, int fromX, int toX, int fromY, int toY)
    {
        for (int x = fromX; x < toX; x++)
            for (int y = fromY; y < toY; y++)
                texture.SetPixel(x, y, new Color(0, 0, 0, 0));
        texture.Apply();
    }


    public static Sprite TextureToSprite(Texture2D texture, int pixels = GlobalManager.PIXELS_PER_UNIT)
    {
        Rect    rect    = new Rect(0, 0, texture.width, texture.height);
        Vector2 vector2 = new Vector2(.5f, .5f);
        Sprite  sprite  = Sprite.Create(texture, rect, vector2, pixels);
        sprite.texture.alphaIsTransparency = true;
        return sprite;
    }

}
