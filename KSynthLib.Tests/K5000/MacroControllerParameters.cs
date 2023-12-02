using KSynthLib.K5000;

namespace KSynthLib.Tests.K5000;

public class MacroControllerParameters
{
    private readonly MacroControllerParameter _macroControllerParameter = new MacroControllerParameter();

    public MacroControllerParameters()
    {
        // type = 2 or Level
        // depth = 43 in the range 33...95 / -31...31, or -21
        this._macroControllerParameter = new MacroControllerParameter(2, 43);
    }

    [Test]
    public void Kind_IsCorrect()
    {
        MacroControllerKind kind = this._macroControllerParameter.Kind;
        Assert.That(MacroControllerKind.Level, Is.EqualTo(kind));
    }

    [Test]
    public void Depth_IsCorrect()
    {
        int depth = this._macroControllerParameter.Depth.Value;
        Assert.That(-21, Is.EqualTo(depth));
    }
}
