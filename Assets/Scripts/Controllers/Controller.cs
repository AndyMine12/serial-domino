using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public static Dictionary<string, Controller> active = new Dictionary<string, Controller>();
    public string identifier;

    protected virtual void Start() {
        try 
        {
            Controller.active.Add(identifier, this);
        }
        catch (System.Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
            Debug.Log(this.GetType().ToString() + " " + this.identifier + " failed to activate. Now deleting");
            Destroy(this.gameObject);
        }
    }

    protected void OnDestroy() {
        Controller.active.Remove(this.identifier);
    }
}
