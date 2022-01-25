using UnityEngine;
using UnityEngine.UI;

namespace TagsAndLayers.Demo
{
    [ExecuteInEditMode]
    public class TagsAndLayers_Example : MonoBehaviour
    {
        [SerializeField]
        private Text textField = null;

        [TagDropdown]
        public string tagToSelect;
        
        [LayerDropdown]
        public int layerToSelect;

        private void LateUpdate()
        {
            if (gameObject.tag == tagToSelect &&
                gameObject.layer == layerToSelect)
            {
                return;
            }

            gameObject.tag = tagToSelect;
            gameObject.layer = layerToSelect;

            if (textField != null)
            {
                textField.text =
                    $"Selected tag: {gameObject.tag}\n" +
                    $"Selected layer: {LayerMask.LayerToName(layerToSelect)}";
            }
        }
    }
}
