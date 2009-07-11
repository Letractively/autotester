namespace Shrinerain.AutoTester.Core
{
    public interface IHierarchy
    {
        object GetParent();
        object[] GetChildren();
        object GetChild(int childIndex);
    }
}
