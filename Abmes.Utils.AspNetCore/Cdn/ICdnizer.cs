namespace Abmes.Utils.AspNetCore.Cdn;

public interface ICdnizer<T> where T : class
{
    string this[string relativePath] { get; }
}
