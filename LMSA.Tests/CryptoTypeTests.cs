using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class CryptoTypeTests
{
    [Fact]
    public void CryptoType_HasCorrectValues()
    {
        Assert.Equal(0, (int)CryptoType.Arc4);
        Assert.Equal(1, (int)CryptoType.Aes);
    }
}
