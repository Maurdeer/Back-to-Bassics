using System;

public interface IAttackReceiver
{
    /// <summary>
    /// This method is called by a requester, and the receiver can return whether or not the requester
    /// is allowed to attack or not.
    /// </summary>
    /// <param name="requester"></param>
    /// <returns> Whether or not the requester is allowed to attack</returns> // TODO: remove the return value once refactoring is done
    public abstract bool ReceiveAttackRequest(IAttackRequester requester, Action onPendingSuccess = null, Action onPendingFail = null);
    /// <summary>
    /// Use this method when there is a timing between receiving the attack and completing the attack
    /// </summary>
    /// <param name="requester"></param>
    public abstract void CompleteAttackRequest(IAttackRequester requester);
}
