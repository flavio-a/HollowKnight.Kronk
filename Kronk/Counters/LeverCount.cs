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

        private static void CountBridgeLevers(UnityEngine.Collider2D otherCollider, UnityEngine.GameObject _slash)
        {
            string otherName = otherCollider.name;
            if (otherCollider.gameObject.scene.name != "Fungus2_21" || !otherName.StartsWith("Bridge Lever "))
            {
                return;
            }
            //Kronk.instance.LogDebug("Slash hit! " + otherCollider.name + "(" + otherCollider.gameObject.scene.name + ")");
            if (otherName == "Bridge Lever 1" && !Kronk.localSettings.BridgeLever1)
            {
                Kronk.localSettings.BridgeLever1 = true;
                IncrementLeverCount();
            }
            if (otherName == "Bridge Lever 2" && !Kronk.localSettings.BridgeLever2)
            {
                Kronk.localSettings.BridgeLever2 = true;
                IncrementLeverCount();
            }
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
                    IncrementLeverCount();
                }));
            }
        }

        private static void CountMantisLever(Scene arg0, Scene arg1)
        {
            if (!string.IsNullOrEmpty(arg0.name)
                && GameManager.GetBaseSceneName(arg0.name) == "Fungus2_15"
                && arg1.name == "Fungus2_31"
                && !Kronk.localSettings.MantisRewardsLever
                && PlayerData.instance.defeatedMantisLords)
            {
                Kronk.localSettings.MantisRewardsLever = true;
                IncrementLeverCount();
            }
        }

        private static void IncrementLeverCount()
        {
            Kronk.localSettings.LeversHit += 1;

            if (IsActive)
            {
                Display.UpdateText();
                if (Kronk.localSettings.LeversHit == NUMOBJECTS)
                {
                    Kronk.SendMessageToLivesplit();
                }
            }
        }
    }
}
