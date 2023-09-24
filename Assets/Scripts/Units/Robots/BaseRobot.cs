
public class BaseRobot : BaseUnit
{
    public UnitDirection direction;
    private bool _isStunned;
    public bool IsStunned => _isStunned;

    public override void GetShot(Faction faction)
    {
        if(Faction != faction)
        {
            _isStunned = true;
        }
    }

    public override void ClearShot()
    {
        _isStunned = false;
    }
}
