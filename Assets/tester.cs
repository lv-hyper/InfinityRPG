using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class tester : MonoBehaviour
{
    [SerializeField] private GameObject g;
    void Start()
    {
        Debug.Log("Run() invoked in Start()");
        Run();
        Debug.Log("Run() returns");
    }

    void Update()
    {
        Debug.Log("Update()");
    }

    async void Run()
    {
        var task = Task.Run(() => SetActiveGameObjectFunction(g));
        await task;
    }

    void SetActiveGameObjectFunction(GameObject g)
    {
        g.SetActive(false);
    }
}
