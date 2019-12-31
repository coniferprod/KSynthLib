using Xunit;
using KSynthLib.K5000;

namespace KSynthLib.Tests.K5000
{
    public class MacroControllerParameters
    {
        private readonly MacroControllerParameter _macroControllerParameter = new MacroControllerParameter();
        
        public MacroControllerParameters()
        {
            // type = 2 or Level
            // depth = 43 in the range 33...95 / -31...31, or -21
            this._macroControllerParameter = new MacroControllerParameter(2, 43);
        }

        [Fact]
        public void Type_IsCorrect()
        {
            MacroControllerType type = this._macroControllerParameter.Type;
            Assert.Equal(MacroControllerType.Level, type);
        }

        [Fact]
        public void Depth_IsCorrect()
        {
            int depth = this._macroControllerParameter.Depth;
            Assert.Equal(-21, depth);
        }
    }
}
