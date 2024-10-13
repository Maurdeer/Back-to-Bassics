public interface IAttackRequester
{
    /// <summary>
    /// When attack does go through
    /// </summary>
    /// <param name="receiver"></param>
    public void OnAttackMaterialize(IAttackReceiver receiver);

    /// <summary>
    /// Short time period after the attack impact that still allows for deflection
    /// </summary>
    /// <returns>Length in beats</returns>
    public float GetDeflectionCoyoteTime();

    /// <summary>
    /// OnUpdate function to be called each frame between hit and coyote time end (either dodged or materialized)
    /// </summary>
    /// <param name="state"></param>
    /// <param name="ctx"></param>
    public virtual void OnUpdateDuringCoyoteTime(Conductor.ConductorSchedulableState state,
        Conductor.ConductorContextState ctx) { }

    /// <summary>
    /// When the attack requested is challenged by the receiver
    /// </summary>
    /// <param name="receiver"></param>
    public bool OnRequestDeflect(IAttackReceiver receiver);
    /// <summary>
    /// When the attack requeseted is denied by the receiver
    /// </summary>
    /// <param name="receiver"></param>
    public bool OnRequestBlock(IAttackReceiver receiver);
    /// <summary>
    /// When the attack requested is avoided by the receiver
    /// </summary>
    /// <param name="receiver"></param>
    public bool OnRequestDodge(IAttackReceiver receiver);
}
