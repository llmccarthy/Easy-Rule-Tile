using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

[CustomEditor(typeof(EasyRuleTile))]
public class EasyRuleTileEditor : Editor
{

    public EasyRuleTile ruleTile;

    static GUILayoutOption GUIWidth = GUILayout.Width(250f);
    static GUILayoutOption GUIHeight = GUILayout.Height(65f);


    public override void OnInspectorGUI()
    {
        ruleTile = (EasyRuleTile)target;

        ruleTile.name = EditorGUILayout.TextField("Tile Name", ruleTile.name);
        ruleTile.tileClass    = EditorGUILayout.TextField("Tile Class", ruleTile.tileClass);
        ruleTile.standalone   = (Sprite)EditorGUILayout.ObjectField("Stand Alone Texture",  ruleTile.standalone,   typeof(Sprite), false, GUIWidth, GUIHeight);
        ruleTile.surrounded   = (Sprite)EditorGUILayout.ObjectField("Surrounded Texture",   ruleTile.surrounded,   typeof(Sprite), false, GUIWidth, GUIHeight);
        ruleTile.horizontal   = (Sprite)EditorGUILayout.ObjectField("Horizontal Texture",   ruleTile.horizontal,   typeof(Sprite), false, GUIWidth, GUIHeight);
        ruleTile.vertical     = (Sprite)EditorGUILayout.ObjectField("Vertical Texture",     ruleTile.vertical,     typeof(Sprite), false, GUIWidth, GUIHeight);
        ruleTile.intersection = (Sprite)EditorGUILayout.ObjectField("Intersection Texture", ruleTile.intersection, typeof(Sprite), false, GUIWidth, GUIHeight);

        

        if (GUILayout.Button("Generate Textures"))
        {
            ruleTile.textures = ruleTile.GenerateTextures();
        }

        EditorUtility.SetDirty(ruleTile);
        base.OnInspectorGUI();
    }
}
