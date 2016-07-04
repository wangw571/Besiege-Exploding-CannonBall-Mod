using spaar.ModLoader;
using spaar.ModLoader.UI;
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
        {            get            {                return "v0.30";            }        }

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
        {get{return new Version("0.3");}}
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

        void Start()
        {
            Commands.RegisterCommand("ChangeExplosionType", (args, notUses) =>
            {
                try
                {
                    TypeOfExplosion = int.Parse(args[0]);
                }
                catch { return "Wrong Option! There are four options: \n 0-No Explosion \n 1-Bomb Explosion \n 2-Grenade \n 3-Rocket \n Example: ChangeExplosionType 2"; }

                return "Complete!";
                 
            }, "Change the explosion type of cannonballs");
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
    }

    public class ExplosionForCannonballs:MonoBehaviour
    {
        public GameObject TheParentOfAll;
        void Start()
        {
            TheParentOfAll = GameObject.Find("Exploding Cannonball Mod");
        }
        void OnCollisionEnter(Collision coll)
        {
            
            if (TheParentOfAll.GetComponent<ExplodingCannonballScript>().TypeOfExplosion == 1 )
            {
                GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[23].gameObject, this.transform.position, this.transform.rotation);
                explo.transform.localScale = Vector3.one * 0.01f;
                ExplodeOnCollideBlock ac = explo.GetComponent<ExplodeOnCollideBlock>();
                ac.radius = 7;
                ac.power = 2100f;
                ac.torquePower = 100000;
                ac.upPower = 0;
                ac.Explodey();
                Destroy(this.gameObject);
            }
            else if (TheParentOfAll.GetComponent<ExplodingCannonballScript>().TypeOfExplosion == 2)
            {
                GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[54].gameObject, this.transform.position, this.transform.rotation);
                explo.transform.localScale = Vector3.one * 0.01f;
                ControllableBomb ac = explo.GetComponent<ControllableBomb>();
                ac.radius = 3;
                ac.power = 1500;
                ac.randomDelay = 0.00001f;
                ac.upPower = 0f;
                ac.StartCoroutine_Auto(ac.Explode());
                Destroy(this.gameObject);
            }
            else if (TheParentOfAll.GetComponent<ExplodingCannonballScript>().TypeOfExplosion == 3)
            {
                GameObject explo = (GameObject)GameObject.Instantiate(PrefabMaster.BlockPrefabs[59].gameObject, this.transform.position, this.transform.rotation);
                explo.transform.localScale = Vector3.one * 0.01f;
                TimedRocket ac = explo.GetComponent<TimedRocket>();
                ac.SetSlip(Color.white);
                ac.radius = 3;
                ac.power = 1500;
                ac.randomDelay = 0.000001f;
                ac.upPower = 0;
                ac.StartCoroutine(ac.Explode(0));
                Destroy(this.gameObject);
            }
        }
    }
}