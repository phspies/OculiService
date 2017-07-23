public interface IStatistics
{
    object GetStatistics(string name);

    object[] GetStatistics(string[] names);
}
