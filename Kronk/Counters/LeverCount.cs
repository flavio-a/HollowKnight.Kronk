using HutongGames.PlayMaker;
using UnityEngine.SceneManagement;
using Kronk.Util;
using Modding;

namespace Kronk.Counters
{
    public static class LeverCount
    {
        internal const int NUMOBJECTS = 63;
        public static void Hook()
        {
            Kronk.instance.Log("Hooking Lever Count...");
            Hooks.OnFsmEnable += CountLevers;
            ModHooks.SlashHitHook += CountBridgeLevers;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CountMantisLever;
        }

        public static void Unhook()
        {
            Kronk.instance.Log("Unhooking Lever Count...");
            Hooks.OnFsmEnable -= CountLevers;
            ModHooks.SlashHitHook -= CountBridgeLevers;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CountMantisLever;
        }

        private static void CountBridgeLevers(UnityEngine.Collider2D otherCollider, UnityEngine.GameObject _slash)
        {
            if (otherCollider.gameObject.scene.name != "Fungus2_21" || !otherCollider.name.StartsWith("Bridge Lever "))
            {
                return;
            }
            //Kronk.instance.LogDebug("Slash hit! " + otherCollider.name + "(" + otherCollider.gameObject.scene.name + ", " + otherCollider.enabled + ")");
            AddLeverHit(GetLeverId(otherCollider.gameObject));
        }

        private static bool IsActive => Kronk.globalSettings.countingMode == CountingMode.Levers;

        private static void CountLevers(PlayMakerFSM fsm)
        {
            if (!(fsm.FsmName == "Switch Control" || fsm.FsmName == "toll switch"))
            {
                return;
            }

            // Exclude Godhome orb from count
            if (fsm.gameObject.name == "gg_roof_lever") return;

            if (fsm.GetState("Hit") is FsmState hitState)
            {
                hitState.AddFirstAction(new ExecuteLambda(() =>
                {
                    AddLeverHit(GetLeverId(fsm.gameObject));
                }));
            }
        }

        private static void CountMantisLever(Scene _arg0, Scene _arg1)
        {
            // Check if the scene added at the bottom of the queue is Fungus2_31
            if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1).name == "Fungus2_31"
                && PlayerData.instance.defeatedMantisLords)
            {
                AddLeverHit("MantisLordsReward-Fungus2_31");
            }
        }

        private static void AddLeverHit(string leverId)
        {
            if (Kronk.localSettings.LeversHit.Contains(leverId))
            {
                //Kronk.instance.LogDebug($"Hit lever {leverId}, but not counted");
                return;
            }
            //Kronk.instance.LogDebug($"Hit lever {leverId}, adding");
            Kronk.localSettings.LeversHit.Add(leverId);

            if (IsActive)
            {
                Display.UpdateText();
                if (Kronk.localSettings.LeversCount == NUMOBJECTS)
                {
                    Kronk.SendMessageToLivesplit();
                }
            }
        }

        private static string GetLeverId(UnityEngine.GameObject lever)
        {
            return $"{lever.name}-{lever.scene.name}";
        }
    }
}
