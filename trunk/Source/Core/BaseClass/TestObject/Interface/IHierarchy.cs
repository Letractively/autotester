namespace Shrinerain.AutoTester.Core
{
    public interface IHierarchy
    {
        object GetParent();
        object[] GetChildren();
        int GetChildCount();
        object GetChild(int childIndex);
    }
}
