namespace Logical;

public interface IFixable
{
    public enum FidelityLevel
    {
        Faithful,
        Intended,
        Fixed,
        Remastered
    }
    
    protected FidelityLevel Fidelity { get; }
    public bool ShallFix(FidelityLevel fidelity) => fidelity >= Fidelity; 
    void Fix(FidelityLevel fidelity);
}