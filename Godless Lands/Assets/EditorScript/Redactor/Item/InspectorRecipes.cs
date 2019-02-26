﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Items
{
    public class InspectorRecipes
    {
        public static void Draw(System.Object obj)
        {
            RecipesItem recipesItem = obj as RecipesItem;
            if (recipesItem == null) return;

            recipesItem.recipeID = EditorGUILayout.IntField("Рецепт:", recipesItem.recipeID);
        }
    }
}
#endif