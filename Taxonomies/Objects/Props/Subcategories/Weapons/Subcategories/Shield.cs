using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.Weapons.Subcategories
{
    /// <summary>
    /// <b>Shield</b> class allows to create a shield for defending from a <see cref="Weapon"/>.
    /// </summary>
    [ECARules4All("shield")]
    [RequireComponent(typeof(Weapon))]
    [DisallowMultipleComponent]
    public class Shield : MonoBehaviour
    {
        /// <summary>
        /// <b>Blocks</b>: This action allows to block the <see cref="Weapon"/> attack.
        /// </summary>
        /// <param name="weapon"></param>
        [Action(typeof(Shield), "blocks", typeof(Weapon))]
        public void Blocks(Weapon weapon)
        {
            if (weapon.power > 0.0f)
            {
                ECACharacter c = GetComponentInParent<ECACharacter>();
                if (c != null)
                {
                    c.life -= weapon.power / (1 + GetComponent<Weapon>().power);
                }
            }
        }
    }
}
