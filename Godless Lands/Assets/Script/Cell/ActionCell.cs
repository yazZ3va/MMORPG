﻿using Cells;
using Items;
using Machines;
using Recipes;
using RUCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionCell : ItemCell
{
   // private Machine machine;
  //  private Components components;
    private ItemCell itemCell;
    public bool fuel;
   // protected int count;
   // protected Text countTxt;
  //  protected Item item;

    private new void Awake()
    {
        base.Awake();
    //    machine = GetComponentInParent<Machine>();
    //    components = GetComponentInParent<Components>();
    }
  /*  public override bool IsEmpty()
    {
        if (item == null) return true;
        if (item.id <= 0) return true;
        return false;
    }*/
    
    public override void Use()
    {
      
    }
    public override void Put(Cell cell)
    {
        if (cell == null) return;
        itemCell = cell as ItemCell;
        if (itemCell == null || itemCell.IsEmpty()) return;
        //  if (components.ConstainsItem(itemCell.GetItem().id)) return;//Если этот предмет уже есть в списке
        //   PutItem(itemCell.GetItem(), itemCell.GetCount(), itemCell.GetKey());//Установить иконку
        NetworkWriter nw = new NetworkWriter(Channels.Reliable);
        nw.SetTypePack(Types.MachineAddComponent);
        nw.write(fuel);
        nw.write(index);
        nw.write(itemCell.GetObjectID());
        NetworkManager.Send(nw);

    }

    public override void PutItem(Item item, int count)
    {
        
        this.item = item;
        ClearCount();
        if (IsEmpty())
        {
            HideIcon();
            return;
        }
        item.count = count;
        ShowIcon();
        icon.sprite = Sprite.Create(item.texture, new Rect(0.0f, 0.0f, item.texture.width, item.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public void SetCount(List<Piece> pieces)
    {
        if (!IsEmpty() && item.stack)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.ID == item.id)
                {
                    if (piece.count <= item.count) countTxt.color = Color.green;
                    else countTxt.color = Color.red;
                    countTxt.text = item.count.ToString() + '/' + piece.count.ToString();
                    return;
                }
            }
        }
        ClearCount();
    }

    public void ClearCount()
    {
        countTxt.color = Color.white;
        if (!IsEmpty() && item.stack)
        {
            countTxt.text = item.count.ToString()+"/0";
        }
        else
            countTxt.text = "";
    }

   /* public int ID()
    {
        if (item == null) return 0;
        return item.id;
    }*/
    public override void Abort()
    {
        NetworkWriter nw = new NetworkWriter(Channels.Reliable);
        nw.SetTypePack(Types.MachineRemoveComponent);
        nw.write(fuel);
        nw.write(index);
        NetworkManager.Send(nw);
    }
}
