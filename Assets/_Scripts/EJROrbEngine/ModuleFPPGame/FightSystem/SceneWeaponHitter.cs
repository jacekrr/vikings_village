// **** Autor: Jacek Ross - games@ejr.com.pl
// **** Plik jest czescia silnika EJROrb użytego w projektach m.in. SymulatorZielarza, Gułag, Vikings Village, wiecej info: http://ejr.com.pl
// **** Wszelkie prawa zastrzeżone: EJR Sp. z o.o. 2016-2020

// **** Author: Jacek Ross - games@ejr.com.pl
// **** This file is a part of an EJROrb engine used in projects: HerbalistSimulator, Gulag, Vikings Village, more info: http://ejr.com.pl
// **** Copyrights: EJR Sp. z o.o.


using EJROrbEngine.FPPGame;
using UnityEngine;
namespace EJROrbEngine.SceneObjects
{

    public class SceneWeaponHitter : MonoBehaviour
    {
        public const float CRITICAL_BONUS = 4f;
        public SceneWeapon RealWeapon;

        public void GeneralHited(SceneDestructible hitedObj, Vector3 hitPoint)
        {
            if (RealWeapon.IsAttacking )
            {
                float hitAmnt = CalculateHitAmmount(RealWeapon.WeaponData, hitedObj);
                hitedObj.Hit(gameObject, hitPoint, hitAmnt);
                Debug.Log("Trafienie w " + hitedObj.gameObject.name + " na " + hitAmnt + " HP");
            }
        }
        public void PlayerHited()
        {
            float hitAmnt = CalculateHitAmmount(RealWeapon.WeaponData, null);
            GameManager.Instance.PlayerWasHited(hitAmnt);
            Debug.Log("Trafienie w gracza  na " + hitAmnt + " HP");
            RealWeapon.IsAttacking = false;
        }

      
        private void OnHit(Collider other)
        {
            if (RealWeapon.IsAttacking && other.gameObject.GetComponent<SceneDestructible>() != null)
            {
                GeneralHited(other.gameObject.GetComponent<SceneDestructible>(), CalculateHitPoint(other.gameObject));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (RealWeapon.IsAttacking && (RealWeapon.CurrentAttackMode == FightSystem.AttackMode.ThrowAttack || RealWeapon.WeaponData["scanHit"] == null || (bool)RealWeapon.WeaponData["scanHit"] == false) && collision.collider.gameObject.GetComponent<SceneDestructible>() != null)
                OnHit(collision.collider);
            RealWeapon.IsAttacking = false;
        }
      

        private float CalculateHitAmmount(BaseDataAddon weaponData, SceneDestructible hited)
        {
            float amnt = 0;
            if ((int)weaponData["meleeMin"] > 0 && (int)weaponData["meleeMax"] > 0 && (int)weaponData["meleeMin"] <= (int)weaponData["meleeMax"])
                amnt = Random.Range((int)weaponData["meleeMin"], (int)weaponData["meleeMax"]);
            else
            if ((int)weaponData["shootMin"] > 0 && (int)weaponData["shootMax"] > 0 && (int)weaponData["shootMin"] <= (int)weaponData["shootMax"])
                amnt = Random.Range((int)weaponData["shootMin"], (int)weaponData["shootMax"]) * RangeModfier(weaponData, GameManager.Instance.ThePlayerController.gameObject);

            if ((int)weaponData["critical"] > 0 && Random.Range(0, 100) < (int)weaponData["critical"])
                amnt *= CRITICAL_BONUS;
            if (hited != null && hited.DestructibleData["hitType"] != null && weaponData[(string)hited.DestructibleData["hitType"] + "Bonus"] != null)
                amnt += (amnt * (int)weaponData[(string)hited.DestructibleData["hitType"] + "Bonus"]) / 100;
            return amnt;
        }
        private float RangeModfier(BaseDataAddon weaponData, GameObject hited)
        {
            float distance = (hited.transform.position - gameObject.transform.position).magnitude;
            if (distance <= 0.5f * (float)weaponData["shootRange"])
                return 1;
            if(distance <= (float)weaponData["shootRange"])
                return 1 - distance / (float)weaponData["shootRange"];
            return 0;
        }
        private Vector3 CalculateHitPoint(GameObject hitted)
        {
            //first: try raycast between hitter and hitted centers
            Ray r = new Ray(transform.position, hitted.transform.position - transform.position);
            RaycastHit rh;
            if (Physics.Raycast(r, out rh))
                return rh.point;
            //second: try raycast between hitter center through camera ray
            r = new Ray(transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(r, out rh))
                return rh.point;
            //third: try to find closest point
            return hitted.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        }
    }

}