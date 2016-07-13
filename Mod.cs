using spaar.ModLoader;
using spaar.ModLoader.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Exploding_CannonBall_Mod
{
    public class Exploding_CannonBall_Mod : Mod
    {
        public GameObject temp;

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
            temp = new GameObject();
            temp.name = "Exploding Cannonball Mod";
            temp.AddComponent < ExplodingCannonballScript >();
            GameObject.DontDestroyOnLoad(temp);
        }

        public override void OnUnload()
        {
            GameObject.Destroy(this.temp);
        }
    }

    public class ExplodingCannonballScript : MonoBehaviour
    {
        public int TypeOfExplosion = 1;
        public float ExplosionDelay = 0;
        public float PowerMultiplierOfExplosion = 1;
        public float RangeMultiplierOfExplosion = 1;
        public float ImpactDetector = 0;
        void Start()
        {
            
            Commands.RegisterCommand("ChangeExplosionType", (args, notUses) =>
            {
                try
                {
                    TypeOfExplosion = int.Parse(args[0]);
                    Configuration.SetInt("Explosion Type", TypeOfExplosion);
                    Configuration.Save();
                    TypeOfExplosion = Mathf.Clamp(TypeOfExplosion, 0, 3);
                }
                catch { return "Wrong Option! There are four options: \n 0-No Explosion \n 1-Bomb Explosion \n 2-Grenade \n 3-Rocket \n Example: ChangeExplosionType 2"; }

                return "Complete!";
                 
            }, "Change the explosion type of cannonballs");
            Commands.RegisterCommand("ChangeImpactDetection", (args, notUses) =>
            {
                try
                {
                    ImpactDetector = float.Parse(args[0]);
                    Configuration.SetFloat("Impact Detection", ImpactDetector);
                    Configuration.Save();
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change detection of impact for cannonballs");
            Commands.RegisterCommand("ChangeExplosionDelay", (args, notUses) =>
            {
                try
                {
                    ExplosionDelay = float.Parse(args[0]);
                    Configuration.SetFloat("Explosion Delay", ExplosionDelay);
                    Configuration.Save();
                    ExplosionDelay = Mathf.Clamp(ExplosionDelay, 0, Mathf.Infinity);
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion delay after cannonballs collide");
            Commands.RegisterCommand("ChangeExplosionPowerX", (args, notUses) =>
            {
                try
                {
                    PowerMultiplierOfExplosion = float.Parse(args[0]);
                    Configuration.SetFloat("Explosion Power", PowerMultiplierOfExplosion);
                    Configuration.Save();
                    PowerMultiplierOfExplosion = Mathf.Clamp(PowerMultiplierOfExplosion, 0, Mathf.Infinity);
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion power.");
            Commands.RegisterCommand("ChangeExplosionRangeX", (args, notUses) =>
            {
                try
                {
                    RangeMultiplierOfExplosion = float.Parse(args[0]);
                    Configuration.SetFloat("Explosion Range", RangeMultiplierOfExplosion);
                    Configuration.Save();
                    RangeMultiplierOfExplosion = Mathf.Clamp(RangeMultiplierOfExplosion, 0, Mathf.Infinity);
                }
                catch { return "Wrong Input"; }

                return "Complete!";

            }, "Change the explosion range.");
            Settings();
        }


        private void Update()
        {
            Settings();
            TypeOfExplosion = Mathf.Clamp(TypeOfExplosion, 0, 3);
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

        void Settings()
        {
            if (Configuration.DoesKeyExist("Explosion Type"))
            {
                TypeOfExplosion = Configuration.GetInt("Explosion Type", TypeOfExplosion);
            }
                Configuration.SetInt("Explosion Type", TypeOfExplosion);
            

            if (Configuration.DoesKeyExist("Explosion Delay"))
            {
                ExplosionDelay = Configuration.GetFloat("Explosion Delay", ExplosionDelay);
            }
                Configuration.SetFloat("Explosion Delay", ExplosionDelay);
            

            if (Configuration.DoesKeyExist("Explosion Power"))
            {
                PowerMultiplierOfExplosion = Configuration.GetFloat("Explosion Power", PowerMultiplierOfExplosion);
            }
                Configuration.SetFloat("Explosion Power", PowerMultiplierOfExplosion);
            

            if (Configuration.DoesKeyExist("Explosion Range"))
            {
                RangeMultiplierOfExplosion = Configuration.GetFloat("Explosion Range", RangeMultiplierOfExplosion);
            }
                Configuration.SetFloat("Explosion Range", RangeMultiplierOfExplosion);

            if (Configuration.DoesKeyExist("Impact Detection"))
            {
                ImpactDetector = Configuration.GetFloat("Impact Detection", ImpactDetector);
            }
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
            ECS = GameObject.Find("Exploding Cannonball Mod").GetComponent<ExplodingCannonballScript>();

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
