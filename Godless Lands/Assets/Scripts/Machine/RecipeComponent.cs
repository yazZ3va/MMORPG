﻿using Items;
using Recipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Machines
{
    public class RecipeComponent : MonoBehaviour
    {
        public Text expandTxt;
        public Text countTxt;
        public Image icon;
        public Text nameTxt;
        public LayoutElement element;
        public RectTransform pointVertical;
        public RectTransform pointHorizontal;
        private bool expand = false;
        private Item item;
        private int allCount;
      //  private int count;
        private Recipe nextRecipe;
        private Workbench workbench;
        private List<RecipeComponent> children;
        private bool child = false;
        private Camera mainCamera;
        public bool active = false;

        public void Start()
        {
            if (active) return;
            children = new List<RecipeComponent>();
            transform.localScale = Vector3.one;
            workbench = GetComponentInParent<Workbench>();
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            active = true;
        }

        public bool IsChild() { return child; }
        //Возвращает разницу между этим и последним дочерним компонентом по Y, если дочерних копмпонентов нет 0
        public float GetVertical()
        {
           // return children.Count * 50.0f;
            if (children.Count < 1 || !children[children.Count - 1].active) return 0.0f;
            return pointVertical.position.y - children[children.Count - 1].GetPoint();
        }
        //Возвращает позицию этого элемента по У
        public float GetPoint()
        {
            // if (mainCamera == null) return 0.0f;
            //  print("camera: "+ (mainCamera == null));
            //  print("pointHorizontal: " + (pointHorizontal == null) );
            return pointHorizontal.position.y;//mainCamera.WorldToScreenPoint(pointHorizontal.position).y;
        }

        public Item GetItem()
        {
            return item;
        }
        public int GetCount()
        {
            return allCount;
        }
        public void Expand() 
        {
            expand = !expand;
            if (nextRecipe == null) return;
            if (expand) { expandTxt.text = "-"; CreateChilds(); }
            else { expandTxt.text = "+"; DestroyChilds(); }
        }

        public void Refresh()
        {
            allCount = Inventory.GetAllCount(item.id);
            if (item.count != 0)
                countTxt.text = " (" + item.count + "/" + allCount + ") ";//Количество необходимое для создание предыдущего компонета, количество в рюкзаке
            foreach (RecipeComponent component in children)
            {
                component.Refresh();
            }
        }

        public void Destroy()
        {
            DestroyChilds();
            Destroy(gameObject);
        }
        private void DestroyChilds()
        {
            foreach(RecipeComponent component in children)
            {
                component.Destroy();
            }
            children.Clear();
        }
        private void CreateChilds()
        {
            int index = transform.GetSiblingIndex();
            foreach (Piece piece in nextRecipe.component)
            {
                GameObject obj = Instantiate(workbench.recipeComponentPref);
                obj.transform.SetParent(transform.parent);
                obj.transform.SetSiblingIndex(++index);
                RecipeComponent component = obj.GetComponent<RecipeComponent>();
                component.SetPiece(piece, element);
                children.Add(component);
            }
            foreach (Piece piece in nextRecipe.fuel)
            {
                GameObject obj = Instantiate(workbench.recipeComponentPref);
                obj.transform.SetParent(transform.parent);
                obj.transform.SetSiblingIndex(++index);
                RecipeComponent component = obj.GetComponent<RecipeComponent>();
                component.SetPiece(piece,element, true);
                children.Add(component);
            }
        }

        public void SetPiece(Piece piece,LayoutElement parenlayout = null, bool fuel = false)
        {
            item = Inventory.CreateItem(piece.ID);
            if (parenlayout != null)//Если есть потомок
            {
                element.minWidth = parenlayout.minWidth + 45.0f;
                child = true;
            }
            nextRecipe = Workbench.GetRecipeByResult(piece.ID);//Поиск состоит ли этот компонент из компонентов
            
            if (nextRecipe != null)//Если состоит
                expandTxt.text = "+";

            allCount = Inventory.GetAllCount(piece.ID);
            item.count = piece.count;
            if (item.count != 0)
                 countTxt.text = " (" + allCount + "/" + item.count + ") ";//Количество необходимое для создание предыдущего компонета, количество в рюкзаке
            else countTxt.text = " ";

           
            icon.sprite = Sprite.Create(item.texture, new Rect(0.0f, 0.0f, item.texture.width, item.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            nameTxt.text = item.nameItem;

            if(fuel) nameTxt.text += " (топливо)";

            if (nextRecipe != null)
            {
                switch (nextRecipe.use)
                {
                    case MachineUse.Smelter:
                        nameTxt.text += " (плавильня)";
                        break;
                    case MachineUse.Grindstone:
                        nameTxt.text += " (точильный камень)";
                        break;
                }
            }
           
        }
    }
}