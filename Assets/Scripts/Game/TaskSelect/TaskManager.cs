using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : Manager<TaskManager>
{
    Pool<Task_UI> task_pool;
    List<Task_UI> task_list;
    int current;

    void Awake()
    {
        Task_UI.ResetID();
        task_pool = new(20,GameObject.Find("TaskGroup").transform);
        task_list = new();
        Init();
    }

    public void Init()
    {
        List<EnemyGroup> list = DataManager.instance.RandomGetEnemyGroupList(1);
        current = 0;
        foreach(var v in list)
        {
            Task_UI task = task_pool.Get(Vector3.zero);
            task.Init(v);
            task_list.Add(task);
        }

        for(int i = 0; i < 2; i++)
        {
            task_list[current].MoveUp();
        }
        if (current + 1 < task_list.Count)
        {
            task_list[current+1].MoveUp();
        }
    }

    public void MoveUp()
    {
        for(int i = 0; i < 3; i++)
        {
            int index = i+current;
            if(index>=task_list.Count)index-=task_list.Count;
            task_list[index].MoveUp();
        }
        current++;
        if (current > task_list.Count)
        {
            current-=task_list.Count;
        }
    }
}
