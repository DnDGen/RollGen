using D20Dice.Bootstrap.Modules;
using Ninject;

namespace D20Dice.Bootstrap
{
    public class D20DiceModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<CoreModule>();
        }
    }
}