namespace HomeBot.ModuleLoader
{
    public class ModuleLoader
    {
        public Dictionary<string, IModule> Modules { get; private set; }

        public ModuleLoader()
        {
            Modules = new Dictionary<string, IModule>();
        }

        public void LoadModule(IModule module)
        {
            if (Modules == null) Modules = new Dictionary<string, IModule>();
            Modules.Add(module.Cmd(), module);
        }
    }
}
