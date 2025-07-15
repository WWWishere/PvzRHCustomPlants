using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace AltIceDoomSuperGatling // Copied from infinite75's Electric Pea Code, do not expect perfectly optimized code
{
    [BepInPlugin("evenmoresnip.icedoomsg", "AltIceDoomSuperGatling", "1.0")]
    class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<AltIceDoomSuperGatling>();
            ClassInjector.RegisterTypeInIl2Cpp<Bullet_altIce_doom>();
            ClassInjector.RegisterTypeInIl2Cpp<Bullet_altIce_doomBig>();
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)1801);
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "alticedoomsupergatling");
            CustomCore.RegisterCustomBullet<Bullet_doom, Bullet_altIce_doom>((BulletType)1802, assetBundle.GetAsset<GameObject>("Bullet_altIceDoom_doom"));
            CustomCore.RegisterCustomBullet<Bullet_doom, Bullet_altIce_doomBig>((BulletType)1803, assetBundle.GetAsset<GameObject>("Bullet_altIceDoom_doomBig"));
            CustomCore.RegisterCustomPlant<SuperGatling, AltIceDoomSuperGatling>(1801, assetBundle.GetAsset<GameObject>("AltIceDoomSuperGatlingPrefab"), assetBundle.GetAsset<GameObject>("AltIceDoomSuperGatlingPreview"), new List<ValueTuple<int, int>>
            {
                new ValueTuple<int, int>(1168, 1040),
                new ValueTuple<int, int>(1040, 1168),
                new ValueTuple<int, int>(1165, 11),
                new ValueTuple<int, int>(11, 1165),
                new ValueTuple<int, int>(1900, 0),
                new ValueTuple<int, int>(0, 1900)
            }, 1.5f, 0f, 300, 300, 0f, 800);
            CustomCore.AddPlantAlmanacStrings(1801, "超级冰毁机枪射手(1801)", "我已经超级累了。\n<color=#3D1400>伤害：</color><color=red>300×6，1800</color>\n<color=#3D1400>融合配方：</color><color=red>超级机枪射手+寒冰毁灭菇</color>\n<color=#3D1400>特点：</color><color=red>发射小寒冰毁灭菇子弹造成10点冻结值和10秒减速。发子弹时有1/8概率改为大寒冰毁灭菇子弹，伤害1800并造成100点冻结值，半径3格无衰减溅射，对直击僵尸造成10秒减速，并在直击僵尸处生成伤害为1800的不冻结关卡的寒冰毁灭菇效果，然后休息3秒</color>\n<color=#3D1400>转换配方：</color><color=red>豌豆射手←→豌豆射手</color>\n<color=#3D1400>啊！这个我认识，以前看到过\n怎么能看到过？是干做出来的</color>\n\n<color=#3D1400>原作:寒冰毁灭菇机枪射手</color>\n<color=#3D1400>作者:@MC屑鱼，@林秋-AutumnLin</color>");
            // ElectricPea.Buff = CustomCore.RegisterCustomBuff("电涌穿透：电能豌豆的子弹每次攻击对本体额外造成总血量5%的伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<ElectricPea>(), 36100, "red", (PlantType)960);
            CustomCore.AddFusion(1900, 1801, 0);
            CustomCore.AddFusion(1900, 0, 1801);
        }
    }
    public class AltIceDoomSuperGatling : MonoBehaviour
    {
        public SuperGatling plant => gameObject.GetComponent<SuperGatling>();
        public AltIceDoomSuperGatling() : base(ClassInjector.DerivedConstructorPointer<AltIceDoomSuperGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public AltIceDoomSuperGatling(IntPtr i) : base(i)
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
            bool flag = __instance.thePlantType == (PlantType)1801;
            if (flag)
            {
                int doomCount = Lawnf.TravelAdvanced(3) ? 8 : 16;
                if (UnityEngine.Random.RandomRangeInt(1, doomCount) == 1)
                {
                    __result = (BulletType)1803;
                }
                else
                {
                    __result = (BulletType)1802;
                }
            }
        }
    }

    public class Bullet_altIce_doom : MonoBehaviour
    {
        public Bullet_doom bullet => gameObject.GetComponent<Bullet_doom>();
        public void Update()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame)
            {
                if (bullet.normalSpeed <= 2)
                {
                    bullet.normalSpeed -= 1;
                }
            }
        }
    }
    public class Bullet_altIce_doomBig : MonoBehaviour
    {
        public Bullet_doom bullet => gameObject.GetComponent<Bullet_doom>();
    }

    [HarmonyPatch(typeof(Bullet_doom), "HitZombie")]
    public class Bullet_doom_HitZombie
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_doom __instance, ref Zombie zombie)
        {
            bool flag = __instance.theBulletType == (BulletType)1802 || __instance.theBulletType == (BulletType)1803;
            bool result;
            if (flag)
            {
                Transform transform = __instance.transform;
                Vector3 position = transform.position;
                GameObject gameObject = CreateParticle.SetParticle(28, position, __instance.theBulletRow, true);
                bool flag2 = __instance.theBulletType == (BulletType)1803;
                if (flag2)
                {
                    bool flag3 = gameObject != null;
                    if (flag3)
                    {
                        Transform transform2 = gameObject.transform;
                        Vector3 localScale = transform2.localScale;
                        transform2.localScale = new Vector3(localScale.x * 2f, localScale.y * 2f, localScale.z * 2f);
                        AoeDamage.BigBomb(position, 3f, __instance.zombieLayer, __instance.theBulletRow, 1800);
                        GameAPP.board.GetComponent<Board>().SetDoom(Mouse.Instance.GetColumnFromX(zombie.axis.transform.position.x), zombie.theZombieRow, false, true, zombie.axis.position, 1800, 0, null);
                    }
                    zombie.AddfreezeLevel(100);
                    GameAPP.PlaySound(41, 0.5f, 1f);
                }
                else
                {
                    GameAPP.PlaySound(70, 0.5f, 1f);
                    zombie.AddfreezeLevel(10);
                }
                zombie.SetCold(10f, 0);
                bool flag4 = Lawnf.TravelAdvanced(37);
                if (flag4)
                {
                    bool flag5 = zombie != null && zombie.axis != null;
                    if (flag5)
                    {
                        Vector3 position2 = zombie.axis.position;
                        int columnFromX = Mouse.Instance.GetColumnFromX(position2.x);
                        int theZombieRow = zombie.theZombieRow;
                        Board.Instance.SetDoom(columnFromX, theZombieRow, false, true, position2, 3600, 0, null);
                    }
                }
                bool flag6 = zombie != null && zombie.freezeTimer > 0f;
                if (flag6)
                {
                    zombie.TakeDamage(DmgType.IceAll, __instance.Damage * 4, false);
                }
                else
                {
                    bool flag7 = zombie != null;
                    if (flag7)
                    {
                        zombie.TakeDamage(DmgType.IceAll, __instance.Damage, false);
                    }
                }
                __instance.Die();
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }

    [HarmonyPatch(typeof(SuperSnowGatling), "Shoot1")]
    public class SuperSnowGatling_Shoot1
    {
        [HarmonyPostfix]
        public static void Postfix(SuperSnowGatling __instance, ref Bullet __result)
        {
            if (__result is null)
            {
                return;
            }
            var bulletType = __result.theBulletType;
            bool flag = bulletType == (BulletType)1802;
            bool flag2 = bulletType == (BulletType)1803;
            if (flag || flag2)
            {
                var position = __instance.shoot.position;
                __result.transform.SetPositionAndRotation(new Vector3(position.x + 0.1f, position.y - 0.2f, position.z), __result.transform.rotation);
                __result.Damage = __instance.attackDamage;
                if (flag2)
                {
                    __result.Damage *= 6;
                }
            }
        }
    }

    [HarmonyPatch(typeof(SuperSnowGatling), "SuperShoot")]
    public class SuperSnowGatling_SuperShoot
    {
        [HarmonyPrefix]
        public static bool Prefix(SuperSnowGatling __instance, ref float angle, ref float speed, ref float x, ref float y)
        {
            bool flag = __instance.thePlantType == (PlantType)1801;
            if (flag)
            {
                x += 0.1f;
                y -= 0.2f;
            }
            return true;
        }
    }
}
