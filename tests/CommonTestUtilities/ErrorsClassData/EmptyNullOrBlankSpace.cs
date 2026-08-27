using System.Collections;

namespace CommonTestUtilities.ErrorsClassData;

public class EmptyNullOrBlankSpace : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "" };
#pragma warning disable CS8625
        yield return new object[] { null };
#pragma warning restore CS8625
        yield return new object[] { "     " };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
