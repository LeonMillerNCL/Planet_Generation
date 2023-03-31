using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Water))]
public class WaterEditor : Editor
{

    Water water;
    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                water.GenerateWater();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            water.GenerateWater();
        }

        DrawSettingsEditor(water.shapeSettings, water.OnShapeSettingsUpdated, ref water.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(water.colourSettings, water.OnColourSettingsUpdated, ref water.colourSettingsFoldout, ref colourEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        water = (Water)target;
    }
}
