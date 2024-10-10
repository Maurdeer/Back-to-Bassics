public interface IAttackRequester
{
    /// <summary>
    /// When attack does go through
    /// </summary>
    /// <param name="receiver"></param>
    public void OnAttackMaterialize(IAttackReceiver receiver);
    
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
