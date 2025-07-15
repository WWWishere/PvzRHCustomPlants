using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace SunSniperPea
{
    [BepInPlugin("evenmoresnip.sunsnip", "SunSniperPea", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SunSniperPea>();
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "tempsniperpea");
            CustomCore.RegisterCustomPlant<SniperPea, SunSniperPea>(1807, assetBundle.GetAsset<GameObject>("TempSniperPeaPrefab"), assetBundle.GetAsset<GameObject>("TempSniperPeaPreview"), new List<ValueTuple<int, int>>
			{
				new ValueTuple<int, int>(1805, 0),
				new ValueTuple<int, int>(0, 1805),
				new ValueTuple<int, int>(1109, 1000),
                new ValueTuple<int, int>(1000, 1109)
            }, 3f, 0f, 600, 300, 0f, 750);
            CustomCore.AddPlantAlmanacStrings(1807, "阳光狙击豌豆(1807)", "定期狙击一只僵尸，造成高伤害和阳光。\n<color=#3D1400>伤害：</color><color=red>600/3秒</color>\n<color=#3D1400>融合配方：</color><color=red>狙击豌豆+豌豆向日葵</color>\n<color=#3D1400>特点：</color><color=red>攻击全场索敌可对空，每次攻击时会产生10阳光，每第6次攻击爆头，造成100万伤害和50阳光，可以与超级阳光机枪射手相转化</color>\n<color=#3D1400>转换配方：</color><color=red>豌豆射手←→豌豆射手</color>\n<color=#3D1400>阳光狙击豌豆不用带太阳镜，因为太阳已经弄到头上了。在它的眼里，僵尸出现会发光</color>");
            // ElectricPea.Buff = CustomCore.RegisterCustomBuff("电涌穿透：电能豌豆的子弹每次攻击对本体额外造成总血量5%的伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<ElectricPea>(), 36100, "red", (PlantType)960);
            CustomCore.AddFusion(1805, 1807, 0);
            CustomCore.AddFusion(1805, 0, 1807);
        }
    }
    public class SunSniperPea : MonoBehaviour
    {
        public SniperPea plant => gameObject.GetComponent<SniperPea>();
        public SunSniperPea() : base(ClassInjector.DerivedConstructorPointer<SunSniperPea>()) => ClassInjector.DerivedConstructorBody(this);

        public SunSniperPea(IntPtr i) : base(i)
        {

        }
		public void FixedUpdate()
        {
			bool flag = plant.targetZombie != null;
			if (flag)
			{
				bool isMindControlled = plant.targetZombie.isMindControlled;
				if (isMindControlled)
				{
					SearchZombie();
				}
			}
        }
        public void AnimShoot_SunSniperPea()
		{
			GameAPP.PlaySound(40, 0.2f, 1f);
			Zombie targetZombie = plant.targetZombie;
			bool flag = targetZombie == null || !this.SearchUniqueZombie(targetZombie);
			if (!flag)
			{
				int attackCount = plant.attackCount;
				plant.attackCount = attackCount + 1;
				int damage = plant.attackDamage;
				bool flag2 = plant.attackCount % 6 == 0;
				if (flag2)
				{
					damage = 1000000;
				}
				this.AttackZombie(targetZombie, damage);
				bool flag3 = targetZombie.theStatus != ZombieStatus.Dying && !targetZombie.beforeDying;
				if (!flag3)
				{
					plant.targetZombie = null;
				}
			}
		}
        public void AttackZombie(Zombie zombie, int damage)
        {
            bool flag = zombie == null;
			if (!flag)
			{
				int sunSize = 13;
                Vector3 zPosition = plant.targetZombie.axis.transform.position;
				if (plant.attackCount % 6 == 0)
				{
					sunSize = 0;
				}
                CreateItem.Instance.SetCoin(0, 0, sunSize, 0, zPosition, false);
                CreateItem.Instance.SetCoin(0, 0, sunSize, 0, zPosition, false);
				zombie.TakeDamage(DmgType.Normal, damage, false);
				Vector3 position = plant.ac.transform.position;
				CreateParticle.SetParticle(84, position, plant.targetZombie.theZombieRow, true);
            }
        }
        public bool SearchUniqueZombie(Zombie zombie)
		{
			bool flag = zombie == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = zombie.isMindControlled || zombie.beforeDying;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int theStatus = (int)zombie.theStatus;
					bool flag3 = theStatus <= 7;
					if (flag3)
					{
						bool flag4 = theStatus == 1 || theStatus == 7;
						if (flag4)
						{
							return false;
						}
					}
					else
					{
						bool flag5 = theStatus == 12 || (theStatus >= 20 && theStatus <= 24);
						if (flag5)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public GameObject SearchZombie()
		{
			plant.zombieList.Clear();
			float num = float.MaxValue;
			GameObject gameObject = null;
			bool flag = plant.board != null;
			if (flag)
			{
				foreach (Zombie zombie in plant.board.zombieArray)
				{
					bool flag2 = zombie == null;
					if (!flag2)
					{
						Transform transform = zombie.transform;
						bool flag3 = transform == null;
						if (!flag3)
						{
							bool flag4 = plant.vision < transform.position.x;
							if (!flag4)
							{
								Transform axis = plant.axis;
								bool flag5 = axis == null;
								if (!flag5)
								{
									bool flag6 = transform.position.x > axis.position.x;
									if (flag6)
									{
										bool flag7 = SearchUniqueZombie(zombie);
										if (flag7)
										{
											float num2 = Vector3.Distance(transform.position, axis.position);
											bool flag8 = num2 < num;
											if (flag8)
											{
												num = num2;
												gameObject = zombie.gameObject;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag9 = gameObject != null;
			GameObject result;
			if (flag9)
			{
				plant.targetZombie = gameObject.GetComponent<Zombie>();
				result = gameObject;
			}
			else
			{
				result = null;
			}
			return result;
		}
    }
}
