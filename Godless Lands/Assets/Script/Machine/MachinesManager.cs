﻿using Machines;
using RUCP;
using RUCP.Handler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinesManager : MonoBehaviour, Manager
{
    public GameObject smelterPref;
    public GameObject grindstonePref;
    public GameObject workbenchPref;
    public GameObject tanneryPref;
    private static Dictionary<int, GameObject> machines;



    private void Awake()
    {
        RegisteredTypes.RegisterTypes(Types.MachineCreate, MachineCreate);
        RegisteredTypes.RegisterTypes(Types.MachineDelete, MachineDelete);
    }



    private void Start()
    {

        machines = new Dictionary<int, GameObject>();
    }

    public void AllDestroy()
    {
        foreach (GameObject obj in machines.Values)
            Destroy(obj);
        machines.Clear();

    }

    private void MachineDelete(NetworkWriter nw)
    {
        int id = nw.ReadInt();
        if (machines.ContainsKey(id))
        {
            Destroy(machines[id]);
            machines.Remove(id);
        }
    }

    private void MachineCreate(NetworkWriter nw)
    {
        int id = nw.ReadInt();
        MachineUse machineUse = (MachineUse) nw.ReadByte();
        Vector3 postion = nw.ReadVec3();
        Vector3 rotation = nw.ReadVec3();
        if (machines.ContainsKey(id)) { print("error create machine"); return; } //Если монстр с таким ид уже создан


        //  GameObject prefabResource = resourceList.GetPrefab(idSkin);
        GameObject prefab;
        switch (machineUse)
        {
            case MachineUse.Smelter:
                prefab = smelterPref;
                break;
            case MachineUse.Grindstone:
                prefab = grindstonePref;
                break;
            case MachineUse.Workbench:
                prefab = workbenchPref;
                break;
            case MachineUse.Tannery:
                prefab = tanneryPref;
                break;
            default:
                print("Error machine skin: ");
                return;
        }
     

        GameObject _obj = Instantiate(prefab, postion, Quaternion.Euler(rotation));
        _obj.transform.SetParent(transform);

        Action action = _obj.GetComponent<Action>();
        action.SetID(id);
        machines.Add(id, _obj);
    }


    private void OnDestroy()
    {
        RegisteredTypes.UnregisterTypes(Types.MachineCreate);
        RegisteredTypes.UnregisterTypes(Types.MachineDelete);
    }
}
