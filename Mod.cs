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
        { get { return "TesseractCat - Maintained By Wang_W571 and MaxTCC Improvement by Lench"; } }

        public override string BesiegeVersion
        { get { return "v0.45a"; } }

        public override bool CanBeUnloaded
        {
            get
            {
                return true;
            }
        }

        public override string DisplayName
        { get { return "Exploding Cannonballs and Arrow Mod"; } }

        public override string Name { get { return "BesiegeExplodingCannonballs"; } }
        public override Version Version
        { get { return new Version("0.4.0"); } }
        public Exploding_CannonBall_Mod()
        {
        }

        public override void OnLoad()
        {
            GameObject.DontDestroyOnLoad(ExplodingCannonballScript.Instance);
            GameObject.DontDestroyOnLoad(ExplodingArrowsScript.Instance);

            ExplodingCannonballScript.Instance.LoadConfiguration();
            ExplodingArrowsScript.Instance.LoadConfiguration();

            InitCannonSliders();
            InitArrowSliders();


            Game.OnBlockPlaced += AddSliders;
            Game.OnKeymapperOpen += () =>
            {
                if (!HasCannonSliders(BlockMapper.CurrentInstance.Block) || !HasArrowSliders(BlockMapper.CurrentInstance.Block))
                    AddSliders(BlockMapper.CurrentInstance.Block);
                AddAllSliders();
            };
        }

        public override void OnUnload()
        {
            ExplodingCannonballScript.Instance.SaveConfiguration();
            ExplodingArrowsScript.Instance.SaveConfiguration();
            GameObject.Destroy(ExplodingCannonballScript.Instance);
            GameObject.Destroy(ExplodingArrowsScript.Instance);

        }

        #region sliders

        // Static references to sliders;
        // All blocks share the same slider instance
        internal static MMenu CannonballExplosionTypeToggle;
        internal static MSlider CannonballImpactDetectionSlider;
        internal static MSlider CannonballExplosionDelaySlider;
        internal static MSlider CannonballExplosionPowerSlider;
        internal static MSlider explosionRangeSlider;
        internal static MToggle cannonBallTrailEnabled;
        internal static MColourSlider cannonBallTrailColor;
        internal static MSlider cannonBallTrailLength;

        internal static MMenu ArrowAfterEffectTypeToggle;
        internal static MSlider ArrowAfterEffectImpactDetectionSlider;
        internal static MSlider ArrowAfterEffectDelaySlider;
        internal static MSlider ArrowAfterEffectPowerSlider;
        internal static MSlider ArrowAfterEffectRangeSlider;
        internal static MToggle ArrowTrailEnabled;
        internal static MColourSlider ArrowTrailColor;
        internal static MSlider ArrowTrailLength;

        /// <summary>
        /// Initializes slider instances.
        /// Must be called after configuration load and before any slider is initialized.
        /// </summary>
        private static void InitCannonSliders()
        {
            CannonballExplosionTypeToggle = new MMenu("explosiontype", ExplodingCannonballScript.Instance.TypeOfExplosion,
                    new List<string>()
                    {
                        "No explosion",
                        "Bomb", //Changed From Bomb Explosion for Chinese Translation
                        "Grenade",
                        "Rocket"
                    }, "Explosion type");
            CannonballExplosionTypeToggle.ValueChanged += (int value) =>
            {
                ExplodingCannonballScript.Instance.TypeOfExplosion = value;
                bool display = value != 0;
                CannonballImpactDetectionSlider.DisplayInMapper = display;
                CannonballExplosionDelaySlider.DisplayInMapper = display;
                CannonballExplosionPowerSlider.DisplayInMapper = display;
                explosionRangeSlider.DisplayInMapper = display;
            };

            CannonballImpactDetectionSlider = new MSlider("Impact detection", "impactdetection", ExplodingCannonballScript.Instance.ImpactDetector, 0, 5);
            CannonballImpactDetectionSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.ImpactDetector = value; };

            CannonballExplosionDelaySlider = new MSlider("Explosion delay", "explosiondelay", ExplodingCannonballScript.Instance.ExplosionDelay, 0, 10);
            CannonballExplosionDelaySlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.ExplosionDelay = value; };

            CannonballExplosionPowerSlider = new MSlider("Explosion power", "explosionpower", ExplodingCannonballScript.Instance.PowerMultiplierOfExplosion, 0, 20);
            CannonballExplosionPowerSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.PowerMultiplierOfExplosion = value; };

            explosionRangeSlider = new MSlider("Explosion range", "explosionrange", ExplodingCannonballScript.Instance.RangeMultiplierOfExplosion, 0, 20);
            explosionRangeSlider.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.RangeMultiplierOfExplosion = value; };

            cannonBallTrailEnabled = new MToggle("Enable Cannon Ball Trail", "TrailEnabled", ExplodingCannonballScript.Instance.IsTrailOn);
            cannonBallTrailEnabled.Toggled += (bool value) => { ExplodingCannonballScript.Instance.IsTrailOn = value; cannonBallTrailColor.DisplayInMapper = value; cannonBallTrailLength.DisplayInMapper = value; };

            cannonBallTrailColor = new MColourSlider("Trail Color", "TrailColor", ExplodingCannonballScript.Instance.TrailColor);
            cannonBallTrailColor.ValueChanged += (Color value) => { ExplodingCannonballScript.Instance.TrailColor = value; };

            cannonBallTrailLength = new MSlider("Trail Decay Rate", "TrailLength", ExplodingCannonballScript.Instance.TrailLength, 0.01f, 100);
            cannonBallTrailLength.ValueChanged += (float value) => { ExplodingCannonballScript.Instance.TrailLength = Mathf.Clamp(value, 0.001f, Mathf.Infinity); };
        }

        private static void InitArrowSliders()
        {
            ArrowAfterEffectTypeToggle = new MMenu("aftereffecttype", ExplodingArrowsScript.Instance.TypeOfAfterEffect,
                    new List<string>()
                    {
                        "No after effect",
                        "Fire",
                        "Kinetic",
                        "Grenade"
                    }, "Explosion type");
            ArrowAfterEffectTypeToggle.ValueChanged += (int value) =>
            {
                ExplodingArrowsScript.Instance.TypeOfAfterEffect = value;
                bool display = value != 0;
                ArrowAfterEffectImpactDetectionSlider.DisplayInMapper = display;
                ArrowAfterEffectDelaySlider.DisplayInMapper = display;
                ArrowAfterEffectPowerSlider.DisplayInMapper = display;
                ArrowAfterEffectRangeSlider.DisplayInMapper = display;
            };

            ArrowAfterEffectImpactDetectionSlider = new MSlider("Arrow Impact detection", "arrowimpactdetection", ExplodingArrowsScript.Instance.ImpactDetector, 0, 5);
            ArrowAfterEffectImpactDetectionSlider.ValueChanged += (float value) => { ExplodingArrowsScript.Instance.ImpactDetector = value; };

            ArrowAfterEffectDelaySlider = new MSlider("Effect Delay", "arroweaftereffectdelay", ExplodingArrowsScript.Instance.AfterEffectDelay, 0, 10);
            ArrowAfterEffectDelaySlider.ValueChanged += (float value) => { ExplodingArrowsScript.Instance.AfterEffectDelay = value; };

            ArrowAfterEffectPowerSlider = new MSlider("Effect Power", "arroweaftereffectpower", ExplodingArrowsScript.Instance.PowerMultiplierOfExplosion, 0, 20);
            ArrowAfterEffectPowerSlider.ValueChanged += (float value) => { ExplodingArrowsScript.Instance.PowerMultiplierOfExplosion = value; };

            ArrowAfterEffectRangeSlider = new MSlider("Effect range", "arroweaftereffectrange", ExplodingArrowsScript.Instance.RangeMultiplierOfExplosion, 0, 20);
            ArrowAfterEffectRangeSlider.ValueChanged += (float value) => { ExplodingArrowsScript.Instance.RangeMultiplierOfExplosion = value; };

            ArrowTrailEnabled = new MToggle("Enable Arrow Trail", "TrailEnabled", ExplodingArrowsScript.Instance.IsTrailOn);
            ArrowTrailEnabled.Toggled += (bool value) => { ExplodingArrowsScript.Instance.IsTrailOn = value; ArrowTrailColor.DisplayInMapper = value; cannonBallTrailLength.DisplayInMapper = value; };

            ArrowTrailColor = new MColourSlider("Trail Color", "TrailColor", ExplodingArrowsScript.Instance.TrailColor);
            ArrowTrailColor.ValueChanged += (Color value) => { ExplodingArrowsScript.Instance.TrailColor = value; };

            ArrowTrailLength = new MSlider("Trail Decay Rate", "TrailLength", ExplodingArrowsScript.Instance.TrailLength, 0.01f, 100);
            ArrowTrailLength.ValueChanged += (float value) => { ExplodingArrowsScript.Instance.TrailLength = Mathf.Clamp(value, 0.001f, Mathf.Infinity); };
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
        public static bool HasCannonSliders(BlockBehaviour block)
        {
            return !(block.GetBlockID() == (int)BlockType.Cannon) || block.MapperTypes.Exists(match => match.Key == "explosiontype");
        }

        public static bool HasArrowSliders(BlockBehaviour block)
        {
            return !(block.GetBlockID() == (int)BlockType.Crossbow) || block.MapperTypes.Exists(match => match.Key == "aftereffecttype");
        }

        /// <summary>
        /// Adds sliders to all blocks that don't yet have them.
        /// </summary>
        public static void AddAllSliders()
        {
            foreach (BlockBehaviour block in Machine.Active().BuildingBlocks.FindAll(block => !HasCannonSliders(block) || !HasArrowSliders(block)))
            {
                AddSliders(block);
            }
        }

        /// <summary>
        /// Wrapper for AddSliders(BlocKBehaviour) with a check and component retrieval.
        /// </summary>
        /// <param name="block">block Transform</param>
        public static void AddSliders(Transform block)
        {
            BlockBehaviour blockbehaviour = block.GetComponent<BlockBehaviour>();
            if (!HasCannonSliders(blockbehaviour) || !HasArrowSliders(blockbehaviour))
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

                currentMapperTypes.Add(CannonballExplosionTypeToggle);
                currentMapperTypes.Add(CannonballImpactDetectionSlider);
                currentMapperTypes.Add(CannonballExplosionDelaySlider);
                currentMapperTypes.Add(CannonballExplosionPowerSlider);
                currentMapperTypes.Add(explosionRangeSlider);
                currentMapperTypes.Add(cannonBallTrailEnabled);
                currentMapperTypes.Add(cannonBallTrailColor);
                currentMapperTypes.Add(cannonBallTrailLength);

                mapperTypesField.SetValue(block, currentMapperTypes);
            }
            else if (block.GetBlockID() == (int)BlockType.Crossbow)
            {
                var currentMapperTypes = block.MapperTypes;

                currentMapperTypes.Add(ArrowAfterEffectTypeToggle);
                currentMapperTypes.Add(ArrowAfterEffectImpactDetectionSlider);
                currentMapperTypes.Add(ArrowAfterEffectDelaySlider);
                currentMapperTypes.Add(ArrowAfterEffectPowerSlider);
                currentMapperTypes.Add(ArrowAfterEffectRangeSlider);
                currentMapperTypes.Add(ArrowTrailEnabled);
                currentMapperTypes.Add(ArrowTrailColor);
                currentMapperTypes.Add(ArrowTrailLength);

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
        public bool IsTrailOn = true;
        public Color TrailColor = Color.yellow;
        public float TrailLength = 1;
        public String UsingShader = "Particles/Additive";
        public Texture TrailTexture;

        public override string Name { get; } = "Exploding Cannonball Mod";

        void Start()
        {

            Commands.RegisterCommand("ChangeExplosionType", (args, notUses) =>
            {
                try
                {
                    TypeOfExplosion = int.Parse(args[0]);
                    TypeOfExplosion = Mathf.Clamp(TypeOfExplosion, 0, 3);
                    Exploding_CannonBall_Mod.CannonballExplosionTypeToggle.Value = TypeOfExplosion;
                }
                catch { return "Wrong Option! There are four options: \n 0-No Explosion \n 1-Bomb Explosion \n 2-Grenade \n 3-Rocket \n Example: ChangeExplosionType 2"; }

                return "Complete!";

            }, "Change the explosion type of cannonballs");
            Commands.RegisterCommand("ChangeImpactDetection", (args, notUses) =>
            {
                try
                {
                    ImpactDetector = float.Parse(args[0]);
                    Exploding_CannonBall_Mod.CannonballImpactDetectionSlider.Value = ImpactDetector;
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
                    Exploding_CannonBall_Mod.CannonballExplosionDelaySlider.Value = ExplosionDelay;
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
                    Exploding_CannonBall_Mod.CannonballExplosionPowerSlider.Value = PowerMultiplierOfExplosion;
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
            Commands.RegisterCommand("ChangeCannonballTrailColor", (args, notUses) =>
            {
                try
                {
                    TrailColor = new Color(float.Parse(args[0]) / 255, float.Parse(args[1]) / 255, float.Parse(args[2]) / 255, float.Parse(args[3]) / 100);
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the trail color.");

            Commands.RegisterCommand("ChangeCannonballTrailTexture", (args, notUses) =>
            {
                try
                {
                    WWW tex = new WWW("File:///" + Application.dataPath + "/Mods/Resources/CannonTrailTexture.png");
                    TrailTexture = tex.texture;
                }
                catch { return "Wrong Input, the file /Mods/Resources/CannonTrailTexture.png might not exist."; }

                return "Complete!";

            }, "Change the texture of the cannon trail.");

            Commands.RegisterCommand("ChangeCannonballTrailShader", (args, notUses) =>
            {
                switch (UsingShader)
                {
                    case "Particles/Additive":
                        UsingShader = ("FX/Glass/Stained BumpDistort");
                        break;
                    case ("FX/Glass/Stained BumpDistort"):
                        UsingShader = ("Particles/Additive");
                        break;
                }

                return "Complete!";

            }, "Change the trail shader.");

        }


        private void Update()
        {
            GameObject go = GameObject.Find("CanonBallHeavy(Clone)");
            if (go != null)
            {
                if (go.GetComponent<Rigidbody>().velocity.sqrMagnitude > 10 || !IsTrailOn)
                {
                    go.name = "CannonBomb(Clone)";

                    if (IsTrailOn)
                    {
                        TrailRenderer tr = go.AddComponent<TrailRenderer>();
                        tr.startWidth = 0.6f;
                        tr.endWidth = 0.6f;
                        tr.material = new Material(Shader.Find("Particles/Additive"));
                        tr.useLightProbes = true;
                        tr.probeAnchor = go.transform;
                        tr.material.SetColor("_TintColor", TrailColor);
                        tr.time = (go.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (TrailLength + 0.0001f);
                        tr.autodestruct = false;
                    }
                    if (TypeOfExplosion != 0)
                    {
                        go.AddComponent<ExplosionForCannonballs>();
                    }
                }
            }
        }
        private void FixedUpdate()
        {
            GameObject go = GameObject.Find("CanonBallHeavy(Clone)");
            if (go != null)
            {
                if (go.GetComponent<Rigidbody>().velocity.sqrMagnitude > 10 || !IsTrailOn)
                {
                    go.name = "CannonBomb(Clone)";

                    if (IsTrailOn)
                    {
                        TrailRenderer tr = go.AddComponent<TrailRenderer>();
                        tr.startWidth = 0.6f;
                        tr.endWidth = 0.6f;
                        tr.material = new Material(Shader.Find(UsingShader));
                        tr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
                        tr.probeAnchor = go.transform;
                        if (TrailTexture == null)
                        {
                            tr.material.SetColor("_TintColor", TrailColor);
                        }
                        else
                        {
                            tr.material.SetTexture("_MainTex", TrailTexture);
                            tr.material.SetTexture("_BumpMap", TrailTexture);
                        }
                        tr.time = (go.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (TrailLength + 0.0001f);
                        tr.autodestruct = false;
                    }
                    if (TypeOfExplosion != 0)
                    {
                        go.AddComponent<ExplosionForCannonballs>();
                    }
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
            IsTrailOn = Configuration.GetBool("Trail Enabled", IsTrailOn);
            TrailColor = new Color(
                Configuration.GetFloat("Trail Color R", TrailColor.r),
                Configuration.GetFloat("Trail Color G", TrailColor.g),
                Configuration.GetFloat("Trail Color B", TrailColor.b)
                );
            TrailLength = Configuration.GetFloat("Decay Rate", TrailLength);
        }

        internal void SaveConfiguration()
        {
            Configuration.SetInt("Explosion Type", TypeOfExplosion);
            Configuration.SetFloat("Explosion Delay", ExplosionDelay);
            Configuration.SetFloat("Explosion Power", PowerMultiplierOfExplosion);
            Configuration.SetFloat("Explosion Range", RangeMultiplierOfExplosion);
            Configuration.SetFloat("Impact Detection", ImpactDetector);
            Configuration.SetBool("Trail Enabled", IsTrailOn);
            Configuration.SetFloat("Trail Color R", TrailColor.r);
            Configuration.SetFloat("Trail Color G", TrailColor.g);
            Configuration.SetFloat("Trail Color B", TrailColor.b);
            Configuration.SetFloat("Decay Rate", TrailLength);

            Configuration.Save();
        }
    }

    public class ExplodingArrowsScript : SingleInstance<ExplodingArrowsScript>
    {
        public int TypeOfAfterEffect = 1;
        public float AfterEffectDelay = 0;
        public float PowerMultiplierOfExplosion = 1;
        public float RangeMultiplierOfExplosion = 1;
        public float ImpactDetector = 0;
        public bool IsTrailOn = true;
        public Color TrailColor = Color.yellow;
        public float TrailLength = 1;
        public String UsingShader = "Particles/Additive";
        public Texture TrailTexture;

        public override string Name { get; } = "Exploding Arrow Mod";

        void Start()
        {

            Commands.RegisterCommand("ChangeArrowEffectType", (args, notUses) =>
            {
                try
                {
                    TypeOfAfterEffect = int.Parse(args[0]);
                    TypeOfAfterEffect = Mathf.Clamp(TypeOfAfterEffect, 0, 3);
                    Exploding_CannonBall_Mod.CannonballExplosionTypeToggle.Value = TypeOfAfterEffect;
                }
                catch { return "Wrong Option! There are four options: \n 0-No After Effect \n 1-Fire \n 2-Kinetic \n 3-Grenade \n Example: ChangeArrowEffectType 2"; }

                return "Complete!";

            }, "Change the after effect type of arrows");
            Commands.RegisterCommand("ChangeArrowImpactDetection", (args, notUses) =>
            {
                try
                {
                    ImpactDetector = float.Parse(args[0]);
                    Exploding_CannonBall_Mod.CannonballImpactDetectionSlider.Value = ImpactDetector;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change detection of impact for cannonballs");
            Commands.RegisterCommand("ChangeArrowAfterEffectDelay", (args, notUses) =>
            {
                try
                {
                    AfterEffectDelay = float.Parse(args[0]);
                    AfterEffectDelay = Mathf.Clamp(AfterEffectDelay, 0, Mathf.Infinity);
                    Exploding_CannonBall_Mod.CannonballExplosionDelaySlider.Value = AfterEffectDelay;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion delay after cannonballs collide");
            Commands.RegisterCommand("ChangeArrowAfterEffectPower", (args, notUses) =>
            {
                try
                {
                    PowerMultiplierOfExplosion = float.Parse(args[0]);
                    PowerMultiplierOfExplosion = Mathf.Clamp(PowerMultiplierOfExplosion, 0, Mathf.Infinity);
                    Exploding_CannonBall_Mod.CannonballExplosionPowerSlider.Value = PowerMultiplierOfExplosion;
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion power.");
            Commands.RegisterCommand("ChangeArrowTrailColor", (args, notUses) =>
            {
                try
                {
                    TrailColor = new Color(float.Parse(args[0]) / 255, float.Parse(args[1]) / 255, float.Parse(args[2]) / 255, float.Parse(args[3]) / 100);
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the trail color.");

            Commands.RegisterCommand("ChangeArrowTrailTexture", (args, notUses) =>
            {
                try
                {
                    WWW tex = new WWW("File:///" + Application.dataPath + "/Mods/Resources/ArrowTrailTexture.png");
                    TrailTexture = tex.texture;
                }
                catch { return "Wrong Input, the file /Mods/Resources/ArrowTrailTexture.png might not exist."; }

                return "Complete!";

            }, "Change the texture of the cannon trail.");

            Commands.RegisterCommand("ChangeArrowTrailShader", (args, notUses) =>
            {
                switch (UsingShader)
                {
                    case "Particles/Additive":
                        UsingShader = ("FX/Glass/Stained BumpDistort");
                        break;
                    case ("FX/Glass/Stained BumpDistort"):
                        UsingShader = ("Particles/Additive");
                        break;
                }

                return "Complete!";

            }, "Change the trail shader.");

        }


        private void Update()
        {
            GameObject go = GameObject.Find("PHYSICS GOAL/CrossbowBolt(Clone)");
            if (go != null)
            {
                if (go.GetComponent<Rigidbody>().velocity.sqrMagnitude > 10 || !IsTrailOn)
                {
                    go.name = "CrossbowBoltEdited(Clone)";

                    if (IsTrailOn)
                    {
                        TrailRenderer tr = go.AddComponent<TrailRenderer>();
                        tr.startWidth = 0.07f;
                        tr.endWidth = 0.07f;
                        tr.material = new Material(Shader.Find("Particles/Additive"));
                        tr.useLightProbes = true;
                        tr.probeAnchor = go.transform;
                        tr.material.SetColor("_TintColor", TrailColor);
                        tr.time = (go.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (TrailLength + 0.0001f);
                        tr.autodestruct = false;
                    }
                    if (TypeOfAfterEffect != 0)
                    {
                        go.AddComponent<AfterEffectsForArrows>();
                    }
                }
            }
        }
        private void FixedUpdate()
        {
            GameObject go = GameObject.Find("PHYSICS GOAL/CrossbowBolt(Clone)");
            if (go != null)
            {
                if (go.GetComponent<Rigidbody>().velocity.sqrMagnitude > 10 || !IsTrailOn)
                {
                    go.name = "CrossbowBoltEdited(Clone)";

                    if (IsTrailOn)
                    {
                        TrailRenderer tr = go.AddComponent<TrailRenderer>();
                        tr.startWidth = 0.07f;
                        tr.endWidth = 0.07f;
                        tr.material = new Material(Shader.Find(UsingShader));
                        tr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
                        tr.probeAnchor = go.transform;
                        if (TrailTexture == null)
                        {
                            tr.material.SetColor("_TintColor", TrailColor);
                        }
                        else
                        {
                            tr.material.SetTexture("_MainTex", TrailTexture);
                            tr.material.SetTexture("_BumpMap", TrailTexture);
                        }
                        tr.time = (go.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (TrailLength + 0.0001f);
                        tr.autodestruct = false;
                    }
                    if (TypeOfAfterEffect != 0)
                    {
                        go.AddComponent<AfterEffectsForArrows>();
                    }
                }
            }
        }

        internal void LoadConfiguration()
        {
            TypeOfAfterEffect = Mathf.Clamp(Configuration.GetInt("Arrow After Effect Type", TypeOfAfterEffect), 0, 3);
            AfterEffectDelay = Configuration.GetFloat("Arrow After Effect Delay", AfterEffectDelay);
            PowerMultiplierOfExplosion = Configuration.GetFloat("Arrow After Effect Power", PowerMultiplierOfExplosion);
            RangeMultiplierOfExplosion = Configuration.GetFloat("Arrow After Effect Range", RangeMultiplierOfExplosion);
            ImpactDetector = Configuration.GetFloat("Arrow Impact Detection", ImpactDetector);
            IsTrailOn = Configuration.GetBool("Arrow Trail Enabled", IsTrailOn);
            TrailColor = new Color(
                Configuration.GetFloat("Arrow Trail Color R", TrailColor.r),
                Configuration.GetFloat("Arrow Trail Color G", TrailColor.g),
                Configuration.GetFloat("Arrow Trail Color B", TrailColor.b)
                );
            TrailLength = Configuration.GetFloat("Arrow Trail Decay Rate", TrailLength);
        }

        internal void SaveConfiguration()
        {
            Configuration.SetInt("Arrow After Effect Type", TypeOfAfterEffect);
            Configuration.SetFloat("Arrow After Effect Delay", AfterEffectDelay);
            Configuration.SetFloat("Arrow After Effect Power", PowerMultiplierOfExplosion);
            Configuration.SetFloat("Arrow After Effect Range", RangeMultiplierOfExplosion);
            Configuration.SetFloat("Arrow Impact Detection", ImpactDetector);
            Configuration.SetBool("Arrow Trail Enabled", IsTrailOn);
            Configuration.SetFloat("Arrow Trail Color R", TrailColor.r);
            Configuration.SetFloat("Arrow Trail Color G", TrailColor.g);
            Configuration.SetFloat("Arrow Trail Color B", TrailColor.b);
            Configuration.SetFloat("Arrow Trail Decay Rate", TrailLength);

            Configuration.Save();
        }
    }


    public class ExplosionForCannonballs : MonoBehaviour
    {
        public ExplodingCannonballScript ECS;
        private float CountDownExplode;
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
            CountDownExplode = ECS.ExplosionDelay * 100;

        }
        void Update()
        {
            if (!Exploding)
                CountDownExplode = (int)(ECS.ExplosionDelay * 100);
        }
        void FixedUpdate()
        {
            ++FrameCount;
            TrailRenderer TR = this.GetComponent<TrailRenderer>();
            if (TR != null) TR.time = (this.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (ECS.TrailLength + 0.0001f);
        }
        void OnCollisionEnter(Collision coll)
        {
            if ((!Exploding || coll.relativeVelocity.magnitude > ECS.ImpactDetector) && FrameCount > 2)
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

    public class AfterEffectsForArrows : MonoBehaviour
    {
        public ExplodingArrowsScript EAS;
        public FireTag FT;
        private float CountDownExplode;
        private bool Exploding = false;
        private int FrameCount = 0;
        private Collider coooll;

        IEnumerator AE()
        {
            if (Exploding)
            {
                while (CountDownExplode >= 0 && EAS.TypeOfAfterEffect == 3)
                {
                    yield return new WaitForFixedUpdate();
                    --CountDownExplode;
                    StartCoroutine(AE());
                    yield break;
                }

                if (EAS.TypeOfAfterEffect == 3)
                {
                    GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[54].gameObject, this.transform.position, this.transform.rotation);
                    explo.transform.localScale = Vector3.one * 0.01f;
                    ControllableBomb ac = explo.GetComponent<ControllableBomb>();
                    ac.radius = 3 * EAS.RangeMultiplierOfExplosion;
                    ac.power = 1500 * EAS.PowerMultiplierOfExplosion;
                    ac.randomDelay = 0.00001f;
                    ac.upPower = 0f;
                    ac.StartCoroutine_Auto(ac.Explode());
                    explo.AddComponent<TimedSelfDestruct>();
                    Destroy(this.gameObject);
                }
                else if (EAS.TypeOfAfterEffect == 2)
                {
                    if (coooll.attachedRigidbody)
                    {
                        coooll.attachedRigidbody.AddForce((coooll.attachedRigidbody.worldCenterOfMass - this.transform.position).normalized * EAS.PowerMultiplierOfExplosion * 4000);
                    }
                }
            }
        }
        void Start()
        {
            EAS = ExplodingArrowsScript.Instance;
            FT = GetComponent<FireTag>();
            CountDownExplode = EAS.AfterEffectDelay * 100;
        }
        void Update()
        {
            if (Exploding) { return; }

            if (!Exploding)
                CountDownExplode = (int)(EAS.AfterEffectDelay * 100);

            if (!FT.burning)
            {
                if (EAS.TypeOfAfterEffect == 1)
                {
                    this.FT.Ignite();
                }
            }
        }
        void FixedUpdate()
        {
            if (Exploding) { return; }
            ++FrameCount;
            TrailRenderer TR = this.GetComponent<TrailRenderer>();
            if (TR != null) TR.time = (this.GetComponent<Rigidbody>().velocity.magnitude + 0.0001f) / (EAS.TrailLength + 0.0001f);

            if (!FT.burning)
            {
                if (EAS.TypeOfAfterEffect == 1)
                {
                    this.FT.Ignite();
                }
            }
        }
        void OnTriggerEnter(Collider coll)
        {
            if (coll.attachedRigidbody)
            {
                if ((!Exploding || (coll.attachedRigidbody.velocity - this.GetComponent<Rigidbody>().velocity).sqrMagnitude > EAS.ImpactDetector * EAS.ImpactDetector) && FrameCount > 2)
                {
                    Exploding = true;
                    coooll = coll;
                    StartCoroutine(AE());
                }
            }
            else
            {
                if ((!Exploding || (Vector3.zero - this.GetComponent<Rigidbody>().velocity).sqrMagnitude > EAS.ImpactDetector * EAS.ImpactDetector) && FrameCount > 2)
                {
                    Exploding = true;
                    coooll = coll;
                    StartCoroutine(AE());
                }
            }
        }
        //void OnCollisionStay(Collision coll)
        //{
        //    if ((!Exploding || coll.relativeVelocity.sqrMagnitude > EAS.ImpactDetector * EAS.ImpactDetector) && FrameCount > 2)
        //    {
        //        Exploding = true;
        //        coooll = coll;
        //        StartCoroutine(AE());
        //    }
        //}
    }

    public class TimedSelfDestruct : MonoBehaviour
    {
        float timer = 0;
        void FixedUpdate()
        {
            ++timer;
            if (timer > 260)
            {
                Destroy(this.gameObject);
            }
            if (this.GetComponent<TimedRocket>())
            {
                Destroy(this.GetComponent<TimedRocket>());
            }
        }
    }
}
