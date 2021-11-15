using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThread : MonoBehaviour
{
    internal static UnityMainThread wkr;
    Queue<Action> jobs = new Queue<Action>();

    private void Awake() {
        wkr = this;
    }

    private void Update() {
        while (jobs.Count > 0) 
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob) {
        jobs.Enqueue(newJob);
    }
}
