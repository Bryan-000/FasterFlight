namespace FasterFlight;

using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ULTRAKILL.Cheats;

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
    public FloatField flightSpeed, flightWeeeeSpeed;

    /// <summary> Load the mod. </summary>
    public void Awake()
    {
        config = PluginConfigurator.Create(PluginInfo.Name, PluginInfo.GUID);

        flightSpeed = new(config.rootPanel, "Regular Noclip speed", "meowmeowmeow mrrrp miaaow rawwrr :3", 1f);
        flightWeeeeSpeed = new(config.rootPanel, "Boost Noclip speed", "meow - tequilla", 2.5f);
        
        new Harmony(PluginInfo.GUID).PatchAll(GetType());
    }

    /// <summary> Patch Noclip.UpdateTick/Flight.Update so it uses the mods flight speed and boost speed instead. </summary>
    [HarmonyTranspiler] [HarmonyPatch(typeof(Noclip), "UpdateTick")] [HarmonyPatch(typeof(Flight), "Update")]
    public static IEnumerable<CodeInstruction> SpeedUpNoclip(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo get_instance = AccessTools.PropertyGetter(typeof(Plugin), "Instance");
        MethodInfo get_value = AccessTools.PropertyGetter(typeof(FloatField), "value");

        int floatCount = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4)
            {
                floatCount++;

                if (floatCount > 2)
                    goto yield_meow;

                yield return new(OpCodes.Callvirt, get_instance); // put the plugin instance on the evaluation stack aka Plugin.Instance
                yield return new(OpCodes.Ldfld, AccessTools.Field(typeof(Plugin), floatCount == 1 ? "flightSpeed" : "flightWeeeeSpeed")); // load instance field aka Plugin.Instance.flightSpeed/flightWeeeeSpeed
                yield return new(OpCodes.Callvirt, get_value); // call property getter for Plugin.Instance.flightSpeed/flightWeeeeSpeed to put its float value onto the evaluation stack to be consumed by `float speedMulti = ;`

                continue;
            }

        yield_meow:
            yield return instruction;
        }
    }
}