using spaar.ModLoader;
using spaar.ModLoader.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

namespace Exploding_CannonBall_Mod
{
    public class Exploding_CannonBall_Mod : Mod
    {
        public override string Author
        {get{return "TesseractCat - Maintained By Wang_W571 and MaxTCC";}}

        public override string BesiegeVersion
        {            get            {                return "v0.3";            }        }

        public override bool CanBeUnloaded
        {
            get
            {
                return true;
            }
        }

        public override string DisplayName
        {get{return "Exploding Cannonballs Mod";}}

        public override string Name{get{return "BesiegeExplodingCannonballs";}}
        public override Version Version
        {get{return new Version("0.3.3");}}
        public Exploding_CannonBall_Mod()
        {
        }

        public override void OnLoad()
        {
            GameObject.DontDestroyOnLoad(ExplodingCannonballScript.Instance);

            ExplodingCannonballScript.Instance.LoadConfiguration();
            InitSliders();

            Game.OnBlockPlaced += AddSliders;
            Game.OnKeymapperOpen += () =>
            {
                if (!HasSliders(BlockMapper.CurrentInstance.Block))
                    AddSliders(BlockMapper.CurrentInstance.Block);
                AddAllSliders();
            };
        }

        public override void OnUnload()
        {
            ExplodingCannonballScript.Instance.SaveConfiguration();
            GameObject.Destroy(ExplodingCannonballScript.Instance);
        }

        #region sliders

        // Static references to sliders;
        // All blocks share the same slider instance
        internal static MMenu explosionTypeToggle;
        internal static MSlider impactDetectionSlider;
        internal static MSlider explosionDelaySlider;
        internal static MSlider explosionPowerSlider;
        internal static MSlider explosionRangeSlider;

        /// <summary>
        /// Initializes slider instances.
        /// Must be called after configuration load and before any slider is initialized.
        /// </summary>
        private static void InitSliders()
        {
            explosionTypeToggle = new MMenu("explosiontype", ExplodingCannonballScript.Instance.TypeOfExplosion,
                    new List<string>()
                    {
                        "No explosion",
                        "Bomb explosion",
                        "Grenade",
                        "Rocket"
                    }, "Explosion type");
            explosionTypeToggle.ValueChanged += (int value) => { ExplodingCannonballScript.Instance.TypeOfExplosion = value; };

            impactDetectionSlider = new MSlider("Impact detection", "impactdetection", ExplodingCannonballScript.Instance.ImpactDetector, 0, 5);
            impactDetectionSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.ImpactDetector = value; };

            explosionDelaySlider = new MSlider("Explosion delay", "explosiondelay", ExplodingCannonballScript.Instance.ExplosionDelay, 0, 5);
            explosionDelaySlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.ExplosionDelay = value; };

            explosionPowerSlider = new MSlider("Explosion power", "explosionpower", ExplodingCannonballScript.Instance.PowerMultiplierOfExplosion, 0, 5);
            explosionPowerSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.PowerMultiplierOfExplosion = value; };

            explosionRangeSlider = new MSlider("Explosion range", "explosionrange", ExplodingCannonballScript.Instance.RangeMultiplierOfExplosion, 0, 5);
            explosionRangeSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.RangeMultiplierOfExplosion = value; };
        }

        /// <summary>
        /// BlockBehaviour private readonly mapperTypes field.
        /// Of type List<MapperType>.
        /// </summary>
        private static FieldInfo mapperTypesField = typeof(BlockBehaviour).GetField("mapperTypes", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Returns true if block already has added sliders.
        /// Returns true on other blocks than the Cannon.
        /// </summary>
        /// <param name="block">BlockBehaviour of the block.</param>
        public static bool HasSliders(BlockBehaviour block)
        {
            return !(block.GetBlockID() == (int)BlockType.Cannon) || block.MapperTypes.Exists(match => match.Key == "explosiontype");
        }

        /// <summary>
        /// Adds sliders to all blocks that don't yet have them.
        /// </summary>
        public static void AddAllSliders()
        {
            foreach (var block in Machine.Active().BuildingBlocks.FindAll(block => !HasSliders(block)))
                AddSliders(block);
        }

        /// <summary>
        /// Wrapper for AddSliders(BlocKBehaviour) with a check and component retrieval.
        /// </summary>
        /// <param name="block">block Transform</param>
        public static void AddSliders(Transform block)
        {
            var blockbehaviour = block.GetComponent<BlockBehaviour>();
            if (!HasSliders(blockbehaviour))
                AddSliders(blockbehaviour);
        }

        /// <summary>
        /// Adds sliders to the block.
        /// </summary>
        /// <param name="block">BlockBehaviour script</param>
        private static void AddSliders(BlockBehaviour block)
        {
            if (block.GetBlockID() == (int)BlockType.Cannon)
            {
                var currentMapperTypes = block.MapperTypes;

                currentMapperTypes.Add(explosionTypeToggle);
                currentMapperTypes.Add(impactDetectionSlider);
                currentMapperTypes.Add(explosionDelaySlider);
                currentMapperTypes.Add(explosionPowerSlider);
                currentMapperTypes.Add(explosionRangeSlider);

                mapperTypesField.SetValue(block, currentMapperTypes);
            }
        }

        #endregion
    }

    public class ExplodingCannonballScript : SingleInstance<ExplodingCannonballScript>
    {
        public int TypeOfExplosion = 1;
        public float ExplosionDelay = 0;
        public float PowerMultiplierOfExplosion = 1;
        public float RangeMultiplierOfExplosion = 1;
        public float ImpactDetector = 0;

        public override string Name { get; } = "Exploding Cannonball Mod";

        void Start()
        {
            
            Commands.RegisterCommand("ChangeExplosionType", (args, notUses) =>
            {
                try
                {
                    TypeOfExplosion = int.Parse(args[0]);
                    TypeOfExplosion = Mathf.Clamp(TypeOfExplosion, 0, 3);
                    Exploding_CannonBall_Mod.explosionTypeToggle.Value = TypeOfExplosion;
                }
                catch { return "Wrong Option! There are four options: \n 0-No Explosion \n 1-Bomb Explosion \n 2-Grenade \n 3-Rocket \n Example: ChangeExplosionType 2"; }

                return "Complete!";
                 
            }, "Change the explosion type of cannonballs");
            Commands.RegisterCommand("ChangeImpactDetection", (args, notUses) =>
            {
                try
                {
                    ImpactDetector = float.Parse(args[0]);
                    Exploding_CannonBall_Mod.impactDetectionSlider.Value = ImpactDetector;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change detection of impact for cannonballs");
            Commands.RegisterCommand("ChangeExplosionDelay", (args, notUses) =>
            {
                try
                {
                    ExplosionDelay = float.Parse(args[0]);
                    ExplosionDelay = Mathf.Clamp(ExplosionDelay, 0, Mathf.Infinity);
                    Exploding_CannonBall_Mod.explosionDelaySlider.Value = ExplosionDelay;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion delay after cannonballs collide");
            Commands.RegisterCommand("ChangeExplosionPowerX", (args, notUses) =>
            {
                try
                {
                    PowerMultiplierOfExplosion = float.Parse(args[0]);
                    PowerMultiplierOfExplosion = Mathf.Clamp(PowerMultiplierOfExplosion, 0, Mathf.Infinity);
                    Exploding_CannonBall_Mod.explosionPowerSlider.Value = PowerMultiplierOfExplosion;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion power.");
            Commands.RegisterCommand("ChangeExplosionRangeX", (args, notUses) =>
            {
                try
                {
                    RangeMultiplierOfExplosion = float.Parse(args[0]);
                    RangeMultiplierOfExplosion = Mathf.Clamp(RangeMultiplierOfExplosion, 0, Mathf.Infinity);
                    Exploding_CannonBall_Mod.explosionRangeSlider.Value = RangeMultiplierOfExplosion;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion range.");
        }


        private void Update()
        {
            if (TypeOfExplosion != 0)
            {
                if (GameObject.Find("CanonBallHeavy(Clone)"))
                {
                    GameObject.Find("CanonBallHeavy(Clone)").AddComponent<ExplosionForCannonballs>();
                    GameObject.Find("CanonBallHeavy(Clone)").name = "CannonBomb(Clone)";
                }
            }
        }
        private void FixedUpdate()
        {
            if (TypeOfExplosion != 0)
            {
                if (GameObject.Find("CanonBallHeavy(Clone)"))
                {
                    GameObject.Find("CanonBallHeavy(Clone)").AddComponent<ExplosionForCannonballs>();
                    GameObject.Find("CanonBallHeavy(Clone)").name = "CannonBomb(Clone)";
                }
            }
        }

        internal void LoadConfiguration()
        {
            TypeOfExplosion = Mathf.Clamp(Configuration.GetInt("Explosion Type", TypeOfExplosion), 0, 3);
            ExplosionDelay = Configuration.GetFloat("Explosion Delay", ExplosionDelay);
            PowerMultiplierOfExplosion = Configuration.GetFloat("Explosion Power", PowerMultiplierOfExplosion);
            RangeMultiplierOfExplosion = Configuration.GetFloat("Explosion Range", RangeMultiplierOfExplosion);
            ImpactDetector = Configuration.GetFloat("Impact Detection", ImpactDetector);
        }

        internal void SaveConfiguration()
        {
            Configuration.SetInt("Explosion Type", TypeOfExplosion);
            Configuration.SetFloat("Explosion Delay", ExplosionDelay);
            Configuration.SetFloat("Explosion Power", PowerMultiplierOfExplosion);
            Configuration.SetFloat("Explosion Range", RangeMultiplierOfExplosion);
            Configuration.SetFloat("Impact Detection", ImpactDetector);
            Configuration.Save();
        }
    }

    public class ExplosionForCannonballs:MonoBehaviour
    {
        public ExplodingCannonballScript ECS;
        private int CountDownExplode;
        private bool Exploding = false;
        private int FrameCount = 0;
        IEnumerator Explode()
        {
            if (Exploding)
            {
                while (CountDownExplode >= 0)
                {
                    yield return new WaitForFixedUpdate();
                    --CountDownExplode;
                    StartCoroutine(Explode());
                    yield break;
                }
                ECS = GameObject.Find("Exploding Cannonball Mod").GetComponent<ExplodingCannonballScript>();
                if (ECS.TypeOfExplosion == 1)
                {
                    GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[23].gameObject, this.transform.position, this.transform.rotation);
                    explo.transform.localScale = Vector3.one * 0.01f;
                    ExplodeOnCollideBlock ac = explo.GetComponent<ExplodeOnCollideBlock>();
                    ac.radius = 7 * ECS.RangeMultiplierOfExplosion;
                    ac.power = 2100f * ECS.PowerMultiplierOfExplosion;
                    ac.torquePower = 100000 * ECS.PowerMultiplierOfExplosion;
                    ac.upPower = 0;
                    ac.Explodey();
                    explo.AddComponent<TimedSelfDestruct>();
                    Destroy(this.gameObject);
                }
                else if (ECS.TypeOfExplosion == 2)
                {
                    GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[54].gameObject, this.transform.position, this.transform.rotation);
                    explo.transform.localScale = Vector3.one * 0.01f;
                    ControllableBomb ac = explo.GetComponent<ControllableBomb>();
                    ac.radius = 3 * ECS.RangeMultiplierOfExplosion;
                    ac.power = 1500 * ECS.PowerMultiplierOfExplosion;
                    ac.randomDelay = 0.00001f;
                    ac.upPower = 0f;
                    ac.StartCoroutine_Auto(ac.Explode());
                    explo.AddComponent<TimedSelfDestruct>();
                    Destroy(this.gameObject);
                }
                else if (ECS.TypeOfExplosion == 3)
                {
                    GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[59].gameObject, this.transform.position, this.transform.rotation);
                    explo.transform.localScale = Vector3.one * 0.01f;
                    TimedRocket ac = explo.GetComponent<TimedRocket>();
                    ac.SetSlip(Color.white);
                    ac.radius = 3 * ECS.RangeMultiplierOfExplosion;
                    ac.power = 1500 * ECS.PowerMultiplierOfExplosion;
                    ac.randomDelay = 0.000001f;
                    ac.upPower = 0;
                    ac.StartCoroutine(ac.Explode(0.01f));
                    explo.AddComponent<TimedSelfDestruct>();
                    Destroy(this.gameObject);
                }
            }
        }
        void Start()
        {
            ECS = ExplodingCannonballScript.Instance;

        }
        void Update()
        {
            if (!Exploding)
            CountDownExplode = (int)  (ECS.ExplosionDelay * 100);
        }
        void FixedUpdate()
        {
            ++FrameCount;
        }
        void OnCollisionEnter(Collision coll)
        {
            if ((!Exploding || coll.impactForceSum.magnitude > ECS.ImpactDetector) && FrameCount > 2)
            {
                Exploding = true;
                StartCoroutine(Explode());
            }
        }
        void OnCollisionStay(Collision coll)
        {
            if ((!Exploding || coll.relativeVelocity.sqrMagnitude > ECS.ImpactDetector * ECS.ImpactDetector) && FrameCount > 2)
            {
                Exploding = true;
                StartCoroutine(Explode());
            }
        }
    }
    public class TimedSelfDestruct:MonoBehaviour
    {
        float timer = 0;
        void FixedUpdate()
        {
            ++timer;
            if (timer > 260)
            {
                Destroy(this.gameObject);
            }
            if(this.GetComponent<TimedRocket>())
            {
                Destroy(this.GetComponent<TimedRocket>());
            }
        }
    }
}
