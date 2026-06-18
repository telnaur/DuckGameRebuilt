namespace DuckGame.HaloWeapons
{
    public static class Options
    {
        private static OptionsData s_data = new OptionsData();

        public static OptionsData Data => s_data;
        private static string FileName => DuckFile.optionsDirectory + "HaloWeapons.dat";

        public static void Save()
        {
            var data = new DXMLNode("Data");
            var xml = new DuckXML();

            data.Add(s_data.Serialize());
            xml.Add(data);

            DuckFile.SaveDuckXML(xml, FileName);
        }

        public static void Load()
        {
            DuckXML xml = DuckFile.LoadDuckXML(FileName);

            if (xml is null || xml.Elements("Data") is null)
                return;

            foreach (DXMLNode data in xml.Elements("Data").Elements())
            {
                if (data.Name == "Options")
                {
                    s_data.Deserialize(data);
                    break;
                }
            }
        }
    }
}
