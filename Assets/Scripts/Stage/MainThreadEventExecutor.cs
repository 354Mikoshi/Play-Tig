using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインスレッドでイベントを実行する
/// </summary>
public sealed class MainThreadEventExecutor : MonoBehaviour
{
    static MainThreadEventExecutor instance;

    static List<Action> queues = new List<Action>();
    static volatile bool eventQueueEmpty = true;

    public static void Initialize() {
        if (instance == null) {
            instance = new GameObject("MainThreadEventExecutor").AddComponent<MainThreadEventExecutor>();
            DontDestroyOnLoad(instance.gameObject);
        }
    }

    public static void Execute(Action action) {
        lock (queues) {
            queues.Add(action);
            eventQueueEmpty = false;
        }
    }

    public void Update() {
        if (eventQueueEmpty) {
            return;
        }

        var queue = new List<Action>();

        lock (queues) {
            queue.AddRange(queues);
            queues.Clear();
            eventQueueEmpty = true;
        }

        foreach (Action ev in queue) {
            ev.Invoke();
        }
    }
}