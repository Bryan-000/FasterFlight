namespace FasterFlight;

using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.IO;

/// <summary> General plugin handler. </summary>
[BepInDependency("com.eternalUnion.pluginConfigurator")]
[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    /// <summary> Static plugin instance property. </summary>
    public static Plugin Instance { get; internal set; }

    /// <summary> PluginConfigurator instance for this mod :3 </summary>
    public PluginConfigurator config;

    /// <summary> Plugin config float fields for changing the speed of what you fly at. </summary>
    public FloatField flightSpeed, flightWeeeeSpeed,
        noclipSpeed, noclipWeeeeSpeed;

    /// <summary> Load the mod. </summary>
    public void Awake()
    {
        Instance = this;
        LoadConfig();

        new Harmony(PluginInfo.GUID).PatchAll(GetType().Assembly);
    }

    /// <summary> Loads the configurator. </summary>
    public void LoadConfig()
    {
        config = PluginConfigurator.Create(PluginInfo.Name, PluginInfo.GUID);
        config.SetIconWithURL(Path.Combine(Path.GetDirectoryName(Info.Location), "icon.png"));
        config.presetButtonHidden = true; // fuck u

        ConfigDivision flightDivision = new(config.rootPanel, "Flight");
        flightSpeed = new(flightDivision, "Regular Flight speed", "meowmeowmeow mrrrp miaaow rawwrr :3", 1f);
        flightWeeeeSpeed = new(flightDivision, "Boost Flight speed", "meow - tequilla", 2.5f);

        ConfigDivision noclipDivision = new(config.rootPanel, "Noclip");
        noclipSpeed = new(noclipDivision, "Regular Noclip speed", "imhonestlytoolazytoputanythingfunnyhere", 1f);
        noclipWeeeeSpeed = new(noclipDivision, "Boost Noclip speed", "rawr2.2"/*geometry dash*/, 2.5f);
    }
}