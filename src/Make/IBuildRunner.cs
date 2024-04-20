namespace Make;

public interface IBuildRunner
{
    IEnumerable<string> GetGlobs();
    Task<int> Run(BuildContext context);
}