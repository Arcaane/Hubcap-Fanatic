using System.Collections.Generic;
using System.Linq;
using Helper;

public class UpdateManager : Singleton<UpdateManager> {
    #region Variables
    private List<IUpdate> iUpdates = new();
    private List<IFixedUpdate> iFixedUpdates = new();
    #endregion Variables

    #region Registration
    /* Register IUpdate and IFixedUpdate elements */
    public void RegisterIUpdate(IUpdate update) => iUpdates.Add(update);
    public void RegisterIFixedUpdate(IFixedUpdate update) => iFixedUpdates.Add(update);
    
    /* UnRegister IUpdate and IFixedUpdate elements */
    public void UnRegisterIUpdate(IUpdate update) => iUpdates.Remove(update);
    public void UnRegisterIFixedUpdate(IFixedUpdate update) => iFixedUpdates.Remove(update);
    #endregion Registration
    
    /// <summary>
    /// Call all UpdateTick Method inside this Update
    /// </summary>
    private void Update() {
        foreach (IUpdate iUpdate in iUpdates.ToList()) {
            if (!iUpdates.Contains(iUpdate)) continue;
            iUpdate.UpdateTick();
        }
    }
    
    /// <summary>
    /// Call all FixedUpdateTick Method inside this FixedUpdate
    /// </summary>
    private void FixedUpdate() {
        foreach (IFixedUpdate iFixedUpdate in iFixedUpdates.ToList()) {
            if (!iFixedUpdates.Contains(iFixedUpdate)) continue;
            iFixedUpdate.FixedUpdateTick();
        }
    }
}