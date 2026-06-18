using System.IO;
using System.Reflection;

namespace DuckGame.UFFMod
{
    internal class UIMenuActionConvertMap : UIMenuActionCloseMenu
    {
        public UIMenuActionConvertMap(UIComponent _menu)
            : base(_menu)
        {
        }

        public override void Activate()
        {
            Editor editor = Level.current as Editor;
            if (editor != null)
            {
                if (editor.saveName == "")
                    UffMod.Main.DevString = "SAVE THE MAP FIRST";
                else
                {
                    Editor.saving = true;
                    LevelData saveData = editor.CreateSaveData();

                    foreach (BinaryClassChunk node in saveData.objects.objects)
                    {
                        bool modified = false;
                        FieldInfo _extraPropertiesField = typeof(BinaryClassChunk).GetField("_extraProperties", BindingFlags.NonPublic | BindingFlags.Instance);
                        MultiMap<string, object> properties = _extraPropertiesField.GetValue(node) as MultiMap<string, object>;
                        string typeString = node.GetProperty<string>("type");
                        string containsString = node.GetProperty<string>("contains");
                        string uffModPart = ", UFFMod";
                        if (typeString != null)
                        {
                            int typeCommaIndex = typeString.IndexOf(',');
                            Mod.Debug.Log("Type: " + typeString);
                            if (typeString.Substring(typeCommaIndex) == ", UFFMod-dev")
                            {
                                string fixedTypeString = typeString.Substring(0, typeString.IndexOf(uffModPart) + uffModPart.Length);
                                properties.Remove("type");
                                properties.Add("type", fixedTypeString);
                                Mod.Debug.Log("Fixed type to: " + fixedTypeString);
                                modified = true;
                            }
                        }
                        if (containsString != null && containsString != "")
                        {
                            int containsCommaIndex = containsString.IndexOf(',');
                            Mod.Debug.Log("Contains: " + containsString);
                            if (containsString.Substring(containsCommaIndex) == ", UFFMod-dev")
                            {
                                string fixedContainsString = containsString.Substring(0, containsString.IndexOf(uffModPart) + uffModPart.Length);
                                properties.Remove("contains");
                                properties.Add("contains", fixedContainsString);
                                Mod.Debug.Log("Fixed contains to: " + fixedContainsString);
                                modified = true;
                            }
                        }
                        if (modified)
                            _extraPropertiesField.SetValue(node, properties);
                    }

                    saveData.modData.workshopIDs.Remove(665788625);
                    saveData.modData.workshopIDs.Add(566393556);
                    saveData.modData.hasLocalMods = false;
                    saveData.SetPath(editor.saveName);
                    DuckFile.SaveChunk(saveData, editor.saveName);
                    Content.MapLevel(saveData.metaData.guid, saveData, LevelLocation.Custom);
                    if (editor.additionalSaveDirectory != null && editor.saveName.LastIndexOf("assets/levels/") != -1)
                    {
                        File.Copy(editor.saveName, Directory.GetCurrentDirectory() + "/Content/levels/" + editor.saveName.Substring(editor.saveName.LastIndexOf("assets/levels/") + "assets/levels/".Length), true);
                        File.SetAttributes(editor.saveName, FileAttributes.Normal);
                    }
                    if (editor._miniMode)
                        LevelGenerator.ReInitialize();
                    foreach (Thing thing in editor.levelThings)
                        thing.processedByEditor = false;
                    Editor.saving = false;
                    UffMod.Main.DevString = "DONE: " + editor.saveName;
                }
            }
            else
                UffMod.Main.DevString = "NOT IN EDITOR";

            base.Activate();
        }
    }
}
