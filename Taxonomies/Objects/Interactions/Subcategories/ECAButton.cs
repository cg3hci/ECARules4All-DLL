using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories
{
    /// <summary>
    /// <b>Button</b> is an <see cref="ECAInteraction"/> subclass that represents a button.
    /// When a <see cref="ECAButton"/> is pressed, it will trigger an event defined by the End User Developer.
    /// </summary>
    [ECARules4All("button")]
    [RequireComponent(typeof(ECAInteraction))]
    [DisallowMultipleComponent]
    public class ECAButton : MonoBehaviour
    {
        /// <summary>
        /// <b>Presses</b> is a <i>passive</i> function that represents the pressing of the button by a character C.
        /// </summary>
        /// <param name="c">The <see cref="ECACharacter"/> who presses the button. </param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "pushes", typeof(ECAButton))]
        public void _Presses(ECACharacter c)
        {
            //no need for implementation, see Interactable class (behaviour)
        }
    }
}
