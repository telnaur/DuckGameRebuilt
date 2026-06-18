namespace DuckGame.HaloWeapons
{
    public sealed class OptionsData : DataClass
    {
        public OptionsData()
        {
            _nodeName = "Options";
        }

        public bool ShareSkins { get; set; } = true;
        public bool IgnoreOnlineSkins { get; set; } 
    }
}
