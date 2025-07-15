using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace SunSuperGatling
{
    [BepInPlugin("evenmoresnip.sunsg", "SunSuperGatling", "1.0")]
    class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SunSuperGatling>();
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "sunsupergatling");
            CustomCore.RegisterCustomPlant<SuperGatling, SunSuperGatling>(1805, assetBundle.GetAsset<GameObject>("SunSuperGatlingPrefab"), assetBundle.GetAsset<GameObject>("SunSuperGatlingPreview"), new List<ValueTuple<int, int>>
            {
                new ValueTuple<int, int>(1168, 1000),
                new ValueTuple<int, int>(1000, 1168)
            }, 1.5f, 0f, 30, 300, 0f, 750);
            CustomCore.AddPlantAlmanacStrings(1805, "超级阳光机枪射手(1805)", "普通的超级机枪射手，就是弄黄了一点。\n\n<color=#3D1400>伤害：</color><color=red>30×6</color>\n<color=#3D1400>融合配方：</color>\n<color=#3D1400>特点：</color><color=red>子弹命中时有1/3概率获得5阳光</color><color=red>超级机枪射手+豌豆向日葵</color>\n<color=#3D1400>请在这里放一个小的笑话</color>");
        }
    }
    public class SunSuperGatling : MonoBehaviour
    {
        public SuperGatling plant => gameObject.GetComponent<SuperGatling>();
        public SunSuperGatling() : base(ClassInjector.DerivedConstructorPointer<SunSuperGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public SunSuperGatling(IntPtr i) : base(i)
        {

        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).FindChild("Shoot");
        }

    }

    [HarmonyPatch(typeof(SuperGatling), "GetBulletType")]
    public class SuperGatling_GetBulletType
    {
        public static void Postfix(SuperGatling __instance, ref BulletType __result)
        {
            bool flag = __instance.thePlantType == (PlantType)1805;
            if (flag)
            {
                __result = (BulletType)8;
            }
        }
    }
}
