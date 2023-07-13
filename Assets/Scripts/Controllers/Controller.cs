using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public static Dictionary<string, Controller> active = new Dictionary<string, Controller>();
    public string identifier;

    public static T GetActiveController<T>(string identifier) where T:Controller
    {
        Controller candidate = Controller.active[identifier];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Controller '" + identifier + "' not found");}
        if (candidate.GetType() != typeof(T))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as '" + identifier + "' is not a " + typeof(T).ToString());
        }
        T request = (T)candidate;
        return request;
    }

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
