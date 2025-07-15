using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using JetBrains.Annotations;
using System.Reflection;
using UnityEngine;

namespace CalamitySniperPea
{
    [BepInPlugin("evenmoresnip.calasnip", "CalamitySniperPea", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<CalamitySniperPea>();
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "tempfiresniper");
            CustomCore.RegisterCustomPlant<SniperPea, CalamitySniperPea>(1808, assetBundle.GetAsset<GameObject>("TempFireSniperPrefab"), assetBundle.GetAsset<GameObject>("TempFireSniperPreview"), new List<ValueTuple<int, int>>
            {
                new ValueTuple<int, int>(1109, 928),
                new ValueTuple<int, int>(928, 1109)
            }, 3f, 0f, 900, 300, 0f, 925);
            CustomCore.AddPlantAlmanacStrings(1808, "幻灭狙击豌豆(1808)", "比较强的狙击豌豆。\n<color=#3D1400>伤害：</color><color=red>900/3秒</color>\n<color=#3D1400>融合配方：</color><color=red>狙击豌豆+幻灭菇</color>\n<color=#3D1400>特点：</color><color=red>攻击全场索敌可对空，每第4次攻击爆头，造成幻灭菇的结果</color>\n<color=#3D1400>“不管危险多大，我都会做！”</color>");
            // ElectricPea.Buff = CustomCore.RegisterCustomBuff("电涌穿透：电能豌豆的子弹每次攻击对本体额外造成总血量5%的伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<ElectricPea>(), 36100, "red", (PlantType)960);
        }
    }
    public class CalamitySniperPea : MonoBehaviour
    {
        public SniperPea plant => gameObject.GetComponent<SniperPea>();
        public CalamitySniperPea() : base(ClassInjector.DerivedConstructorPointer<CalamitySniperPea>()) => ClassInjector.DerivedConstructorBody(this);

        public CalamitySniperPea(IntPtr i) : base(i)
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
        public void SetHypno(Zombie zombie)
        {
			zombie.SetColor(Zombie.ZombieColor.Doom);
            zombie.isDoom = true;
			zombie.doomWithPit = false;
            zombie.SetMindControl(1);
        }
        public void AnimShoot_CalamitySniperPea()
        {
            GameAPP.PlaySound(40, 0.2f, 1f);
            Zombie targetZombie = plant.targetZombie;
            bool flag = targetZombie == null || !this.SearchUniqueZombie(targetZombie);
            if (!flag)
            {
                int attackCount = plant.attackCount;
                plant.attackCount = attackCount + 1;
                int damage = plant.attackDamage;
                Vector3 zPosition = targetZombie.axis.transform.position;
                bool flag2 = plant.attackCount % 4 == 0;
                if (flag2)
                {
                    damage = 0;
                    GameAPP.board.GetComponent<Board>().SetDoom(Mouse.Instance.GetColumnFromX(zPosition.x), targetZombie.theZombieRow, false, false, zPosition, 0, 0, null);
                    foreach (Zombie zombie in plant.board.zombieArray)
                    {
                        bool zflag = zombie == null;
                        if (!zflag)
                        {
                            var zPos2 = zombie.axis.transform.position;
                            int diffx = Math.Abs(Mouse.Instance.GetColumnFromX(zPosition.x) - Mouse.Instance.GetColumnFromX(zPos2.x));
                            int diffy = Math.Abs(targetZombie.theZombieRow - zombie.theZombieRow);
                            bool zflag1 = Math.Pow(diffx,2) + Math.Pow(diffy,2) < 15;
                            if (zflag1)
                            {
                                SetHypno(zombie);
                            }
                        }
                    }
                }
                this.AttackZombie(targetZombie, damage);
                bool flag3 = targetZombie.theStatus != ZombieStatus.Dying && !targetZombie.beforeDying;
                if (targetZombie.isMindControlled || !flag3)
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
                zombie.TakeDamage(DmgType.Normal, damage, false);
                Vector3 position = plant.ac.transform.position;
                CreateParticle.SetParticle(27, position, plant.targetZombie.theZombieRow, true);
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
