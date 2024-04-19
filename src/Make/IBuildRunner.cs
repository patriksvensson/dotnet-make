namespace Make;

public interface IBuildRunner
{
    bool CanHandle(BuildContext context);
    Task<int> Run(BuildContext context);
}