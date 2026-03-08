namespace FasterFlight;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using ULTRAKILL.Cheats;
using PluginConfig.API.Fields;
using System.Reflection.Emit;

/// <summary> class with all the patches :33333 </summary>
[HarmonyPatch]
public static class Patches
{
    /// <summary> Transpile <see cref="Flight.Update"/> so it uses <seealso cref="noclipSpeed"/> and <seealso cref="noclipWeeeeSpeed"/> instead of fixed values of 1f and 2.5f. </summary>
    [HarmonyTranspiler] [HarmonyPatch(typeof(Flight), "Update")]
    public static IEnumerable<CodeInstruction> FlightTranspiler(IEnumerable<CodeInstruction> instructions) =>
        FlightTranspiler(instructions, "flightSpeed", "flightWeeeeSpeed");

    /// <summary> Transpile <see cref="Noclip.UpdateTick"/> so it uses <seealso cref="noclipSpeed"/> and <seealso cref="noclipWeeeeSpeed"/> instead of fixed values of 1f and 2.5f. </summary>
    [HarmonyTranspiler] [HarmonyPatch(typeof(Noclip), "UpdateTick")]
    public static IEnumerable<CodeInstruction> NoclipTranspiler(IEnumerable<CodeInstruction> instructions) =>
        FlightTranspiler(instructions, "noclipSpeed", "noclipWeeeeSpeed");

    /// <summary> Transpiler to be used on <see cref="Flight.Update"/> and <seealso cref="Noclip.UpdateTick"/> </summary>
    public static IEnumerable<CodeInstruction> FlightTranspiler(IEnumerable<CodeInstruction> instructions, string regularSpeedFieldName, string boostSpeedFieldName)
    {
        MethodInfo get_instance = AccessTools.PropertyGetter(typeof(Plugin), "Instance");
        MethodInfo get_value = AccessTools.PropertyGetter(typeof(FloatField), "value");

        int floatCount = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4)
            {
                floatCount++;

                // if this is past the second ldc.r4 then just skip everything
                if (floatCount > 2)
                    goto yield_meow;

                // put the plugin instance on the evaluation stack aka Plugin.Instance
                yield return new(OpCodes.Callvirt, get_instance);

                // load instance field from plugin class with the provided names based on if this is the first or second ldc.r4
                yield return new(OpCodes.Ldfld, AccessTools.Field(typeof(Plugin), floatCount == 1 ? regularSpeedFieldName : boostSpeedFieldName));

                // call property getter for the field to put its (should be) float value onto the evaluation stack to be consumed by whatever was doing ldc.r4
                // (should be float multiSpeed setter)
                yield return new(OpCodes.Callvirt, get_value);

                // skip so we dont return the original ldc.r4
                continue;
            }

        yield_meow:
            yield return instruction;
        }
    }
}
